using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_core.Models
{
    public class User
    {
        [Key, Required]
        public long Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool AgreeTerm { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool RememberMe { get; set; }
        [NotMapped]
        public string Token { get; set; }
        [NotMapped]
        public string AppOriginUrl { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        
    }
}