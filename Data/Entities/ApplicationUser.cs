using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SiliconBlazorFrontEnd.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;


     }

}