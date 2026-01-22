namespace PostManagement.Application.Contracts.Auth;

public class LoginResponse
{
    public string Token { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string[] Roles { get; set; } = System.Array.Empty<string>();
}
