using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp2.API.Controllers
{
    [Authorize(Roles = "admin", Policy = "IstanbulPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetInvoice()
        {
            var username = HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var userRolesClaim = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            var rolesAsString = string.Join(", ", userRolesClaim);
            var birthDate = User.Claims.FirstOrDefault(c => c.Type == "birth-date");
            return Ok($"Stok işlemleri: {userIdClaim.Value}---{username}---{userEmailClaim.Value}---{rolesAsString}---{birthDate.Value}");
        }
    }
}
