using PostManagement.Application.Contracts.Auth;
using PostManagement.Application.Contracts.Password;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Notification;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Interfaces.Security;
using PostManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
        private readonly INotificationService _notificationService;

        public PasswordService(IUserRepository userRepository, IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }


        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new InvalidOperationException("User not found.");

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.UpdatedAt = DateTime.Now;
            user.LastActivityAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {

            var user = await _userRepository.GetByEmailAsync(request.Email);
            //?? throw new InvalidOperationException("User not found.");

            // SECURITY: don’t reveal if email exists


            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var encodedToken = WebUtility.UrlEncode(token);

            user.PasswordResetToken = encodedToken?? throw new NullReferenceException("Token couldn't be stored.");
            user.PasswordResetTokenExpiresAt = DateTime.Now.AddMinutes(30);

            //await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

           
            await _notificationService.SendPasswordResetAsync(user.Email, encodedToken);



        }

        //public async Task ResetPasswordAync(ResetPasswordRequest request)
        //{
            


        //}
    }
}
