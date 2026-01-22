using System.ComponentModel.DataAnnotations;

namespace PostManagement.Application.Contracts.Auth;

public class LoginRequest
{
    [Required]
    [RegularExpression(
     @"^(?! )[A-Za-z0-9._ ]+$",
     ErrorMessage = "Username can contain letters, numbers, dots, underscores, and spaces, but cannot start with a space"
 )]
    public string Username { get; set; } = default!;
    [Required]
    public string Password { get; set; } = default!;
}
