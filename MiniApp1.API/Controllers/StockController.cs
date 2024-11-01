using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniApp1.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {

        [Authorize(Roles = "admin")]
        [Authorize(Policy = "AgePolicy")]
        [HttpGet]
        public async Task<IActionResult> GetSTOCK()
        {
            var username = HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var userRolesClaim = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            var rolesAsString = string.Join(", ", userRolesClaim);
            return Ok($"Stok işlemleri: {userIdClaim.Value}---{username}---{userEmailClaim.Value}---{rolesAsString}");
        }
    }
}
