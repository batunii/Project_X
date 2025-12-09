// Maps to the "profiles" table in Supabase
using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace SupabaseAuth.Models
{
    [Table("profiles")]
    public class Profile : BaseModel
    {
        [PrimaryKey("id")]
        public string Id { get; set; } = string.Empty;

        [Column("role")]
        public string? Role { get; set; }
    }
}   