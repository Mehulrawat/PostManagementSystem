using System.ComponentModel.DataAnnotations;

namespace PostManagement.Application.Contracts.Auth;

public class RegisterUserRequest
{
    [Required]
    [RegularExpression(
      @"^(?! )[A-Za-z0-9._ ]+$",
      ErrorMessage = "Username can contain letters, numbers, dots, underscores, and spaces, but cannot start with a space"
  )]
    public string Username { get; set; } = default!;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^(?!\.)(?!.*\.\.)[A-Za-z0-9.]+(?<!\.)@infinite\.com$",
        ErrorMessage = "Email must use only letters, numbers, dots, or underscores before @ and must be an Infinite email"
    )]
    public string Email { get; set; } = default!;
    [Required]
    public string Password { get; set; } = default!;
}
