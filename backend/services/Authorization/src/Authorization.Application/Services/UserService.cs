﻿using Authorization.Application.DTOs;
using Authorization.Application.Exceptions;
using Authorization.Application.Interfaces;
using Authorization.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(
            IInvitationRepository invitationRepository,
            IEmailService emailService,
            UserManager<User> userManager,
            IUserRepository userRepository,
            ILogger<UserService> logger,
            IConfiguration configuration)
        {
            _invitationRepository = invitationRepository;
            _emailService = emailService;
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Guid> SendInvitationAsync(CreateUserDTO createUserDTO)
        {
            string email = createUserDTO.Email;
            string fullName = createUserDTO.FullName;
            _logger.LogInformation("Sending invitation to email {Email} for user {FullName}", email, fullName);

            try
            {

                // Check if an invitation already exists with this email
                var existingInvitation = await _invitationRepository.GetInvitationByEmailAsync(email);
                if (existingInvitation != null)
                {
                    _logger.LogInformation("Invitation already exists for email {Email} with Invitation ID {InvitationId}", email, existingInvitation.Id);
                    // Return the existing invitation ID, or handle the scenario based on your business logic.
                    return existingInvitation.Id;
                }

                // Generate a unique invitation token
                var token = GenerateInvitationToken(email);
                _logger.LogInformation("Generated invitation token for email {Email}", email);

                // Create the invitation
                var invitation = new Invitation
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Token = token,
                    FullName = fullName,
                    ExpirationDate = DateTime.UtcNow.AddDays(1) // Token expires in 1 day
                };

                // Save the invitation
                await _invitationRepository.SaveInvitationAsync(invitation);
                _logger.LogInformation("Saved invitation with ID {InvitationId} for email {Email}", invitation.Id, email);

                // Send the invitation email
                var tokenQueryParam = Uri.EscapeDataString(token);
                var invitationLink = $"{_configuration["HostUrl"]}/account/acceptinvitation?token={tokenQueryParam}";
                await _emailService.SendEmailAsync(email, "Invitation to Join", $"Click here to register: {invitationLink}");

                _logger.LogInformation("Successfully sent invitation email to {Email}", email);
                return invitation.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invitation to email {Email}", email);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task CompleteRegistrationAsync(string token, string password)
        {
            _logger.LogInformation("Completing registration for token {Token}", token);

            try
            {
                // Get the invitation by token
                var invitation = await _invitationRepository.GetInvitationByTokenAsync(token);
                if (invitation == null || invitation.ExpirationDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Invalid or expired token: {Token}", token);
                    throw new Exception("Invalid or expired token.");
                }

                _logger.LogInformation("Found valid invitation for email {Email} with token {Token}", invitation.Email, token);

                // Check if a user with the same email already exists
                var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User with email {Email} already exists.", invitation.Email);
                    throw new Exception("User with this email already exists.");
                }

                // Create the user
                var user = new User
                {
                    Id = invitation.Id.ToString(),
                    UserName = invitation.Email,
                    Email = invitation.Email,
                    FullName = invitation.FullName
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    _logger.LogError("User creation failed for email {Email}. Errors: {Errors}", invitation.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new Exception("User creation failed.");
                }

                _logger.LogInformation("Successfully created user for email {Email}", invitation.Email);

                // Assign the role to the user
                try
                {
                    var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Student");
                    if (!roleAssignmentResult.Succeeded)
                    {
                        _logger.LogError("Failed to assign role to user {Email}. Errors: {Errors}", invitation.Email, string.Join(", ", roleAssignmentResult.Errors.Select(e => e.Description)));
                        throw new Exception("Role assignment failed.");
                    }

                    _logger.LogInformation("Successfully assigned role 'Student' to user {Email}", invitation.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to assign role to user {Email}", invitation.Email);
                    throw; // Re-throw the exception after logging
                }

                // Delete the invitation after successful registration
                await _invitationRepository.DeleteInvitationAsync(token);
                _logger.LogInformation("Successfully deleted invitation for token {Token}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete registration for token {Token}", token);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<Invitation> GetValidInvitationAsync(string token)
        {
            _logger.LogInformation("Entering GetValidInvitationAsync method with token: {Token}", token);

            var invitation = await _invitationRepository.GetInvitationByTokenAsync(token);

            if (invitation == null)
            {
                _logger.LogWarning("Invitation not found for token: {Token}", token);
            }
            else
            {
                _logger.LogInformation("Found invitation for token: {Token}", token);
            }

            return invitation;
        }


        private string GenerateInvitationToken(string email)
        {
            _logger.LogInformation("Entering GenerateInvitationToken method for email: {Email}", email);

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(email + DateTime.UtcNow.ToString()));
            var token = Convert.ToBase64String(hash);

            _logger.LogInformation("Generated invitation token for email: {Email}", email);

            return token;
        }


        // Method to get all users
        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            _logger.LogInformation("Entering GetAllUsers method.");

            var users = await _userRepository.GetAllUsersAsync();

            _logger.LogInformation("Fetched {UserCount} users from the repository.", users.Count());

            var userDTOs = users.Select(user => new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth
            });

            _logger.LogInformation("Converted {UserCount} users to UserDTOs.", userDTOs.Count());

            return userDTOs;
        }


        // Method to get a user by their ID
        public async Task<UserDTO> GetUserById(Guid userId)
        {
            _logger.LogInformation("Entering GetUserById method for userId: {UserId}", userId);

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User with userId {UserId} not found.", userId);
                return null; // Or throw an exception as needed
            }

            _logger.LogInformation("Found user with userId {UserId}. Converting to UserDTO.", userId);

            var userDTO = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth
            };

            return userDTO;
        }

        public async Task DeleteUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                var invitation = await _invitationRepository.GetInvitationByIdAsync(userId) 
                    ?? throw new NotFoundException($"User with ID {userId} was not found.");
                await _invitationRepository.DeleteInvitationAsync(invitation.Id);
                return; // return from here , no need to delete the user because user is not created
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to delete user: {errors}");
            }
        }
    }
}
