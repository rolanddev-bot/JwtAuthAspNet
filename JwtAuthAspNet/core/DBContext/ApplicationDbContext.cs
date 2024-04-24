namespace JwtAuthAspNet.core.DBContext;

using JwtAuthAspNet.core.DBContext.entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

}


