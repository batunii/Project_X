# VR Art Gallery Authentication Integration Guide

## Overview

This guide explains how to integrate the Supabase authentication system into your Unity VR Art Gallery project.

## Prerequisites

1. **Unity Packages Required:**
   - Supabase Unity SDK
   - TextMeshPro (for UI)
   - XR Interaction Toolkit (already in project)

2. **Supabase Setup:**
   - Supabase project with authentication enabled
   - `profiles` table created with the following schema:
   ```sql
   create table profiles (
     id uuid references auth.users on delete cascade primary key,
     role text default 'Guest',
     display_name text,
     avatar_url text,
     created_at timestamp with time zone default timezone('utc'::text, now()) not null,
     updated_at timestamp with time zone default timezone('utc'::text, now()) not null
   );
   ```

## Installation Steps

### 1. Install Supabase Unity SDK

```bash
# Using Unity Package Manager, add this Git URL:
https://github.com/supabase-community/supabase-csharp.git
```

### 2. Setup Authentication Manager

1. Create an empty GameObject in your scene
2. Name it "AuthenticationManager"
3. Add the `AuthenticationManager` component
4. Configure your Supabase URL and API key in the inspector

### 3. Setup Game Manager

1. Create an empty GameObject in your scene (or use the same one)
2. Name it "GameManager" 
3. Add the `GameManager` component
4. Configure scene names in the inspector

### 4. Create Authentication UI

1. Create a Canvas in your scene
2. Add the authentication UI elements:
   - Login Panel (with email/password fields and login button)
   - Register Panel (with email/password/confirm fields and register button)
   - Authenticated Panel (with welcome text and logout button)
   - Loading Panel (with loading indicator)
   - Error Panel (with error message display)

3. Add the `AuthenticationUI` component to the Canvas
4. Assign all UI elements in the inspector

### 5. Add User Status Display

1. In any scene where you want to show user status
2. Add the `UserStatusDisplay` component to a UI element
3. Configure the UI references in the inspector

## Script Overview

### Core Scripts

1. **`AuthenticationManager.cs`**
   - Singleton that handles all authentication logic
   - Manages Supabase connection
   - Provides events for UI updates
   - Handles user sessions and roles

2. **`GameManager.cs`**
   - Main application manager
   - Handles authentication state across scenes
   - Manages feature access based on user roles
   - Provides scene management

3. **`UserRole.cs`**
   - Enum defining user roles (Guest, Artist, Admin)
   - Extension methods for role-based permissions

4. **`UserProfile.cs`**
   - Model class for Supabase profiles table
   - Maps to database schema

### UI Scripts

5. **`AuthenticationUI.cs`**
   - Complete authentication interface
   - Handles login/registration forms
   - Input validation and error display
   - Responsive to authentication events

6. **`UserStatusDisplay.cs`**
   - Displays current user information
   - Can be used in any scene
   - Shows authentication status and user role

## Usage Examples

### Basic Authentication Check

```csharp
public class ExampleScript : MonoBehaviour
{
    private void Start()
    {
        var gameManager = GameManager.Instance;
        
        if (gameManager.IsUserAuthenticated)
        {
            Debug.Log($"User is logged in as: {gameManager.GetCurrentUserDisplayName()}");
            Debug.Log($"User role: {gameManager.CurrentUserRole}");
        }
        else
        {
            Debug.Log("User is not authenticated");
        }
    }
}
```

### Role-Based Feature Access

```csharp
public class ArtCreationTool : MonoBehaviour
{
    private void Start()
    {
        var gameManager = GameManager.Instance;
        
        // Only allow art creation for Artists and Admins
        if (gameManager.CanCreateArt())
        {
            EnableArtTools();
        }
        else
        {
            ShowUpgradeToArtistMessage();
        }
    }
}
```

### Listening to Authentication Events

```csharp
public class ExampleEventListener : MonoBehaviour
{
    private void Start()
    {
        var authManager = AuthenticationManager.Instance;
        
        authManager.OnUserLoggedIn += (user) =>
        {
            Debug.Log($"Welcome {user.Email}!");
        };
        
        authManager.OnUserLoggedOut += () =>
        {
            Debug.Log("User logged out");
        };
        
        authManager.OnUserRoleChanged += (role) =>
        {
            Debug.Log($"User role changed to: {role}");
        };
    }
}
```

## Configuration

### Authentication Manager Settings

- **Supabase URL**: Your Supabase project URL
- **Supabase Key**: Your Supabase anon/public key
- **Enable Debug Logs**: Enable/disable debug logging

### Game Manager Settings

- **Scene Names**: Configure your scene names
- **Require Authentication**: Whether gallery access requires authentication
- **Minimum Role**: Minimum role required for gallery access

### Authentication UI Settings

- **Hide UI When Authenticated**: Hide auth UI after successful login
- **Error Display Duration**: How long to show error messages

## Security Notes

1. **Never commit API keys to version control**
   - Use Unity's built-in secrets management
   - Consider environment-based configuration

2. **Row Level Security (RLS)**
   - Enable RLS on your Supabase tables
   - Ensure users can only modify their own profiles

3. **Client-side validation**
   - All validation in Unity is for UX only
   - Server-side validation must be implemented in Supabase

## Troubleshooting

### Common Issues

1. **"Supabase client not initialized"**
   - Ensure AuthenticationManager is in the scene
   - Check Supabase URL and key are correct

2. **"Profile table not found"**
   - Create the profiles table in Supabase
   - Ensure table permissions are set correctly

3. **"Authentication failed"**
   - Check email/password are correct
   - Verify Supabase authentication is enabled
   - Check network connectivity

### Debug Information

Enable debug logs in AuthenticationManager to see detailed authentication flow information.

## Next Steps

1. **Implement VR-specific UI**
   - Create 3D authentication interfaces
   - Add hand tracking support for input

2. **Add more user features**
   - Profile customization
   - Avatar selection
   - Friend lists

3. **Integrate with art system**
   - Link artwork to authenticated users
   - Implement ownership tracking
   - Add collaboration features

## Support

For issues related to:
- **Unity integration**: Check Unity console for errors
- **Supabase connection**: Verify dashboard settings
- **Authentication flow**: Enable debug logging