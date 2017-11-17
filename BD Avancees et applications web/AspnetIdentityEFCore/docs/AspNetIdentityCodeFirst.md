# Utilisation d'ASP.NET Core Identity dans un projet existant utilisant EF Core en mode Code First

## Ajout des références nécessaires
Comparez les références de votre projet et celles du projet fourni. Ajoutez chez vous ce qui semble manquer.

## Modélisation de la classe User
Deux options:
- Si vous avez déjà modélisé une classe User, faites-la hériter de __IdentityUser__. 
- Si vous n'avez pas encore de classe User, crééez-en une (dans le projet fourni, il s'agit de __ApplicationUser__) et faites-la hériter de __IdentityUser__. Allez voir la définition de IdentityUser et remontez la chaine d'héritage pour connaître les propriétés déjà offertes par la librairie ASP.NET Core Identity.

## Relier le DbContext existant à ASP.NET Identity
Ensuite, assurez-vous que votre DbContext (supposé déjà existant) hérite de __Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser>__ (remplacez ApplicationUser par le nom de votre classe User, voir ci-dessus). Dans cette même classe, vous pouvez vous assurer que la DB est bien créée si elle n'existe pas.

```csharp
using System;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class ApplicationDbContext: Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
        :base(options)
        {
            this.Database.EnsureCreated();            
        }
    }
}
```

## Plugger ASP.NET Core Identity dans le pipeline ASP.NET
Rendez-vous dans la classe __Startup.cs__ et assurez-vous d'avoir le code suivant dans la méthode ConfigureServices.

```csharp
public void ConfigureServices(IServiceCollection services)
        {
            string connectionString=@"VOTRE_CONNECTION_STRING => IDEALEMENT A L'EXTERIEUR DES SOURCES";
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //.. Le reste de la méthode, déjà existante
        }
```

## Tester
Lancez votre projet et faites en sorte de passer par le constructeur de ApplicationDbContext afin de forcer la création de votre DB. Allez ensuite dans votre DB et vérifiez que les tables nécessaires ont bien été créées.