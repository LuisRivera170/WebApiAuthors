using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public AccountsController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService
        )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("UNIQUE_SECRET_VALUE");
        }

        [HttpPost("register", Name = "RegisterUser")]
        public async Task<ActionResult<AuthenticationResponse>> Post(UserCredentials userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return await BuildToken(userCredentials);
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(
                userCredentials.Email,
                userCredentials.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                return BadRequest("Bad credentials");
            }

            return await BuildToken(userCredentials);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("renew", Name = "RenewToken")]
        public async Task<ActionResult<AuthenticationResponse>> RenewToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var userCredentials = new UserCredentials()
            {
                Email = email
            };

            return await BuildToken(userCredentials);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("grant/admin", Name = "GrantAdmin")]
        public async Task<ActionResult> GrantAdmin(UpdateAdminDTO updateAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(updateAdminDTO.Email);
            await userManager.AddClaimAsync(user, new Claim("isAdmin", "true"));

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("revoke/admin", Name = "RevokeAdmin")]
        public async Task<ActionResult> RevokeAdmin(UpdateAdminDTO updateAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(updateAdminDTO.Email);
            await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "true"));

            return NoContent();
        }

        [HttpGet("encrypt")]
        public ActionResult EncryptTest()
        {
            var plainText = "Encryption test";
            var cifredText = dataProtector.Protect(plainText);

            var decriptedText = dataProtector.Unprotect(cifredText);

            return Ok(new
            {
                plainText,
                cifredText,
                decriptedText
            });
        }

        [HttpGet("hash/{plainText}")]
        public ActionResult HashTest(string plainText)
        {
            var result1 = hashService.Hash(plainText);
            var result2 = hashService.Hash(plainText);

            return Ok(new
            {
                plainText,
                result1,
                result2
            });
        }

        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };

            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);


            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
            };
        }

    }
}
