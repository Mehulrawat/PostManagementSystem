using PostManagement.Domain.Entities;

namespace PostManagement.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IEnumerable<string> roles);
}
