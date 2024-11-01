using AuthServer.Core.Configurations;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.DTOs;
using System.Net;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRepository;

        public AuthenticationService(IOptions<List<Client>> clients, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRepository)
        {
            _clients = clients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto is null)
            {
                throw new ArgumentException(nameof(loginDto));
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return ResponseDto<TokenDto>.Fail("Email or Password is wrong", HttpStatusCode.BadRequest.GetHashCode(), true);
            }
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return ResponseDto<TokenDto>.Fail("Email or Password is wrong", HttpStatusCode.BadRequest.GetHashCode(), true);
            }

            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRepository.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            if (userRefreshToken is null)
            {
                await _userRepository.AddAsync(new UserRefreshToken
                { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return ResponseDto<TokenDto>.Success(token, HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRepository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken is null)
            {
                return ResponseDto<TokenDto>.Fail("RefreshToken not found.", HttpStatusCode.NotFound.GetHashCode(), true);
            }

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user is null)
            {
                return ResponseDto<TokenDto>.Fail("User not found.", HttpStatusCode.NotFound.GetHashCode(), true);
            }

            var tokenDto = _tokenService.CreateToken(user);
            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return ResponseDto<TokenDto>.Success(tokenDto, HttpStatusCode.OK.GetHashCode());
        }

        public async Task<ResponseDto<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRepository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken is null)
            {
                return ResponseDto<NoDataDto>.Fail("RefreshToken not found.", HttpStatusCode.NotFound.GetHashCode(), true);
            }
            _userRepository.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return ResponseDto<NoDataDto>.Success(HttpStatusCode.OK.GetHashCode());
        }

        public ResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x =>
                x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
            if (client is null)
            {
                return ResponseDto<ClientTokenDto>.Fail("ClientId or Secret Not Found",
                    HttpStatusCode.NotFound.GetHashCode(), true);
            }

            var token = _tokenService.CreateTokenByClient(client);
            return ResponseDto<ClientTokenDto>.Success(token, HttpStatusCode.OK.GetHashCode());
        }
    }
}
