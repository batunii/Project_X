using System;

namespace VRGallery.Authentication
{
    /// <summary>
    /// User roles in the VR Art Gallery system
    /// </summary>
    [Serializable]
    public enum UserRole
    {
        Guest = 0,      // Default role, limited access
        Artist = 1,     // Can create and submit artwork
        Admin = 2       // Full access to all features
    }

    /// <summary>
    /// Extension methods for UserRole enum
    /// </summary>
    public static class UserRoleExtensions
    {
        public static string ToDisplayString(this UserRole role)
        {
            return role switch
            {
                UserRole.Guest => "Guest",
                UserRole.Artist => "Artist",
                UserRole.Admin => "Administrator",
                _ => "Unknown"
            };
        }

        public static bool CanCreateArt(this UserRole role)
        {
            return role >= UserRole.Artist;
        }

        public static bool CanModerateContent(this UserRole role)
        {
            return role >= UserRole.Admin;
        }

        public static bool CanManageUsers(this UserRole role)
        {
            return role >= UserRole.Admin;
        }
    }
}