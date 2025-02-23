using Authorization.Application.Interfaces;
using Authorization.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Infrastructure.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly ApplicationDbContext _context;

        public InvitationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveInvitationAsync(Invitation invitation)
        {
            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();
        }

        public async Task<Invitation> GetInvitationByTokenAsync(string token)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Invitations
                .FirstOrDefaultAsync(i => i.Token == token);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task DeleteInvitationAsync(string token)
        {
            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invitation != null)
            {
                _context.Invitations.Remove(invitation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Invitation> GetInvitationByEmailAsync(string email)
        {
            return await _context.Invitations
                                   .FirstOrDefaultAsync(i => i.Email == email);
        }

    }
}
