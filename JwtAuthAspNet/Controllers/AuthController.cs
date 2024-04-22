using JwtAuthAspNet.core.DBContext.Dto;
using JwtAuthAspNet.core.OtherObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // route for seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            bool isOwnerRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isUserRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isAdminRolesExists && isUserRolesExists && isOwnerRolesExists)
                return Ok(" Roles Seeding is Already Done !");

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
            return Ok("Role seeding Done successfully !");
        }

        //Route pour ajouter un utilisateur

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var isExistsUser= await _userManager.FindByNameAsync(registerDto.UserName);
            if (isExistsUser != null)
                return BadRequest("Username Aleady Exists !");
            IdentityUser newUser = new IdentityUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult=await _userManager.CreateAsync(newUser, registerDto.Password);
            if(!createUserResult.Succeeded)
            {
                var errorString = "Username creation failed";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += "#" + error.Description;
                }
                return BadRequest(errorString);
            }
            await  _userManager.AddToRoleAsync(newUser,StaticUserRoles.USER);
            return Ok("User created successfully");

        }


        //Route pour le Login 
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if(user is null)
                return Unauthorized("Invalid credentiels");

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if(! isPasswordCorrect)
                return Unauthorized("Invalid credentiels");

            var userRoles=  await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim("JWT",Guid.NewGuid().ToString())
            };

            foreach (var item in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, item));
            }

            var token = GenerateNewJsonWebToken(authClaims); ;
            return Ok(token);
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
               issuer: _configuration["JWT:ValidIssuer"],
               audience: _configuration["JWT:ValidAudience"],
               expires: DateTime.Now.AddHours(1),
               claims: claims,
               signingCredentials: new SigningCredentials(authSecret,SecurityAlgorithms.HmacSha256)
                );
            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;

        }


    }
}
