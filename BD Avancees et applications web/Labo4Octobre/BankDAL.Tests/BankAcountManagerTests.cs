using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankDAL.Tests
{
    [TestClass]
    public class BankAccountManagerTests
    {
         [TestInitialize]
        public void AvantChaqueTest()
        {
           BankAccountManager manager = new BankAccountManager();
           manager.SupprimerComptes();
           manager.CreerCompte("BE68539007547034", 54.75);
           manager.CreerCompte("BE987654321", 655);
           manager.CreerCompte("BE666555777888", 776);
           if(File.Exists("Log"))
                File.Delete("Log");
                
        }

        [TestMethod]
        [ExpectedException(typeof(BankAccountNotFoundException))]
        public void LeveExceptionSiCompteEnBanqueOrigineInexistant()
        {
            BankAccountManager manager = new BankAccountManager();
            manager.TransfererArgent("CetIBANNexistePas", "BE68539007547034", 123);
        }

        [TestMethod]
        [ExpectedException(typeof(BankAccountNotFoundException))]
        public void LeveExceptionSiCompteEnBanqueDestinationInexistant()
        {
            BankAccountManager manager = new BankAccountManager();
            manager.TransfererArgent("BE68539007547034", "CetIBANNexistePas", 123);
        }

        [TestMethod]
        public void ObtenirSoldeFonctionne()
        {
            BankAccountManager manager = new BankAccountManager();
            double solde = manager.ObtenirSolde("BE68539007547034");
            //le solde de départ est défini dans le script de création de la base de données.
            Assert.AreEqual(54.75, solde);
        }

        [TestMethod]
        public void LaisseBaseDeDonneesDansEtatCoherentSiErreurDeTransfert()
        {
            BankAccountManager manager = new BankAccountManager();
            try
            {
                manager.TransfererArgent("BE68539007547034", "CetIBANNexistePas", 123);
            }
            catch { }
            double soldeApresOperation = manager.ObtenirSolde("BE68539007547034");
            //le solde après opération doit être celui de départ, car l'opération de transfert n'a pas pu se produire
            //étant donné que le compte de destination n'existe pas.
            //le solde de départ est défini dans le script de création de la base de données.
            Assert.AreEqual(54.75, soldeApresOperation);
        }

# region concurrency examples
        [TestMethod]
        public void DeadlockExample()
        {
            using(var logger=new OperationsLogger("log1"))
            {
                var john=new BankAccountManager(idleTimeInSeconds:15, managerName:"John", logger: logger);
                var steve=new BankAccountManager(managerName:"Steve",logger: logger);

                Task johnsTransfer=Task.Run(()=>john.TransfererArgent("BE68539007547034","BE987654321",10));
                //on octroie du temps à John pour lui laisser une chance de démarrer avant Steve
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Task stevesTransfer=Task.Run(()=>steve.TransfererArgent("BE987654321","BE68539007547034",3));
                Task.WaitAll(johnsTransfer, stevesTransfer);
                // => une des deux transactions ne va pas pouvoir aller jusqu'au bout. Voici la séquence des opérations
                // John a débité le compte BE68539007547034 puis s'est endormi pour 15 secondes. Pendant ce temps de repos,
                // Steve a débité le compte BE987654321, puis il enchaîne tout de suite 
                // en créditant le compte BE68539007547034
                // => problème! John a commencé une transaction qui est toujours en cours et qui implique une modification sur le compte que Steve essaie de modifier. Steve doit donc attendre que John termine sa transaction
                // John se réveille, essaie ensuite de passer à la seconde opération, le crédit du compte BE987654321. 
                // => problème! Steve a commencé une transaction qui est toujours en cours et qui implique une modification sur le compte que 
                // John essaie de modifier. John doit donc attendre que Steve termine sa transaction.
                // Or, tous les deux attendent une ressource que l'autre a verrouillé => DEADLOCK => Une des deux transactions doit être annulée. 


            }
        }

         [TestMethod]
        public void SerializableExample()
        {
            using(var logger=new OperationsLogger("log2"))
            {
                var john=new BankAccountManager(idleTimeInSeconds:15, managerName:"John", logger: logger);
                var steve=new BankAccountManager(managerName:"Steve",logger: logger);

                Task johnsTransfer=Task.Run(()=>john.TransfererArgent("BE68539007547034","BE987654321",10));
                //on octroie du temps à John pour lui laisser une chance de démarrer avant Steve
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Task stevesTransfer=Task.Run(()=>steve.TransfererArgent("BE68539007547034","BE666555777888",3));
                Task.WaitAll(johnsTransfer, stevesTransfer);
                //Les deux transactions vont opérer sur le même compte donneur d'ordre. La seconde transaction
                //doit attendre la fin de la première en mode Serializable. 

            }
        }

        [TestMethod]
        public void NoDirtyWritesExample()
        {
            using(var logger=new OperationsLogger("log3"))
            {
                var john=new BankAccountManager(idleTimeInSeconds:15, managerName:"John", logger: logger, isolationLevel: System.Data.IsolationLevel.ReadUncommitted);
                var steve=new BankAccountManager(managerName:"Steve",logger: logger, isolationLevel: System.Data.IsolationLevel.ReadUncommitted);

                Task johnsTransfer=Task.Run(()=>john.TransfererArgent("BE68539007547034","BE987654321",10));
                //on octroie du temps à John pour lui laisser une chance de démarrer avant Steve
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Task stevesTransfer=Task.Run(()=>steve.TransfererArgent("BE68539007547034","BE666555777888",3));
                Task.WaitAll(johnsTransfer, stevesTransfer);
                // Nous ne sommes pas en Serializable, mais l'effet est le même,
                // car les dirty writes ne sont pas autorisés. 

            }
        }

        #endregion
    }

    public class OperationsLogger : IOperationLogger, IDisposable
    {
        private StreamWriter writer;
        public OperationsLogger(string logName)
        {
            writer=new StreamWriter(File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),logName+".txt"), FileMode.Create));
        }

        void IDisposable.Dispose()
        {
            writer.Flush();
            writer.Dispose();
        }

        void IOperationLogger.Log(string message)
        {
           writer.WriteLine(message);
        }
    }
}
