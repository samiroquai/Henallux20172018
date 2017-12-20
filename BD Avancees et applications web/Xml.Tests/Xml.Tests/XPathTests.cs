using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.XPath;
namespace Xml.Tests
{
    [TestClass]
    [DeploymentItem(@"..\..\..\Samples")]
    public class XPathTests
    {
        public TestContext Context { get; set; }
        [TestMethod]
        public void Position_Permet_De_Recuperer_Un_Noeud_A_Une_Certaine_Position()
        {
            /* Aidez-vous des ressources disponibles dans l'énoncé pour:
            1. Savoir comment créer un document XPath et exécuter des requêtes XPath à son encontre en .NET
            2. Savoir formuler la requête qui permet de récupérer le titre du troisième CD dans le document XML. Son titre est "Greatest Hits"
            
            Règle triple A en unit testing: 
                Arrange: Fait pour vous.
                Act: Définissez la valeur de la variable XPathQuery.
                Assert: à votre disposition. 
            */
            
            var doc = new XPathDocument("CDs.xml");
            XPathNavigator nav = doc.CreateNavigator();
            string xPathQuery = null;
            Assert.IsNotNull(xPathQuery);
            XPathNodeIterator iterator = nav.Select(xPathQuery);
            //L'énumérateur peut bien aller sur le premier résultat
            Assert.IsTrue(iterator.MoveNext());
            //le résultat est bien celui attendu
            Assert.AreEqual("Greatest Hits", iterator.Current.Value);
            //il n'y a bien qu'un résultat correspondant à la requête XPath
            Assert.AreEqual(1, iterator.Count);
        }

        [TestMethod]
        public void RequeteAvecCondition()
        {
            /* A vous de jouer. 
             * Vous devez créer un test qui permettra de vérifier une requête XPath listant tous les artistes qui ont publié en 1997. 
             * Il y en a deux. Vos tests doivent vérifier leur titre et leur nombre.
            */
           
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SeDeplacerDansLesAxes()
        {
            /* A vous de jouer. 
             * La requête XPath que vous allez créer ici devra retourner le frère jumeau précédent le CD dont l'artiste est "Jorn Hoel"
             * (Vous devez donc trouver le CD dont le titre est Tupelo Honey de Van Morrison)
             * 
             * Indice: un axe qui ressemble à sibling. Voir axes sur w3schools
            */
            
            Assert.Inconclusive();
        }
    }
}
