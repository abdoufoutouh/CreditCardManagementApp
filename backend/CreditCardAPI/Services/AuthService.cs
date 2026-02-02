using CreditCardManagementApp.Data;
using CreditCardManagementApp.DTOS;
using CreditCardManagementApp.Helpers;
using CreditCardManagementApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CreditCardManagementApp.Services
{
    /// <summary>
    /// Service for handling authentication business logic
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponse?> SignupAsync(SignupRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _passwordHasher = new PasswordHasher<User>();
        }

        /// <summary>
        /// Registers a new user and returns authentication token
        /// </summary>
        public async Task<AuthResponse?> SignupAsync(SignupRequest request)
        {
           
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return null; 
            }


            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.ToLower(),
                Password = string.Empty // Will be set after hashing
            };

            // Hash the password
            user.Password = _passwordHasher.HashPassword(user, request.Password);

            // Save user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user.Id, user.Email);
            var expiresAt = _jwtHelper.GetTokenExpiration();

            return new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ExpiresAt = expiresAt
            };
        }

        /// <summary>
        /// Authenticates a user and returns authentication token
        /// </summary>
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return null; // User not found
            }

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user, 
                user.Password, 
                request.Password
            );

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return null; // Invalid password
            }

            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user.Id, user.Email);
            var expiresAt = _jwtHelper.GetTokenExpiration();

            return new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ExpiresAt = expiresAt
            };
        }
    }
}
