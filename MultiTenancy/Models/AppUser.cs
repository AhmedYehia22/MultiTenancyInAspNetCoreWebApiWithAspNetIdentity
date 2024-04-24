using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Models
{
    public class AppUser : IdentityUser
    {
        public string TenantId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [ForeignKey("AppTenant")]
        public int AppTenantId { get; set; }

    }
}
