using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankDAL
{
    public class BankAccountManager
    {
        private string _managerName;
        private int _idleTimeInSeconds=0;
        private IOperationLogger _logger=null;
        private System.Data.IsolationLevel _isolationLevel;
        public BankAccountManager(int idleTimeInSeconds=0, string managerName=null, IOperationLogger logger=null, System.Data.IsolationLevel isolationLevel=System.Data.IsolationLevel.ReadCommitted)
        {
            _idleTimeInSeconds=idleTimeInSeconds;
            _managerName=managerName;
            _logger=logger;
            _isolationLevel=isolationLevel;
        }
        public void SupprimerComptes()
        { 
            using (SqlConnection cn = GetDatabaseConnection())
            {
                cn.Open();
                var command = new SqlCommand("DELETE FROM [CompteEnBanque]", cn);
                command.ExecuteReader();
                cn.Close();
            }
        }

        public void CreerCompte(string iban, double montantInitial)
        { 
            using (SqlConnection cn = GetDatabaseConnection())
            {
                cn.Open();
                var command = new SqlCommand("INSERT INTO [CompteEnBanque] ([Solde], [IBAN]) VALUES (@solde, @iban)", cn);
                command.Parameters.AddWithValue("@solde", montantInitial);
                command.Parameters.AddWithValue("@iban", iban);
                ExecuterRequeteEtVerifierImpact(command);
                cn.Close();
            }
        }
        
        public void TransfererArgent(string ibanOrigine, string ibanDestination, int montantATransferer)
        {
            using (SqlConnection cn = GetDatabaseConnection())
            {
                cn.Open();
                Log("Transaction démarrée. Niveau d'isolation: "+_isolationLevel);
                using(SqlTransaction tn=cn.BeginTransaction(_isolationLevel))
                {
                    try
                    {
                        DebiterDe(montantATransferer, ibanOrigine, cn, tn);
                        if(_idleTimeInSeconds>0)
                        Thread.Sleep(TimeSpan.FromSeconds(_idleTimeInSeconds));
                        CrediterDe(montantATransferer, ibanDestination, cn, tn);
                        tn.Commit();
                    }
                    catch
                    {
                        tn.Rollback();
                        Log("Erreur lors de la transaction opérée par "+_managerName+". Annulation de celle-ci.");
                        throw;
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            }
        }

        private void CrediterDe(int montantAAjouter, string iban, SqlConnection cn, SqlTransaction tn)
        {
            var command = new SqlCommand("UPDATE [CompteEnBanque] SET [Solde]=[Solde]+@montantAAjouter WHERE [IBAN]=@iban", cn,tn);
            command.Parameters.AddWithValue("@montantAAjouter", montantAAjouter);
            command.Parameters.AddWithValue("@iban", iban);
            ExecuterRequeteEtVerifierImpact(command);
            Log("Compte crédité par "+_managerName);
        }

        private void DebiterDe(int montantARetirer, string iban, SqlConnection cn, SqlTransaction tn)
        {
            var command = new SqlCommand("UPDATE [CompteEnBanque] SET [Solde]=[Solde]-@montantARetirer WHERE [IBAN]=@iban", cn, tn);
            command.Parameters.AddWithValue("@montantARetirer", montantARetirer);
            command.Parameters.AddWithValue("@iban", iban);
            ExecuterRequeteEtVerifierImpact(command);
            Log("Compte débité par "+_managerName);
        }

        private void Log(string message)
        {
            if(_logger!=null)
                _logger.Log(message);
        }

        private static void ExecuterRequeteEtVerifierImpact(SqlCommand command)
        {
            var nombreDeComptesEnBanqueAffectes = command.ExecuteNonQuery();
            if (nombreDeComptesEnBanqueAffectes != 1)
                throw new BankAccountNotFoundException();
        }

        public static SqlConnection GetDatabaseConnection()
        {
            throw new Exception("N'oubliez pas d'insérer votre connection string :)");
            string connectionString=@"*********";
            return new SqlConnection(connectionString);
        }

        public double ObtenirSolde(string iban)
        {
            using (SqlConnection connection = GetDatabaseConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT [Solde] FROM [CompteEnBanque] WHERE [IBAN]=@iban", connection);
                command.Parameters.AddWithValue("@iban", iban);
                var solde = (decimal)command.ExecuteScalar();
                connection.Close();
                return Convert.ToDouble(solde);
            }
        }
    }
}
