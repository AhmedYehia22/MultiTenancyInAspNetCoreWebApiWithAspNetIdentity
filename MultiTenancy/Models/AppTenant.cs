using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models
{
    public class AppTenant
    {
        public int Id { get; set; }
        public string Name { get; set; } =null!;
        public string ConnectionString { get; set; } =null!;
        public string DbProvider { get; set; } =null!;
        public ICollection<AppUser> Users { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
