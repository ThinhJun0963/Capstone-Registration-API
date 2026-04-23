using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.AccountRequest;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProjectRegistration.Services.Service.AccountService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<ApplicationUser> userManager,
                           IConfiguration config,
                           IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            // Assign role
            await _userManager.AddToRoleAsync(user, request.Role);

            // Create profile theo role
            if (request.Role == "Student")
            {
                var student = new Student
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    ApplicationUserId = user.Id
                };

                await _unitOfWork.Students.AddAsync(student);
            }
            else if (request.Role == "Lecturer")
            {
                var lecturer = new Lecturer
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    ApplicationUserId = user.Id
                };

                await _unitOfWork.Lecturers.AddAsync(lecturer);
            }
            else if (request.Role == "Admin")
            {
                var admin = new Admin
                {
                    Name = request.Name,
                    Email = request.Email,
                    ApplicationUserId = user.Id
                };

                await _unitOfWork.Admins.AddAsync(admin);
            }

            await _unitOfWork.SaveChangesAsync();

            return "Register success";
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return "User not found";

            var check = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!check)
                return "Wrong password";

            var roles = await _userManager.GetRolesAsync(user);
            return GenerateJwtToken(user, roles);
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!)
    );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? "")
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
