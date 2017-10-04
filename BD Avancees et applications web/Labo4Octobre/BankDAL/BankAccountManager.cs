using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDAL
{
    public class BankAccountManager
    {

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
                
                    try
                    {
                        DebiterDe(montantATransferer, ibanOrigine, cn);
                        CrediterDe(montantATransferer, ibanDestination, cn);
                    }
                    finally
                    {
                        cn.Close();
                    }
                
            }
        }

        private void CrediterDe(int montantAAjouter, string iban, SqlConnection cn)
        {
            var command = new SqlCommand("UPDATE [CompteEnBanque] SET [Solde]=[Solde]+@montantAAjouter WHERE [IBAN]=@iban", cn);
            command.Parameters.AddWithValue("@montantAAjouter", montantAAjouter);
            command.Parameters.AddWithValue("@iban", iban);
            ExecuterRequeteEtVerifierImpact(command);
        }

        private void DebiterDe(int montantARetirer, string iban, SqlConnection cn)
        {
            var command = new SqlCommand("UPDATE [CompteEnBanque] SET [Solde]=[Solde]-@montantARetirer WHERE [IBAN]=@iban", cn);
            command.Parameters.AddWithValue("@montantARetirer", montantARetirer);
            command.Parameters.AddWithValue("@iban", iban);
            ExecuterRequeteEtVerifierImpact(command);
        }

        private static void ExecuterRequeteEtVerifierImpact(SqlCommand command)
        {
            var nombreDeComptesEnBanqueAffectes = command.ExecuteNonQuery();
            if (nombreDeComptesEnBanqueAffectes != 1)
                throw new BankAccountNotFoundException();
        }

        public static SqlConnection GetDatabaseConnection()
        {
            throw new Exception("Initialisez connectionString avec une chaîne valable :)");
            string connectionString=null;
            return new SqlConnection(connectionString);
        }

        public double ObtenirSolde(string iban)
        {
            using (SqlConnection connection = GetDatabaseConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT [Solde] FROM [CompteEnBanque] WHERE [IBAN]=@iban", connection);
                command.Parameters.AddWithValue("@iban", iban);
                var solde = (double)command.ExecuteScalar();
                connection.Close();
                return Convert.ToDouble(solde);
            }
        }
    }
}
