using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiliconBlazorFrontEnd.Data;


namespace Infrastructure.Contexts
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
      public DbSet<NewsLetterEntity> NewsLetter { get; set; }
    }
}
