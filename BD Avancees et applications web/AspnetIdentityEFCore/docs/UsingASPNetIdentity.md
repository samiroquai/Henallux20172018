# Utiliser ASP.NET Identity Core

Vous devez au préalable avoir mis en place ASP.NET Identity Core.

## Identification des utilisateurs
Cette section est subdivisée:
- Pouvoir créer un compte utilisateur
- Pouvoir identifier un utilisateur à l'aide d'un token (son compte existe déjà)

### Création d'un compte utilisateur

Vous allez créer un nouveau controller dans votre Web API. Ce controller sera chargé de la gestion des comptes utilisateurs. Nommez le AccountController. La méthode Post permet la création d'un compte utilisateur. Une dépendance vers UserManager<ApplicationUser> est nécessaire. Celle-ci est injectée par le runtime lorsque le constructeur est créé. 

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectUsingDbFirst.DTO;

namespace ProjectUsingDbFirst.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            this._userManager=userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]NewUserDTO dto)
        {
            
                var newUser=new ApplicationUser{
                        UserName=dto.UserName,
                        Email = dto.Email
                        
                };
                IdentityResult result = await _userManager.CreateAsync(newUser,dto.Password);
                // TODO: retourner un Created à la place du Ok;
                return (result.Succeeded)?Ok():(IActionResult)BadRequest();
        }
    }
}

```

Notez le paramètre de la méthode Post, qui est extrait depuis le corps de la requête HTTP et sérialisé vers un objet du type NewUserDTO. Conseil: rajoutez des annotations sur ce DTO et utilisez-les à l'aide de ModelState.IsValid.

```csharp
namespace ProjectUsingDbFirst.DTO
{
    public class NewUserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Email { get; set; }       

    }
}
```

Compilez et lancez votre API. Testez votre AccountController (à l'aide de votre debugger HTTP préféré) pour la création d'un compte utilisateur en réalisant un Post sur ce dernier. Mettez des breakpoints afin d'être certain que vous y arrivez bien et que vos paramètres sont bien communiqués. Si votre requête HTTP échoue, c'est peut-être que vous n'avez pas respecté certaines contraintes (ex: complexité minimale du mot de passe). Placez un breakpoint dans votre controller avant le return et examinez la valeur de __result__ pour en apprendre davantage. Dans la même optique, il serait intéressant de retourner la raison de l'erreur à l'utilisateur qui tente de créer un compte. Ceci est laissé pour exercice.

```http
POST /api/Account/ HTTP/1.1
Host: localhost:5000
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: de2a281e-580e-094f-0a20-5fce4ae897c5

{
"UserName":"MON_USER",
"Password":"test123TEST*",
"Email":"MON_E_MAIL@henallux.be"
}
```

Les autres méthodes de gestion des comptes (reset password, blocage, suppression) sont laissées pour exercice. 



Voir aussi: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?tabs=visual-studio%2Caspnetcore2x 

### Identifier un utilisateur

Une fois la mise en place de la création d'un compte réalisée (et testée), vous pouvez passer à la mise en place de l'identification des utilisateurs. Plusieurs options existent (cookies, tokens...). Vous allez utiliser l'identification par Token JWT.

Le tutoriel suivant vous montrera comment procéder. Il ne faut pas le suivre "bêtement", il faut l'adapter pour qu'il s'intègre à ASP.NET Identity. La solution démo qui vous est fournie dans le repository vous propose une solution adaptée. 

https://goblincoding.com/2016/07/03/issuing-and-authenticating-jwt-tokens-in-asp-net-core-webapi-part-i/ 

La plupart du tutoriel de référence est repris ci-dessous avec quelques adaptations. 

Vous avez déjà une classe ApplicationUser, vous pouvez directement vous rendre à l'étape 3 (création de JwtIssuerOptions). 

Créez une nouvelle classe nommée JwtIssuerOptions

```csharp
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace WebApiJwtAuthDemo.Options
{
  public class JwtIssuerOptions
  {
    /// <summary>
    /// "iss" (Issuer) Claim
    /// </summary>
    /// <remarks>The "iss" (issuer) claim identifies the principal that issued the
    ///   JWT.  The processing of this claim is generally application specific.
    ///   The "iss" value is a case-sensitive string containing a StringOrURI
    ///   value.  Use of this claim is OPTIONAL.</remarks>
    public string Issuer { get; set; }

    /// <summary>
    /// "sub" (Subject) Claim
    /// </summary>
    /// <remarks> The "sub" (subject) claim identifies the principal that is the
    ///   subject of the JWT.  The claims in a JWT are normally statements
    ///   about the subject.  The subject value MUST either be scoped to be
    ///   locally unique in the context of the issuer or be globally unique.
    ///   The processing of this claim is generally application specific.  The
    ///   "sub" value is a case-sensitive string containing a StringOrURI
    ///   value.  Use of this claim is OPTIONAL.</remarks>
    public string Subject { get; set; }

    /// <summary>
    /// "aud" (Audience) Claim
    /// </summary>
    /// <remarks>The "aud" (audience) claim identifies the recipients that the JWT is
    ///   intended for.  Each principal intended to process the JWT MUST
    ///   identify itself with a value in the audience claim.  If the principal
    ///   processing the claim does not identify itself with a value in the
    ///   "aud" claim when this claim is present, then the JWT MUST be
    ///   rejected.  In the general case, the "aud" value is an array of case-
    ///   sensitive strings, each containing a StringOrURI value.  In the
    ///   special case when the JWT has one audience, the "aud" value MAY be a
    ///   single case-sensitive string containing a StringOrURI value.  The
    ///   interpretation of audience values is generally application specific.
    ///   Use of this claim is OPTIONAL.</remarks>
    public string Audience { get; set; }

    /// <summary>
    /// "nbf" (Not Before) Claim (default is UTC NOW)
    /// </summary>
    /// <remarks>The "nbf" (not before) claim identifies the time before which the JWT
    ///   MUST NOT be accepted for processing.  The processing of the "nbf"
    ///   claim requires that the current date/time MUST be after or equal to
    ///   the not-before date/time listed in the "nbf" claim.  Implementers MAY
    ///   provide for some small leeway, usually no more than a few minutes, to
    ///   account for clock skew.  Its value MUST be a number containing a
    ///   NumericDate value.  Use of this claim is OPTIONAL.</remarks>
    public DateTime NotBefore => DateTime.UtcNow;

    /// <summary>
    /// "iat" (Issued At) Claim (default is UTC NOW)
    /// </summary>
    /// <remarks>The "iat" (issued at) claim identifies the time at which the JWT was
    ///   issued.  This claim can be used to determine the age of the JWT.  Its
    ///   value MUST be a number containing a NumericDate value.  Use of this
    ///   claim is OPTIONAL.</remarks>
    public DateTime IssuedAt => DateTime.UtcNow;

    /// <summary>
    /// Set the timespan the token will be valid for (default is 5 min/300 seconds)
    /// </summary>
    public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// "exp" (Expiration Time) Claim (returns IssuedAt + ValidFor)
    /// </summary>
    /// <remarks>The "exp" (expiration time) claim identifies the expiration time on
    ///   or after which the JWT MUST NOT be accepted for processing.  The
    ///   processing of the "exp" claim requires that the current date/time
    ///   MUST be before the expiration date/time listed in the "exp" claim.
    ///   Implementers MAY provide for some small leeway, usually no more than
    ///   a few minutes, to account for clock skew.  Its value MUST be a number
    ///   containing a NumericDate value.  Use of this claim is OPTIONAL.</remarks>
    public DateTime Expiration => IssuedAt.Add(ValidFor);

    /// <summary>
    /// "jti" (JWT ID) Claim (default ID is a GUID)
    /// </summary>
    /// <remarks>The "jti" (JWT ID) claim provides a unique identifier for the JWT.
    ///   The identifier value MUST be assigned in a manner that ensures that
    ///   there is a negligible probability that the same value will be
    ///   accidentally assigned to a different data object; if the application
    ///   uses multiple issuers, collisions MUST be prevented among values
    ///   produced by different issuers as well.  The "jti" claim can be used
    ///   to prevent the JWT from being replayed.  The "jti" value is a case-
    ///   sensitive string.  Use of this claim is OPTIONAL.</remarks>
    public Func<Task<string>> JtiGenerator =>
      () => Task.FromResult(Guid.NewGuid().ToString());

    /// <summary>
    /// The signing key to use when generating tokens.
    /// </summary>
    public SigningCredentials SigningCredentials { get; set; }
  }
}
```
Ajoutez dans le fichier appsettings.json (normalement déjà existant dans votre projet web api) le contenu suivant:
```json
{
  "JwtIssuerOptions": {
    "Issuer": "SuperAwesomeTokenServer",
    "Audience": "http://localhost:5000/"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
````
**Attention: adaptez le paramètre Audience en fonction de la manière dont votre web api tourne (sur Azure par exemple, la valeur sera différente, le numéro de port peut changer...).** 

Rendez-vous dans la classe Startup.cs et modifiez la méthode ConfigureServices()

```csharp
public void ConfigureServices(IServiceCollection services)
{
  // Add framework services.
  services.AddOptions();

  // ..........

  // Get options from app settings
  var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

  // Configure JwtIssuerOptions
  services.Configure<JwtIssuerOptions>(options =>
  {
    options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
    options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
    options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
  });
  // N'oubliez pas le reste de la méthode, qui n'a rien à voir avec ce tuto et qui existe peut-être déjà dans votre code.
}
```
Dans la même classe, rajoutez les deux constantes suivantes. Utilisez une clé secrète qui vous est propre et gardez la... secrète :). A cette fin, l'idéal serait de la placer dans la configuration et de ne pas stocker cette configuration dans le système de gestion de sources. Mais par souci de simplification elle est hardcodée. 

```csharp
private const string SecretKey = "needtogetthisfromenvironment";
private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
```

Créez un nouvel API controller nommé JwtController qui contient le code suivant (attention toujours aux namespaces quand vous collez du code). Ce controller vous permettra de récupérer un token sur base d'un couple username/password.

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using WebApiJwtAuthDemo.Options;
using ProjectUsingDbFirst;
using ProjectUsingDbFirst.DTO;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

//Credits : https://goblincoding.com/2016/07/03/issuing-and-authenticating-jwt-tokens-in-asp-net-core-webapi-part-i/
//Adapté pour récupérer les claims depuis ASP.NET Identity
namespace WebApiJwtAuthDemo.Controllers
{
  [Route("api/[controller]")]
  public class JwtController : Controller
  {
    private readonly JwtIssuerOptions _jwtOptions;
    private readonly ILogger _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public JwtController(IOptions<JwtIssuerOptions> jwtOptions, ILoggerFactory loggerFactory, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _jwtOptions = jwtOptions.Value;
      ThrowIfInvalidOptions(_jwtOptions);
      _logger = loggerFactory.CreateLogger<JwtController>();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromBody] LoginDTO loginInfo)
    {
        var user=await _userManager.FindByNameAsync(loginInfo.UserName);
        if(user==null)
        {
            return Unauthorized();
        }
        Microsoft.AspNetCore.Identity.SignInResult result = await  _signInManager.PasswordSignInAsync(user,loginInfo.Password,true,false);
        if(!result.Succeeded)
        {
            _logger.LogInformation($"Invalid username ({loginInfo.UserName}) or password ({loginInfo.Password})");
            return BadRequest("Invalid credentials");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                    ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), 
                    ClaimValueTypes.Integer64),
        };
        IEnumerable<string> roles=await _userManager.GetRolesAsync(user);
        IEnumerable<Claim> allClaimsWithRoles = roles.Select(roleName=>new Claim("Role",roleName))
        .Union(claims);

        // Create the JWT security token and encode it.
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: allClaimsWithRoles,
            notBefore: _jwtOptions.NotBefore,
            expires: _jwtOptions.Expiration,
            signingCredentials: _jwtOptions.SigningCredentials);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        // Serialize and return the response
        var response = new
        {
            access_token = encodedJwt,
            expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
        };
        return Ok(response);
    }

    private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
    {
      if (options == null) throw new ArgumentNullException(nameof(options));

      if (options.ValidFor <= TimeSpan.Zero)
      {
        throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
      }

      if (options.SigningCredentials == null)
      {
        throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
      }

      if (options.JtiGenerator == null)
      {
        throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
      }
    }

    /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
    private static long ToUnixEpochDate(DateTime date)
      => (long)Math.Round((date.ToUniversalTime() - 
                           new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                          .TotalSeconds);
  }
}
```
Compilez et lancez votre web api. Testez ensuite à l'aide de votre debugger HTTP préféré la méthode POST de votre controller JWT. Le résultat devrait ressembler à celui ci-dessous. 

![Images/RecuperationTokenFonctionnelle.png](Images/RecuperationImageFonctionnelle.png)


Ce token est un token JWT. Il contient des informations relatives à l'utilisateur est peut être décodé. Exemple ci-dessous. Une signature (créée à l'aide de la SecretKey déclarée plus tôt dans ce tutoriel) permet au serveur de le valider lorsqu'une requête HTTP est reçue. Ci-dessous, le contenu du token retourné par votre Controller.

![Images/DecodageDuJWT.JPG](Images/DecodageDuJWT.JPG)

Le token contient entre autres informations:
- Le User name (samiroquai)
- Les rôles de l'utilisateur (ici Admin et Modérateur)

Pour confirmation, vous pouvez regarder dans la DB les rôles conférés aux utilisateurs.

![Images/ContenuDeLaDb.JPG](Images/ContenuDeLaDb.JPG)

Maintenant que vous savez ce que le token contient, si vous vous posez des questions quant à sa constitution, si vous souhaitez y rajouter des informations, rendez-vous dans la méthode Post de votre JwtController. C'est là que les claims sont créées, puis ajoutées au JWT. 

Vous pouvez également décoder le token côté client (app Android, app UWP) pour connaître les rôles conférés à un utilisateur et donc sécuriser les fonctionnalités en fonction des permissions octroyées. Le site https://jwt.io/ vous aidera à identifier quelques librairies.

### Autoriser les utilisateurs
Pour autoriser des utilisateurs à exécuter une requête, il faut avant tout pouvoir identifier l'utilisateur. Votre récupération de token doit donc fonctionner avant de passer à cette étape.

Vous devrez présenter à votre Web API lors chaque requête HTTP le token que vous aurez récupéré au préalable. Le token est passé dans les headers de vos requêtes HTTP. Le nom du header est "Authorization" et sa valeur est "Bearer XXXXX" (XXXXX est à remplacer par le token que vous avez récupéré préalablement).

```http
GET /api/Values HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzYW11ZWwuc2Nob2x0ZXMiLCJqdGkiOiIwODUzOTAwNi1jMWI5LTRjNTctYjFiNS0zZGM2ZTZhNGRjMTEiLCJpYXQiOjE1MTA4NDMzMDMsIm5iZiI6MTUxMDg0MzMwMywiZXhwIjoxNTEwODQzNjAzLCJpc3MiOiJTdXBlckF3ZXNvbWVUb2tlblNlcnZlciIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC8ifQ.sqJN1c5gosQbszrrC0DWWo0KgVdwyfeI4O3JzQkx2jg
```

Vous devez également décorer les controllers que vous souhaitez protéger (pas d'accès anonyme) avec l'attribut Authorize. Voyez la ligne 12 du controller ci-dessous. On spécifie que le controller doit être protégé. On spécifie également que le mécanisme d'identification à utiliser est celui des JWT. Pour que ça puisse fonctionner, ajoutez une référence au package Microsoft.AspNetCore.Authentication.JwtBearer. 

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProjectUsingDbFirst.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ValuesController : BaseController
    {
        public ValuesController(UserManager<ApplicationUser> userManager)
            :base(userManager)
        {

        }
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            ApplicationUser monUtilisateur = await GetCurrentUserAsync();
            Console.WriteLine("L'utilisateur "+ monUtilisateur.UserName+" vient d'appeler la méthode Get sur le Values Controller!!!");
            return new string[] { "value1", "value2" };
        }
    }
}
```

Notez également que ce controller hérite d''un BaseController. Dans ce controller de base, j'ai défini une méthode utilitaire qui renvoie une instance de ApplicationUser. Vous pourriez faire hériter tous les controllers de votre API de ce controller de base afin de profiter de cette facilité.

```csharp
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProjectUsingDbFirst.Controllers
{
    public abstract class BaseController : Controller
    {
        private UserManager<ApplicationUser> _uMgr;
        public BaseController(UserManager<ApplicationUser> uMgr)
        {
            _uMgr=uMgr;
        }
       protected async Task<ApplicationUser> GetCurrentUserAsync()
       {
            if(this.HttpContext.User==null)
                throw new Exception("L'utilisateur n'est pas identifié");
            Claim userNameClaim=this.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type==ClaimTypes.NameIdentifier);
            if(userNameClaim==null)
                throw new Exception("Le token JWT semble ne pas avoir été interprété correctement");
            return await _uMgr.FindByNameAsync(userNameClaim.Value);
       }
    }
}
```
Pour que ce code fonctionne, vous devez configurer votre API afin qu'elle intègre le middleware JWT nécessaire. Rendez-vous dans la classe Startup, ligne 7 et ajoutez la ligne app.UseAuthentication() **__avant l'appel à UseMvc()__**

```csharp
 public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
```

Compilez et lancez votre API. Récupérez un token frais (voir précédemment). Tentez ensuite d'appeler une méthode protégée d'un de vos controllers en passant ce token dans les headers de la requête HTTP (voir précédemment). Testez en modifiant légèrement le token (supprimer une lettre par exemple) également afin de vous assurer que ça ne fonctionne pas.

Votre token aura une durée de vie de 300 secondes par défaut. Laissez cette durée de vie courte dans un premier temps pour tester l'identification et l'autorisation. Ensuite, afin d'être plus relax pour développer votre application, augmentez la durée de vie du token. Cherchez la propriété ValidFor sur la classe JwtIssuerOptions.

Il ne vous est pas demandé de gérer les cas d'expiration du token dans vos applications pour le projet Smart City. 

## Pour finir

Vous êtes perdu? Examinez le code du projet web api ProjectUsingDbFirst. Ajoutez les mêmes références (voir fichier csproj) à votre projet. Comparez votre fichier __startup.cs__ avec celui du projet de référence. Regardez dans la classe __JwtController__ comment elle fonctionne. Regardez dans le __ValuesController__ comment protéger un controller à l'aide de l'attribut __Authorize__. Si certaines méthodes d'un controller doivent être publiques, décorez-les avec __AllowAnonymous__.



## Ressources
https://blogs.msdn.microsoft.com/webdev/2017/04/06/jwt-validation-and-authorization-in-asp-net-core/

https://samueleresca.net/2016/12/developing-token-authentication-using-asp-net-core/

https://goblincoding.com/2016/07/03/issuing-and-authenticating-jwt-tokens-in-asp-net-core-webapi-part-i/

https://jwt.io/

https://github.com/aspnet/Security/issues/1310

https://github.com/aspnet/Security/issues/1338


