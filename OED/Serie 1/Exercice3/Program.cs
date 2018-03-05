using System;
using System.Linq;
namespace Exercice3
{
    class Program
    {
        static void Main(string[] args)
        {
            var tableau = new Offre[]
            {
                new Offre(){ Prix=10, Delai=3},
                new Offre(){ Prix=20, Delai=10},
                new Offre(){ Prix=20, Delai=15},
                new Offre(){ Prix=20, Delai=20},
                new Offre(){ Prix=30, Delai=2},
            };
            Display(tableau);
            Offre nouvelleOffre=ObtenirOffre();
            while(nouvelleOffre!=null)
            {
                int indiceInsertion = RechercherIndiceInsertion(tableau, nouvelleOffre);
                Inserer(tableau, indiceInsertion, nouvelleOffre);
                Display(tableau);
                nouvelleOffre=ObtenirOffre();
            }
        }

        private static Offre ObtenirOffre()
        {
            Console.WriteLine("Insérer le prix et le délai, séparés par ; ou Q pour quitter");
            string saisie=Console.ReadLine();
            string[] nouvelleOffre=saisie.Split(';',StringSplitOptions.RemoveEmptyEntries);
            return (saisie=="Q")?null:new Offre(){ Prix=double.Parse(nouvelleOffre[0]), Delai=int.Parse(nouvelleOffre[1]) };
        }

        private static void Inserer(Offre[] offres, int indiceInsertion, Offre offreAInserer)
        {
            if(indiceInsertion==5)
                return;
            for(int indCourrant=4;indCourrant>indiceInsertion;indCourrant--) {
                offres[indCourrant]=offres[indCourrant-1];
            }
            offres[indiceInsertion]=offreAInserer;
        }

        private static int RechercherIndiceInsertion(Offre[] tableau, Offre aInserer)
        {
            int i=0;
            while(
                i<5 
                && 
                (tableau[i].Prix<aInserer.Prix || (tableau[i].Prix == aInserer.Prix && tableau[i].Delai<aInserer.Delai)))
            //while(i<5 && tableau[i].Prix<aInserer.Prix && tableau[i].Delai<aInserer.Delai)
            {
                i++;
            }
            return i;
        }

        private static void Display(Offre[] tableau)
        {
            Console.WriteLine("==================== Etat du tableau ");
            Console.WriteLine("Prix\tDélai");
            for(int i=0;i<5;i++){
                Console.WriteLine(tableau[i].Prix+"\t"+tableau[i].Delai);
            }
        }
    }
}
