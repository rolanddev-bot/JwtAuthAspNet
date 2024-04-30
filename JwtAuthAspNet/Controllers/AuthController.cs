using JwtAuthAspNet.core.Dto;
using JwtAuthAspNet.core.Entities;
using JwtAuthAspNet.core.Interfaces;
using JwtAuthAspNet.core.OtherObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthAspNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
      private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // route for seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
          var seedRoles =await _authService.SeedRolesAsync();
            return Ok(seedRoles);
        }

        //Route pour ajouter un utilisateur

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerReseult = await _authService.RegisterAsync(registerDto);
                if(registerReseult.IsSucceed)
                return Ok(registerReseult);

          return BadRequest(registerReseult);

        }


        //Route pour le Login 
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult= await _authService.LoginAsync(loginDto);
            if(loginResult.IsSucceed)
                return Ok(loginResult);

           return BadRequest(loginResult); 
        }

        //fonction pour generer le token
      


        ////Route pour le Login 
        //[HttpGet]
        //[Route("list-user")]
        //[Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.OWNER)]

        //public async Task<IActionResult> ListUser()
        //{
        //    var users = await _userManager.Users.ToListAsync(); // Récupère la liste des utilisateurs

        //    // Convertir la liste des utilisateurs en JSON
        //  //  var json = JsonConvert.SerializeObject(users);

        //    return Ok(users);
        //    //return Content(json, "application/json");
        //}


        //create user has role OWNER
        [HttpPost]
        [Route("make-owner")]
        public async Task<IActionResult> makeOwner([FromBody] UpdatePermissionDto updatePermissionDto)
        {
           var makeOwner = await _authService.MakeOwnerAsync(updatePermissionDto);
            if(makeOwner.IsSucceed)
                return Ok(makeOwner);

        return BadRequest(makeOwner);   
            

        }

        //create user has role ADMIN

        [HttpPost]
        [Route("make-admin")]
        public async Task<IActionResult> makeAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {

            var makeAdmin = await _authService.MakeAdminAsync(updatePermissionDto);
            if(makeAdmin.IsSucceed)
                return Ok(makeAdmin);

            return BadRequest(makeAdmin);

        }
    }
}
