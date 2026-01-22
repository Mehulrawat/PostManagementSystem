using PostManagement.Application.Contracts.Users;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;

namespace PostManagement.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        public UserService(IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }
        public async Task<UserSummaryResponse> MeAsync(Guid userId)
        {
           
                

                var User = await _userRepository.GetByIdAsync(userId);
                var roles = await _userRoleRepository.GetRoleNamesForUserAsync(userId);
              

                return new UserSummaryResponse
               {
                    Id=User.Id,
                   Username= User.Username,
                    Email=User.Email,
                    IsActive=User.IsActive,
                    IsAutoDeactivated=User.IsAutoDeactivated,
                    CreatedAt = User.CreatedAt,
                    Roles= roles.ToArray()


                };

}
          
        }
       
   
    }

