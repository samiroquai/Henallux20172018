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
    }
}
