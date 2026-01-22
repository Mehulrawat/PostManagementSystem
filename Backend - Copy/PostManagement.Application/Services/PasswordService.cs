using PostManagement.Application.Contracts.Auth;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Interfaces.Security;
using PostManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostManagement.Application.Services
{
    public class PasswordService
    {
        public const string DefaultResetPassword = "ChangeMyPa$$word1";


        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public PasswordService(IUserRepository userRepository, IPasswordHasher passwordHasher,IUnitOfWork unitOfWork)
        {
        _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }


        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new InvalidOperationException("User not found.");

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.LastActivityAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }

 


    }
}
