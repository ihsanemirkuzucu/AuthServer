using AuthServer.Core.Configurations;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SharedLibrary.Services;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _customTokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> customTokenOption)
        {
            _userManager = userManager;
            _customTokenOption = customTokenOption.Value;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accestokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
            var refreshtokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.ResfreshTokenExpiration);
            var secretKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);
            SigningCredentials signingCredentials =
                new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
            (
                issuer: _customTokenOption.Issuer,
                expires: accestokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaims(userApp, _customTokenOption.Audience).Result,
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accestokenExpiration,
                RefreshTokenExpiration = refreshtokenExpiration
            };
            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accestokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
            var secretKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);
            SigningCredentials signingCredentials =
                new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
            (
                issuer: _customTokenOption.Issuer,
                expires: accestokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimByClient(client),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new ClientTokenDto()
            {
                AccessToken = token,
                AccessTokenExpiration = accestokenExpiration,
            };
            return tokenDto;
        }

        private async Task<IEnumerable<Claim>> GetClaims(UserApp userApp, List<string> audiences)
        {
            var userRoles = await _userManager.GetRolesAsync(userApp);

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, userApp.Id),
                new (JwtRegisteredClaimNames.Email, userApp.Email),
                new (ClaimTypes.Name, userApp.UserName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new ("city",userApp.City),
                new ("birth-date",userApp.BirthDate.ToShortDateString())
            };

            claims.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            claims.AddRange(userRoles.Select(x=> new Claim(ClaimTypes.Role,x)));
            return claims;
        }

        private IEnumerable<Claim> GetClaimByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.Id);
            return claims;
        }

        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }
    }
}
