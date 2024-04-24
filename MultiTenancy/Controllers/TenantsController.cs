using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Dtos;
using MultiTenancy.Models;
using MultiTenancy.Settings;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "superadmin")]
    public class TenantsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public TenantsController(ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        [HttpGet("GetAllTenants")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_context.Tenants.ToList());
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(_context.Tenants.Find(id));
        }
        [HttpPost("AddTenant")]
        public async Task<IActionResult> AddTenant(TenantRequest tenantRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var tenantInDb = _context.Tenants.SingleOrDefault(t => t.Name == tenantRequest.TenantName);
            //var user = _userManager.FindByEmailAsync(tenantRequest.OwnerEmail);
            var user = _context.Users.SingleOrDefault(u=>u.UserName==tenantRequest.OwnerEmail);
            if (user == null)
            {
                user = _context.Users.SingleOrDefault(u => u.Email == tenantRequest.OwnerEmail);
               // user = _userManager.FindByEmailAsync(tenantRequest.OwnerEmail);
                if (user is not null)
                {
                    return BadRequest("Try Different UserName");
                }
            }else
            {
                return BadRequest("Try Different UserName");
            }
            if (tenantInDb != null)
            {
                return BadRequest("Try Different TenantName");

            }
            var tenant = new AppTenant
            {
                Name = tenantRequest.TenantName,
                DbProvider = "mssql",
                ConnectionString = $"server=(localdb)\\ProjectModels;database={tenantRequest.TenantName}Db;trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;"
            };
            await _context.Tenants.AddAsync(tenant);
            _context.SaveChanges();
            var tenantOwner = new AppUser
            {
                UserName = tenantRequest.OwnerName,
                NormalizedUserName = tenantRequest.OwnerName,
                Email = tenantRequest.OwnerEmail,
                NormalizedEmail = tenantRequest.OwnerEmail,
                TenantId = tenantRequest.TenantName,
                
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            Random _rdm = new Random();
            int random = _rdm.Next(0000, 9999);
            var generatedPass = $"{Char.ToUpper(tenantOwner.UserName.First())}{Char.ToLower(tenantOwner.UserName.First())}@{tenantRequest.TenantName.ToLower()}{random}";
            var result = await _userManager.CreateAsync(tenantOwner, generatedPass);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(tenantOwner, "tenantadmin");
            await SendConfirmationEmail(tenantOwner.Email, tenantOwner);
            return Ok("Tenant Created Successfully!");
        }

        private async Task SendConfirmationEmail(string? email, AppUser? user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:7060/api/Tenants/confirm-email?UserId={user.Id}&Token={token}";
            await _emailService.SendEmailAsync(email, "Email Confrimation", $"<h3>Please confirm your account by</h3> <a href='{confirmationLink}'>clicking here</a>;.", true);

        }
        [HttpGet("confirm-email")]
        public async Task<String> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (userId == null || token == null)
            {
                return "Link expired";
            }
            else if (user == null)
            {
                return "User not Found";
            }
            else
            {
                token = token.Replace(" ", "+");
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return "Thank you for confirming your email";
                }
                else
                {
                    return "Email not confirmed";
                }
            }
        }
    }
}