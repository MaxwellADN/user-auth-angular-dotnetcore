using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_core.Models
{
    public class Tenant
    {
        [Key, Required]
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}