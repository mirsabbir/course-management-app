﻿using Authorization.Application.DTOs;
using Authorization.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Application.Interfaces
{
    public interface IUserService
    {
        Task<Guid> SendInvitationAsync(CreateUserDTO createUserDTO);
        Task CompleteRegistrationAsync(string token, string password);
        Task<Invitation> GetValidInvitationAsync(string token);
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserById(Guid userId);
        Task DeleteUser(Guid userId);
    }
}
