using JwtAuthAspNet.core.Dto;
using JwtAuthAspNet.core.Entities;
using JwtAuthAspNet.core.Interfaces;
using JwtAuthAspNet.core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthAspNet.core.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

    

       

        public async Task<AuthServicesResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user is null)
                return new AuthServicesResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid credentiels"
                };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
                return new AuthServicesResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid credentiels"
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim("JWT",Guid.NewGuid().ToString()),
                new Claim("Firstname",user.FirstName),
                new Claim("Lastname",user.LastName)
            };

            foreach (var item in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, item));
            }

            var token = GenerateNewJsonWebToken(authClaims);

            return new AuthServicesResponseDto()
            {
                IsSucceed = false,
                Message = token
            };
        }



       public async Task<AuthServicesResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(updatePermissionDto.UserName);
            if (isExistsUser is null)
                return new AuthServicesResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid UserName"
                };
            await _userManager.AddToRoleAsync(isExistsUser, StaticUserRoles.ADMIN);

            return new AuthServicesResponseDto()
            {
                IsSucceed = true,
                Message = "User is now an OWNER" + isExistsUser
            };
        }



       public async Task<AuthServicesResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(updatePermissionDto.UserName);
            if (isExistsUser is null)
            return new AuthServicesResponseDto()
            {
                IsSucceed = false,
                Message ="Invalid UserName"
            };
            await _userManager.AddToRoleAsync(isExistsUser, StaticUserRoles.OWNER);

            return new AuthServicesResponseDto()
            {
                IsSucceed = true,
                Message = "User is now an OWNER" + isExistsUser
            };
        }

        public async Task<AuthServicesResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (isExistsUser != null)
                return new AuthServicesResponseDto()
                {
                    IsSucceed = false,
                    Message = "Username Aleady Exists !"
                };
            ApplicationUser newUser = new ApplicationUser()
            {
                Email = registerDto.Email,
                LastName = registerDto.LastName,
                FirstName = registerDto.FirstName,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errorString = "Username creation failed";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += "#" + error.Description;
                }
                return new AuthServicesResponseDto()
                {
                    IsSucceed = false,
                    Message = errorString
                };

            }
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);
            return new AuthServicesResponseDto()
            {
                IsSucceed = true,
                Message = "User created successfully"
            };
        }

        public async Task<AuthServicesResponseDto> SeedRolesAsync()
        {
            bool isOwnerRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isUserRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isAdminRolesExists && isUserRolesExists && isOwnerRolesExists)
                return new AuthServicesResponseDto() {
                    IsSucceed = true,
                    Message="Roles Seeding is Already Done"
                };

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

            return new AuthServicesResponseDto()
            {
                IsSucceed = true,
                Message = "Role seeding Done successfully !"
            };
            //return Ok("Role seeding Done successfully !");
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
               issuer: _configuration["JWT:ValidIssuer"],
               audience: _configuration["JWT:ValidAudience"],
               expires: DateTime.Now.AddHours(1),
               claims: claims,
               signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );
            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;

        }
    }
}
