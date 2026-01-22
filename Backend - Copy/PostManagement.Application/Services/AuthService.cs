using PostManagement.Application.Contracts.Auth;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Interfaces.Security;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;


namespace PostManagement.Application.Services;


public class AuthService
{
    public const string DefaultResetPassword = "ChangeMyPa$$word1";

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task RegisterAsync(RegisterUserRequest request)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            throw new InvalidOperationException("Username already exists.");

        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var userRole = await _roleRepository.GetByNameAsync(UserRoleName.User)
                       ?? throw new InvalidOperationException("Default role 'User' not found. Make sure roles are seeded.");

        await _userRoleRepository.AddAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = userRole.Id
        });

        await _unitOfWork.SaveChangesAsync();
    }


    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        await EnsureActiveUserAsync(user);

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        user.LastActivityAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return await BuildLoginResponseAsync(user);
    }

    public async Task<LoginResponse> IssueTokenForUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new UnauthorizedAccessException("User not found.");

        await EnsureActiveUserAsync(user);

        user.LastActivityAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return await BuildLoginResponseAsync(user);
    }



    private async Task EnsureActiveUserAsync(User user)
    {
        if (user.LastActivityAt.HasValue)
        {
            var inactiveFor = DateTime.UtcNow - user.LastActivityAt.Value;
            if (inactiveFor.TotalDays >= 365)
            {
                user.IsActive = false;
                user.IsAutoDeactivated = true;
                await _unitOfWork.SaveChangesAsync();
                throw new UnauthorizedAccessException("Your account has been deactivated due to inactivity. Please contact an administrator.");
            }
        }

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is inactive. Please contact an administrator.");
    }

    private async Task<LoginResponse> BuildLoginResponseAsync(User user)
    {
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id);
        if (roles == null || roles.Count == 0)
        {
            roles = new() { "User" };
        }

        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Roles = roles.ToArray()
        };
    }
}
