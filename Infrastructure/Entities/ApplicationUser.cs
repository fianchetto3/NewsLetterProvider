using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;


public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Biography { get; set; }

    public string ProfileImage { get; set; } = "avatar.svg";

    public int? AddressId { get; set; }

    public AddressEntity? Address { get; set; }
}


public class AddressEntity
{
    [Key]
    public int Id { get; set; }

    public string StreetName { get; set; } = null!;

    public string City { get; set; } = null!;

    public string PostalCode { get; set; } = null!;


}