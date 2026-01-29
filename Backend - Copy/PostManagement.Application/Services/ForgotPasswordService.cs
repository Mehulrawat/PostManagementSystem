using PostManagement.Application.Contracts.Password;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Notification;
using PostManagement.Domain.Entities;
using PostManagement.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PostManagement.Application.Services
{
    public class ForgotPasswordService
    {
        private readonly AuthService _authService;
        private readonly UserManagementService _userManagementService;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;


        public ForgotPasswordService(AuthService authService, IUserRepository userRepository,
            UserManagementService userManagementService, IUserRoleRepository userRoleRepository,
            INotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _userManagementService = userManagementService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

       
    }
}
