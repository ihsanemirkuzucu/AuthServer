using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using AuthServer.Service.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.DTOs;
using System.Net;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp { Email = createUserDto.Email, UserName = createUserDto.UserName, City = createUserDto.City, BirthDate = createUserDto.BirthDate};
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return ResponseDto<UserAppDto>.Fail(new ErrorDto(errors, true), HttpStatusCode.BadRequest.GetHashCode());
            }

            var userApDto = ObjectMapper.Mapper.Map<UserAppDto>(user);
            return ResponseDto<UserAppDto>.Success(userApDto, HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
            {
                return ResponseDto<UserAppDto>.Fail("Username Not Found.", HttpStatusCode.NotFound.GetHashCode(), true);
            }
            var userApDto = ObjectMapper.Mapper.Map<UserAppDto>(user);
            return ResponseDto<UserAppDto>.Success(userApDto, HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<NoContent>> CreateUserRole(string userName)
        {
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                await _roleManager.CreateAsync(new() { Name = "admin" });
                await _roleManager.CreateAsync(new() { Name = "manager" });
            }

            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, "admin");
            await _userManager.AddToRoleAsync(user, "manager");
            return ResponseDto<NoContent>.Success(HttpStatusCode.Created.GetHashCode());
        }
    }
}
