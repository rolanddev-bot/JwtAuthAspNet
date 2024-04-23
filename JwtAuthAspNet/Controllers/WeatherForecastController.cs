using JwtAuthAspNet.core.OtherObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthAspNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

       

      

        [HttpGet]
        [Route("get")]
        public IActionResult Get()
        {
            return Ok(Summaries);
        }

        [HttpGet]
        [Route("GetUserRole")]
        [Authorize(Roles =StaticUserRoles.USER)]
        public IActionResult GetUserRoles()
        {
            return Ok(Summaries);
        }


        [HttpGet]
        [Route("GetAdminRole")]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public IActionResult GetAdminRoles()
        {
            return Ok(Summaries);
        }

        [HttpGet]
        [Route("GetOwnerRole")]
        [Authorize(Roles = StaticUserRoles.OWNER)]
        public IActionResult GetOwnerRoles()
        {
            return Ok(Summaries);
        }
    }
}
