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
        Task SendInvitationAsync(string email);
        Task CompleteRegistrationAsync(string token, string password);
        Task<Invitation> GetValidInvitationAsync(string token);
    }
}
