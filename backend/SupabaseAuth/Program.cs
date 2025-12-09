// testing features

using SupabaseAuth.Services;
using SupabaseAuth.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string supabaseUrl = "https://jdorkglqkatydqxcgshu.supabase.co";
        string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Impkb3JrZ2xxa2F0eWRxeGNnc2h1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjM5ODk4NzcsImV4cCI6MjA3OTU2NTg3N30.cOeuchYm2_Ix3Kp61rUmWb5MBt7_nqE69fAX3pkeoK8";

        // Setup logging
        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        var logger = loggerFactory.CreateLogger<SupabaseAuthService>();

        var authService = new SupabaseAuthService(supabaseUrl, supabaseKey, logger);

        string email = "jaden.adamczak@gmail.com";
        string password = "12345678";

        // // 1. Register user
        // var newUser = await authService.RegisterUser(email, password);
        // Console.WriteLine($"User created: {newUser.Email}");

        // 2. Log in user and set their role to Artist
        var session = await authService.LoginUser(email, password);
        Console.WriteLine($"Logged in: {session.User.Email}");

        // Always use the authenticated user's ID
        var userId = session.User.Id;
        Console.WriteLine($"Authenticated userId: {userId}");

        // 3. Get current role
        var currentRole = await authService.GetUserRole(userId);
        Console.WriteLine($"Current Role: {currentRole}");

        // 4. Set role to Artist
        await authService.SetUserRole(userId, "Artist");
        Console.WriteLine($"Set role to Artist for userId: {userId}");

        // 5. Get updated role
        var updatedRole = await authService.GetUserRole(userId);
        Console.WriteLine($"Updated Role: {updatedRole}");
    }
}