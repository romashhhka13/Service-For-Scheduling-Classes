using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Models;
using ScheduleMaster.Security;

namespace ScheduleMaster.Services
{
    public class AuthService
    {
        public readonly ScheduleMasterDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly JwtProvider _jwtProvider;


        public AuthService(ScheduleMasterDbContext context, IConfiguration configuration, JwtProvider jwtProvider)
        {
            _context = context;
            _configuration = configuration;
            _jwtProvider = jwtProvider;
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO registerDTO)
        {

            var userExists = await _context.Users.AnyAsync(user => user.Email == registerDTO.Email);
            if (userExists)
                throw new Exception($"Пользователь с email {registerDTO.Email} уже существует");

            var hashedPassword = PasswordHasher.Generate(registerDTO.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Surname = registerDTO.Surname,
                Name = registerDTO.Name,
                MiddleName = registerDTO.MiddleName,
                Email = registerDTO.Email,
                PasswordHash = hashedPassword,
                Role = registerDTO.Role,
                Faculty = registerDTO.Faculty,
                GroupName = registerDTO.GroupName,
                Course = registerDTO.Course
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponseDTO
            {
                Email = user.Email,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Role = user.Role,
                Faculty = user.Faculty,
                GroupName = user.GroupName,
                Course = user.Course
            };
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null || !PasswordHasher.Verify(loginDTO.Password, user.PasswordHash))
                throw new Exception("Неверный email или пароль");

            var token = _jwtProvider.GenerateToken(user);
            // String token = "123";

            return new LoginResponseDTO
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresIn = 3600
            };
        }

    }
}