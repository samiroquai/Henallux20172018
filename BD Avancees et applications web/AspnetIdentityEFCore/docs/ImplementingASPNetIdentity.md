# Intégrer ASP.NET Identity à son projet
La procédure à suivre diffère selon l'approche utilisée pour votre DAL:
- DAL EF Core DB First
- DAL EF Core Code First
- DAL ADO.NET

Il faut avant tout noter que l'approche Code First est celle avec laquelle l'intégration sera la plus facile. L'approche DB First n'est pas impossible mais demande plus de travail de mise en place.

## DAL EF Core DB First
Voir la [procédure fournie](AspNetIdentitydbFirst.md). Vous pouvez également vous référer au projet ci-joint nommé ProjectUsingDbFirst.

## DAL EF Core Code First
Voir la [procédure fournie](AspNetIdentityCodeFirst.md) Voir aussi le projet fourni ProjectUsingCodeFirst.

## DAL ADO.NET
Pour une DAL ADO.NET, il vous est conseillé d'ajouter ASP.NET Identity comme une DAL séparée, comme si vous n'aviez pas encore de DAL. Consultez l'article suivant: http://www.binaryintellect.net/articles/b957238b-e2dd-4401-bfd7-f0b8d984786d.aspx à partir de l'étape 2 (puisque vous avez déjà un projet Web API existant). 

