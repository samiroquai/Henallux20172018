using Microsoft.VisualStudio.TestTools.UnitTesting;
using model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace tests
{
    [TestClass]
    public class AdoDotNetGeographyTests
    {
        private readonly Point _rueDeBruxelles=new Point{
            Latitude = 50.46633f,
            Longitude = 4.86048f
        };

        [TestMethod]
        public void RechercherDansUnRayon()
        {
            int rayonMaximalEnMetres = 5000;
            Assert.AreEqual(2, CompterCorrespondancesDansRayon(_rueDeBruxelles, rayonMaximalEnMetres));
        }

        private int CompterCorrespondancesDansRayon(Point center, int rayonMaximalEnMetres)
        {
            int correspondancesDansRayon = -1;
            using(SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand command=GetCommand(center, rayonMaximalEnMetres);
                command.Connection=connection;
                correspondancesDansRayon = (int)command.ExecuteScalar();
                connection.Close();
            }
            return correspondancesDansRayon;
        }

        private SqlConnection GetConnection()
        {
            throw new NotImplementedException("Remplacez dans la connection string ci-dessous par les valeurs connues de vous seul. Laissez le paramètre Initial Catalog intact.");
            return new SqlConnection(@"Data Source=LE_SERVEUR_DE_L_IESN; Initial Catalog=mdpschsa_labaccconc; User Id=VOTRE_USER_ID; Password=VOTRE_MOT_DE_PASSE");
        }

        private SqlCommand GetCommand(Point center, int rayonEnMetres)
        {
            // la première requête est un exemple qui retourne l'identifiant du point d'intérêt, son nom et la distance le séparant du centre. Elle est donnée à titre informatif. 
            // var cmd=new SqlCommand("SELECT Id, Nom, Coordonnees.STDistance(geography::STPointFromText('POINT('+CAST (@lat AS varchar(20))+' '+CAST(@lon AS varchar(20))+')', 4326)) FROM GeoDemo WHERE Coordonnees.STDistance(geography::STPointFromText('POINT('+CAST (@lat AS varchar(20))+' '+CAST(@lon AS varchar(20))+')', 4326)) <= @rayonEnMetres");
            var cmd=new SqlCommand("SELECT COUNT(*) FROM GeoDemo WHERE Coordonnees.STDistance(geography::STPointFromText('POINT('+CAST (@lat AS varchar(20))+' '+CAST(@lon AS varchar(20))+')', 4326)) <= @rayonEnMetres");
            cmd.Parameters.AddWithValue("@rayonEnMetres", rayonEnMetres);
            cmd.Parameters.AddWithValue("@lat", center.Latitude);
            cmd.Parameters.AddWithValue("@lon", center.Longitude);
            return cmd;
        }

        [TestMethod]
        public void RechercherDansUnRayonPlusReduit()
        {
            int rayonMaximalEnMetres = 1000;
            Assert.AreEqual(1, CompterCorrespondancesDansRayon(_rueDeBruxelles,rayonMaximalEnMetres));
        }

        [TestMethod]
        public void RechercherDansUnRayonEncorePlusReduit()
        {
            int rayonMaximalEnMetres = 5;
            Assert.AreEqual(0, CompterCorrespondancesDansRayon(_rueDeBruxelles, rayonMaximalEnMetres));
        }
    }
}
