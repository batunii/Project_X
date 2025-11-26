#nullable enable
using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace VRGallery.Authentication
{
    /// <summary>
    /// User profile model that maps to the "profiles" table in Supabase
    /// </summary>
    [Table("profiles")]
    public class UserProfile : BaseModel
    {
        [PrimaryKey("id")]
        public string Id { get; set; } = string.Empty;

        [Column("role")]
        public string? Role { get; set; }

        [Column("display_name")]
        public string? DisplayName { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}