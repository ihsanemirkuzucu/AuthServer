using AuthServer.Core.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using SharedLibrary.DTOs;

namespace AuthServer.Core.Services
{
    public interface IUserService
    {
        Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<ResponseDto<UserAppDto>> GetUserByNameAsync(string userName);
        Task<ResponseDto<NoContent>> CreateUserRole(string userName);
    }
}
