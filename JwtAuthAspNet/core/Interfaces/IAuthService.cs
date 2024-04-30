using JwtAuthAspNet.core.Dto;

namespace JwtAuthAspNet.core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServicesResponseDto> SeedRolesAsync();
        Task<AuthServicesResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthServicesResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthServicesResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto);
        Task<AuthServicesResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto);


    }
}
