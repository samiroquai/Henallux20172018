using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using ProjectUsingCodeFirst.DTO;

namespace ProjectUsingCodeFirst.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this._userManager=userManager;
            this._signInManager=signInManager;
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
