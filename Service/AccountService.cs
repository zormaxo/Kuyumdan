using API.Errors;
using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Service
{
    public class AccountService : BaseService
    {
        private readonly IGenericRepository<AppUser> _appUsersRepo;
        private readonly ITokenService _tokenService;

        public AccountService(IGenericRepository<AppUser> appUsersRepo, IMapper mapper, ITokenService tokenService) : base(mapper)
        {
            _appUsersRepo = appUsersRepo;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
                return new ApiResponse<UserDto>((int)HttpStatusCode.BadRequest, "Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                Username = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            await _appUsersRepo.AddAsync(user);
            await _appUsersRepo.SaveChangesAsync();

            var userDto = new UserDto
            {
                FirstName = user.FirstName,
                Token = _tokenService.CreateToken(user)
            };

            return new ApiResponse<UserDto>(userDto);
        }

        public async Task<ApiResponse<UserDto>> Login(LoginDto loginDto)
        {
            var spec = new UsersSpecification(loginDto.Username);
            var user = await _appUsersRepo.GetEntityWithSpec(spec);

            if (user == null)
                return new ApiResponse<UserDto>((int)HttpStatusCode.Unauthorized, "Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return new ApiResponse<UserDto>((int)HttpStatusCode.Unauthorized, "Invalid password");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Token = _tokenService.CreateToken(user)
            };

            return new ApiResponse<UserDto>(userDto);
        }

        private async Task<bool> UserExists(string username)
        {
            return await _appUsersRepo.AnyAsync(x => x.Username == username.ToLower());
        }
    }
}