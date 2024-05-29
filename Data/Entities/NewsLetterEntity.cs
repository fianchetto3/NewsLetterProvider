using System.ComponentModel.DataAnnotations;

namespace SiliconBlazorFrontEnd.Data;

public class NewsLetterEntity
{
    [Key]
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsSubscribed { get; set; }
}
