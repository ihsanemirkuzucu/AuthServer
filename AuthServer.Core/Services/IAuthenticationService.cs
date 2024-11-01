using AuthServer.Core.Dtos;
using SharedLibrary.DTOs;

namespace AuthServer.Core.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto);
        Task<ResponseDto<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken);
        Task<ResponseDto<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken);
        ResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);
    }
}
