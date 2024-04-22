using System.ComponentModel.DataAnnotations;

namespace JwtAuthAspNet.core.DBContext.Dto
{
    public class updatePermissionDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

    }
}
