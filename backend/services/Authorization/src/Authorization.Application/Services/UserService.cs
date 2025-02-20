using Authorization.Application.Interfaces;
using Authorization.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public UserService(
            IInvitationRepository invitationRepository,
            IEmailService emailService,
            UserManager<User> userManager)
        {
            _invitationRepository = invitationRepository;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task SendInvitationAsync(string email)
        {
            // Generate a unique invitation token
            var token = GenerateInvitationToken(email);

            // Create the invitation
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = email,
                Token = token,
                ExpirationDate = DateTime.UtcNow.AddDays(1) // Token expires in 1 day
            };

            // Save the invitation
            await _invitationRepository.SaveInvitationAsync(invitation);

            // Send the invitation email
            var tokenQueryParam = Uri.EscapeDataString(token);
            var invitationLink = $"https://localhost:7209/account/acceptinvitation?token={tokenQueryParam}";
            await _emailService.SendEmailAsync(email, "Invitation to Join", $"Click here to register: {invitationLink}");
        }

        public async Task CompleteRegistrationAsync(string token, string password)
        {
            // Get the invitation by token
            var invitation = await _invitationRepository.GetInvitationByTokenAsync(token);
            if (invitation == null || invitation.ExpirationDate < DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired token.");
            }

            // Create the user
            var user = new User
            {
                UserName = invitation.Email,
                Email = invitation.Email,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception("User creation failed.");
            }

            try
            {
                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Student2");
                //if (!roleAssignmentResult.Succeeded)
                //{
                //    throw new Exception("Failed to assign role to the user.");
                //}
            }
            catch (Exception ex)
            {

                throw;
            }
            // Assign the role to the user
            

            // Delete the invitation after successful registration
            await _invitationRepository.DeleteInvitationAsync(token);
        }

        public async Task<Invitation> GetValidInvitationAsync(string token)
        {
            return await _invitationRepository.GetInvitationByTokenAsync(token);
        }

        private string GenerateInvitationToken(string email)
        {
            // Generate a unique token (e.g., using a hash or GUID)
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(email + DateTime.UtcNow.ToString()));
            return Convert.ToBase64String(hash);
        }
    }
}
