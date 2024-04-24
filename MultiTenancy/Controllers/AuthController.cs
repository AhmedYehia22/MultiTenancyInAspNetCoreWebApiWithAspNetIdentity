using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Authorization;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context, 
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {  
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByNameAsync(request.Email);

            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(request.Email);
            }
            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return BadRequest("Invalid Login Attempts");
          
                var userRole = (from ur in _context.UserRoles
                                join r in _context.Roles on ur.RoleId equals r.Id
                                where (ur.UserId == user.Id)
                                select new Roles
                                {
                                    Id = r.Id,
                                    Name = r.Name
                                }).ToList();
               
                    var token = JwtHelper.GenerateToken(user, userRole,_context.Tenants.FirstOrDefault(t => t.Name == user.TenantId));
                return Ok(token);
            }
        [HttpGet("CreateNewRole")]
        [Authorize(Roles="superadmin")]
        public async Task<IActionResult> CreateRole()
        {
            var roles = new[] { "superadmin", "tenantadmin", "manager","teacher","student" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

            }

            return Ok("Roles Created successfully!");
        }
    }
   
      
    }

