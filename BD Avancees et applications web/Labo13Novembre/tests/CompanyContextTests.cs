using System.Threading.Tasks;
using AccesConcurrentsNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;



// 0. Loading related entities : 
// 1. Deferred query execution
// 2. Change Tracking
// 3. Id? => references/navigation properties




namespace tests
{
    [TestClass]
    public class CompanyContextTests
    {

       
        CompanyContext _context;
        [AssemblyInitialize]
        public static void GlobalSetup(TestContext testContext){
            //Cette méthode s'exécutera une seule fois, avant le tout premier test joué.
            //Voir attribut AssemblyInitialize
            using(var tmpContext=new CompanyContext(GetDbContextOptions()))
            {
                tmpContext.Database.EnsureDeleted();
                tmpContext.Database.EnsureCreated();
            }
        }

        [TestInitialize]
        public async Task Setup()
        {
            //cette méthode s'exécutera n fois (n = nombre de tests)
            //avant chaque test.
            //Voir attribut TestInitialize
            _context=new CompanyContext(GetDbContextOptions());
            await CreerClient();
        }

        [TestMethod]
        public async Task First_RetourneUnElement()
        {
            //Faire un FirstAsync a-t-il beaucoup d'intérêt niveau métier?
            Customer client=await _context.Customers.FirstAsync();
            Assert.AreEqual("5000", client.PostCode);
        }

        [TestMethod]
        public async Task TestInsertionClient(){
           Customer clientCree=await CreerClient();
           // Find = même chose que First(c=>c.Id==clientCree.Id)
           // mais il recherche implicitement sur la clé primaire de l'entité.
           Customer clientTrouve=await _context.Customers.FindAsync(clientCree.Id);
           Assert.IsNotNull(clientTrouve);
        }


        [TestMethod]
        public async Task RechercheClientParNom(){
             Customer clientCree=await CreerClient();
             Customer clientTrouve=await _context.Customers.FirstOrDefaultAsync(c=>c.Name.Contains(clientCree.Name));
             
             Assert.IsNotNull(clientTrouve);
        }

        [TestMethod]
        public async Task MiseAJourFonctionne(){
            Customer clientCree=await CreerClient();
            await ModifierClient(clientCree.Id, "Super nouvelle remarque!!!");
            Customer clientMisAJour =_context.Customers.Find(clientCree.Id);
            Assert.AreEqual("Super nouvelle remarque!!!", clientMisAJour.Remark);
        }

        [TestMethod]
        public async Task FirstOrDefaultAsync_ReturnsNullIfNotFound(){
             Customer clientTrouve=await _context.Customers.FirstOrDefaultAsync(c=>c.Name.Equals("zzzzz"));
             Assert.IsNull(clientTrouve);
        }

         [TestMethod]
        public async Task SuppressionFonctionne(){
            
             Customer clientCree=await CreerClient();
             await SupprimerClient(clientCree.Id);
             Customer clientTrouve=await _context.Customers.FindAsync(clientCree.Id);
             Assert.IsNull(clientTrouve);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void DetecteLesEditionsConcurrentes()
        {
            using (CompanyContext contexteDeJohn = new CompanyContext(GetDbContextOptions()))
            {
                using (CompanyContext contexteDeSarah = new CompanyContext(GetDbContextOptions()))
                {
                    var clientDeJohn = contexteDeJohn.Customers.First();
                    var clientDeSarah = contexteDeSarah.Customers.First();

                    clientDeJohn.AccountBalance += 1000;
                    contexteDeJohn.SaveChanges();

                    clientDeSarah.AccountBalance += 2000;

                    contexteDeSarah.SaveChanges();


                }
            }
        }

        #region helper methods

        private async Task ModifierClient(long customerId, string nouvelleRemarque)
        {
            //Pourquoi ne pas utiliser la variable d'instance _context?
            //Pourquoi réinstancier un nouveau contexte pour faire la modification?
            // => pour des raisons de caching. Si on fait l'update puis la recherche sur la 
            // même instance de contexte, le contexte renverra l'instance qu'il possède en cache
            // et n'ira pas jusqu'à la DB. En utilisant des contextes différents, 
            // on s'assure qu'on va jusqu'à la DB lors de la recherche ultérieure. 
            using(var tmpContext=new CompanyContext(GetDbContextOptions()))
            {
              Customer clientTrouve=tmpContext.Customers.Find(customerId);
              clientTrouve.Remark=nouvelleRemarque;
              await tmpContext.SaveChangesAsync();
            }
        }
        
        private async Task SupprimerClient(long customerId){
            using(var tmpContext=new CompanyContext(GetDbContextOptions()))
            {
              Customer clientTrouve=tmpContext.Customers.Find(customerId);
              tmpContext.Customers.Remove(clientTrouve);
              await tmpContext.SaveChangesAsync();
            }
        }

        private async Task<Customer> CreerClient(){
            var johnDoe=new Customer(){
                     AccountBalance=100,
                      AddressLine1="Rue de Bruxelles, 61",
                       PostCode="5000",
                        Country="Belgique",
                         City="Namur",
                         EMail="john@doe.com",
                         Name="John Doe",
                          Remark="Fauché"
            };
             
            using(var tempContext=new CompanyContext(GetDbContextOptions()))
            {
                tempContext.Customers.Add(johnDoe);
                await tempContext.SaveChangesAsync();
            };
            return johnDoe;
        }

         private static DbContextOptions GetDbContextOptions(){
            DbContextOptionsBuilder builder=new DbContextOptionsBuilder();
            string connectionString=null;
                if(connectionString==null)
                    throw new Exception("!!!!!!!!!!!!!!!!!!!!! ==========================> Veuillez renseigner votre connection string :)");
            DbContextOptions options = builder.UseSqlServer(connectionString).Options;
            return options;
        }
        #endregion
    }
}
