using CourseManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<string> CreateUserAsync(CreateUserDTO createUserDTO);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(Guid id);
        Task DeleteUserByIdAsync(Guid id);
    }
}
