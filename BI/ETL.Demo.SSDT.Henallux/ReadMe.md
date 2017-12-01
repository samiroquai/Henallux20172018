# Projet démo ETL

Ce projet de démo est une solution possible au problème d'ETL posé lors des laboratoires. Vous pouvez l'ouvrir avec les SQL Server Data Tools incluant SSIS 11.0.2100.60 (Shell VS2010), version disponible sur les machines de laboratoire de l'IESN. 

Pour pavenir à faire fonctionner ce package SSIS, vous devrez modifier les connection managers pour:
- faire pointer ces derniers vers vos bases de données (éditez les propriétés de la connexion)
- faire pointer le fichier plat des dates vers un fichier existant sur votre disque (fichier disponible dans cette arborescence, cherchez ListeDates.csv).


