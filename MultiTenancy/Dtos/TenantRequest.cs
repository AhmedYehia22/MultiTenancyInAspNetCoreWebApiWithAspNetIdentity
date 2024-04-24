using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Dtos
{
    public class TenantRequest
    {
        [Required, MaxLength(30)]
        public string TenantName { get; set; } = null!;
        [Required, MaxLength(50)]
        public string OwnerName { get; set; } = null!; 
        [Required, MaxLength(50), EmailAddress]
        public string OwnerEmail { get; set; } = null!;
        

    }
}
