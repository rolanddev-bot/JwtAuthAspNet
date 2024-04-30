using System.ComponentModel.DataAnnotations;

namespace JwtAuthAspNet.core.Dto
{
    public class UpdatePermissionDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

    }
}
