using Microsoft.VisualStudio.TestTools.UnitTesting;
using model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace tests
{
    [TestClass]
    public class EFCoreGeographyTests
    {
        private readonly Point _rueDeBruxelles=new Point{
            Latitude = 50.46633f,
            Longitude = 4.86048f
        };

        [TestMethod]
        public void RechercherDansUnRayon()
        {
            int rayonMaximalEnMetres = 5000;
             
            var context=new mdpschsa_labaccconcContext();
            var resultatsAProximite = context.GeoDemo.FromSql("[usp_GetThingsAround] @p0, @p1, @p2",_rueDeBruxelles.Latitude, _rueDeBruxelles.Longitude, rayonMaximalEnMetres);
            Assert.AreEqual(2,resultatsAProximite.Count());
        }

        [TestMethod]
        public void RechercherDansUnRayonPlusReduit()
        {
            int rayonMaximalEnMetres = 1000;
            var context=new mdpschsa_labaccconcContext();
            var resultatsAProximite = context.GeoDemo.FromSql("[usp_GetThingsAround] @p0, @p1, @p2",_rueDeBruxelles.Latitude, _rueDeBruxelles.Longitude, rayonMaximalEnMetres);
            Assert.AreEqual(1,resultatsAProximite.Count());
            Assert.AreEqual(resultatsAProximite.Single().Nom,"IESN");
        }

        [TestMethod]
        public void RechercherDansUnRayonEncorePlusReduit()
        {
            int rayonMaximalEnMetres = 5;
            var context=new mdpschsa_labaccconcContext();
            var resultatsAProximite = context.GeoDemo.FromSql("[usp_GetThingsAround] @p0, @p1, @p2",_rueDeBruxelles.Latitude, _rueDeBruxelles.Longitude, rayonMaximalEnMetres);
            Assert.AreEqual(0,resultatsAProximite.Count());
        }
    }
}
