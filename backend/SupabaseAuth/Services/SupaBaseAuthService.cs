using Supabase;
using Supabase.Gotrue;
using SupabaseAuth.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SupabaseAuth.Services
{
    public class SupabaseAuthService
    {
        private readonly Supabase.Client _client;
        private readonly ILogger<SupabaseAuthService>? _logger;

        public SupabaseAuthService(string supabaseUrl, string supabaseKey, ILogger<SupabaseAuthService>? logger = null)
        {
            _logger = logger;
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false
            };

            _client = new Supabase.Client(supabaseUrl, supabaseKey, options);
            _client.InitializeAsync().Wait();
        }

        // Create a new user
        public async Task<User?> RegisterUser(string email, string password)
        {
            try
            {
                _logger?.LogInformation("Attempting to register user with email: {Email}", email);
                var response = await _client.Auth.SignUp(email, password);
                
                if (response?.User == null)
                {
                    _logger?.LogError("User registration failed for email: {Email}", email);
                    throw new Exception("User creation failed.");
                }
                
                _logger?.LogInformation("Successfully registered user: {UserId}", response.User.Id);
                return response.User;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error registering user with email: {Email}", email);
                throw;
            }
        }

        // Log in a user
        public async Task<Session?> LoginUser(string email, string password)
        {
            try
            {
                _logger?.LogInformation("Attempting to login user with email: {Email}", email);
                var response = await _client.Auth.SignIn(email, password);
                
                if (response?.User == null)
                {
                    _logger?.LogError("Login failed for email: {Email}", email);
                    throw new Exception("Login failed.");
                }
                
                _logger?.LogInformation("Successfully logged in user: {UserId}", response.User.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error logging in user with email: {Email}", email);
                throw;
            }
        }

        // Get the role of a user
        public async Task<UserRole> GetUserRole(string userId)
        {
            try
            {
                _logger?.LogInformation("Getting role for user: {UserId}", userId);
                var response = await _client
                    .From<Profile>()
                    .Where(x => x.Id == userId)
                    .Single();

                if (response == null)
                {
                    _logger?.LogInformation("No profile found for user {UserId}, returning Guest role", userId);
                    return UserRole.Guest;
                }

                var role = response.Role?.ToLower() switch
                {
                    "artist" => UserRole.Artist,
                    "admin" => UserRole.Admin,
                    _ => UserRole.Guest,
                };
                
                _logger?.LogInformation("Retrieved role {Role} for user: {UserId}", role, userId);
                return role;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting role for user: {UserId}", userId);
                throw;
            }
        }

        // Set the role of a user
        public async Task SetUserRole(string userId, string role)
        {
            try
            {
                _logger?.LogInformation("Setting role {Role} for user: {UserId}", role, userId);
                
                var profile = new Profile
                {
                    Id = userId,
                    Role = role
                };

                await _client
                    .From<Profile>()
                    .Upsert(profile);
                    
                _logger?.LogInformation("Successfully set role {Role} for user: {UserId}", role, userId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error setting role {Role} for user: {UserId}", role, userId);
                throw;
            }
        }
    }
}

