using PostManagement.Application.Contracts.Users;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Interfaces.Security;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;

namespace PostManagement.Application.Services;

public class UserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostRepository _postRepository;

    public UserManagementService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IPostRepository postRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _postRepository = postRepository;

    }

    public async Task<List<UserSummaryResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var results = new List<UserSummaryResponse>();

        foreach (var user in users)
        {
            var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id);
            results.Add(MapToSummary(user, roles));
        }

        return results;
    }

    public async Task SuspendUserAsync(Guid targetUserId, Guid actingUserId)
    {
        var actingRoles = await _userRoleRepository.GetRoleNamesForUserAsync(actingUserId);
        var (target, targetRoles) = await LoadUserWithRolesAsync(targetUserId);

        EnsureCanManage(actingRoles, targetRoles);

        target.IsActive = false;
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ReinstateUserAsync(Guid targetUserId, Guid actingUserId)
    {
        var actingRoles = await _userRoleRepository.GetRoleNamesForUserAsync(actingUserId);
        var (target, targetRoles) = await LoadUserWithRolesAsync(targetUserId);

        EnsureCanManage(actingRoles, targetRoles);

        target.IsActive = true;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid targetUserId, Guid actingUserId)
    {
        var actingRoles = await _userRoleRepository.GetRoleNamesForUserAsync(actingUserId);
        var (target, targetRoles) = await LoadUserWithRolesAsync(targetUserId);

        EnsureCanManage(actingRoles, targetRoles);

    
        var posts = await _postRepository.GetByUserAsync(targetUserId);

        foreach (var post in posts)
        {
            await _postRepository.DeleteAsync(post);
        }

       
        await _userRepository.DeleteAsync(target);

        await _unitOfWork.SaveChangesAsync();
    }


    public async Task PromoteToAdminAsync(Guid targetUserId, Guid actingUserId)
    {
        await EnsureSuperAdminAsync(actingUserId);
        var (target, targetRoles) = await LoadUserWithRolesAsync(targetUserId);

        if (HasRole(targetRoles, UserRoleName.Admin))
            return;

        var adminRole = await _roleRepository.GetByNameAsync(UserRoleName.Admin)
                        ?? throw new InvalidOperationException("Admin role not found.");

        await _userRoleRepository.AddAsync(new UserRole
        {
            UserId = target.Id,
            RoleId = adminRole.Id
        });
      

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DemoteAdminAsync(Guid targetUserId, Guid actingUserId)
    {
        await EnsureSuperAdminAsync(actingUserId);
        var (target, targetRoles) = await LoadUserWithRolesAsync(targetUserId);

        if (!HasRole(targetRoles, UserRoleName.Admin))
            return;

        await _userRoleRepository.RemoveRoleAsync(target.Id, UserRoleName.Admin);

     
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ResetPasswordAsync(Guid targetUserId, Guid actingUserId, string defaultPassword)
    {
        var actingRoles = await _userRoleRepository.GetRoleNamesForUserAsync(actingUserId);
        var (target, targetRoles) = await LoadUserWithRolesAsync(targetUserId);

        EnsureCanManage(actingRoles, targetRoles);

        target.PasswordHash = _passwordHasher.Hash(defaultPassword);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureSuperAdminAsync(Guid actingUserId)
    {
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(actingUserId);
        if (!HasRole(roles, UserRoleName.SuperAdmin))
        {
            throw new UnauthorizedAccessException("Only the superadmin can perform this action.");
        }
    }

    private static bool HasRole(IEnumerable<string> roles, UserRoleName role)
    {
        return roles.Any(r => string.Equals(r, role.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    private void EnsureCanManage(IEnumerable<string> actingRoles, IEnumerable<string> targetRoles)
    {
        var actingRoleList = actingRoles.ToList();
        var targetRoleList = targetRoles.ToList();

        var actingIsSuperAdmin = HasRole(actingRoleList, UserRoleName.SuperAdmin);
        var actingIsAdmin = HasRole(actingRoleList, UserRoleName.Admin);

        if (!actingIsAdmin && !actingIsSuperAdmin)
            throw new UnauthorizedAccessException("Only admins can manage users.");

        if (HasRole(targetRoleList, UserRoleName.SuperAdmin))
            throw new InvalidOperationException("Cannot manage the superadmin account.");

        if (HasRole(targetRoleList, UserRoleName.Admin) && !actingIsSuperAdmin)
            throw new UnauthorizedAccessException("Only the superadmin can manage admin accounts.");
    }

    private async Task<(User user, List<string> roles)> LoadUserWithRolesAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new InvalidOperationException("User not found.");
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(userId);
        return (user, roles);
    }

    private static UserSummaryResponse MapToSummary(User user, IEnumerable<string> roles)
    {
        return new UserSummaryResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            IsAutoDeactivated = user.IsAutoDeactivated,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToArray()
        };
    }
}
