using Authorization.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Application.Interfaces
{
    public interface IInvitationRepository
    {
        Task SaveInvitationAsync(Invitation invitation);
        Task<Invitation> GetInvitationByTokenAsync(string token);
        Task DeleteInvitationAsync(string token);
    }
}
