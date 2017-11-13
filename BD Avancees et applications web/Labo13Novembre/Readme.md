# démo EF Core Code First
## Structure de la solution
3 projets: Model, webapi, tests

Le projet Model contient le modèle OO que vous avez défini ainsi qu'une classe héritant de DbContext (EF Core), réalisant le mapping avec une DB Relationnelle. 
Le projet Web API exploite la librairie Model en exposant une API REST (ex: CustomerController qui permet de manipuler les clients de la DB).
Le projet tests contient du code illustrant quelques manipulations de base d'un contexte EF Core. Il contient également du code permettant de créer la Db sur base du modèle OO défini (voir méthode TestInitialize).

