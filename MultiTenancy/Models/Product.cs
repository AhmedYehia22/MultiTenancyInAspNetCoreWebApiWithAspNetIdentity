using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Models;

public class Product //: IMustHaveTenant
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Rate { get; set; }
    public string TenantId { get; set; } = null!;

    [ForeignKey("AppTenant")]
    public int AppTenantId { get; set; }

}