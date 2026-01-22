using System.Security.Cryptography;
using System.Text;
using PostManagement.Application.Interfaces.Security;

namespace PostManagement.Infrastructure.Security;

public class SimplePasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string password, string hash)
    {
        return Hash(password) == hash;
    }
}
