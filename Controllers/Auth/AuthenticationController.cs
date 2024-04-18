using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WikiApi.Data.DTOs;
using WikiApi.Data.Models;
using WikiApi.Data.Models.Auth;

namespace WikiApi.Controllers.Auth;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AuthController> _logger;

    // private readonly JwtConfig _jwtConfig;

    public AuthController(UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDTO requestData)
    {
        // Validate incoming request
        if (ModelState.IsValid)
        {
            // Check if the user already exists
            User? userExist = await _userManager.FindByEmailAsync(requestData.Email);

            if (userExist is not null)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string> {
                            "Email already exists"
                        }
                });
            }

            var ageIsValid = DateOnly.TryParse(requestData.Birthday, out DateOnly birthDate);
            int age = DateTime.Now.Year - birthDate.Year;

            // Check if the user has 18 years or above
            if (!ageIsValid || age < 18)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string> {
                            "User sould be 18 years old or above."
                        }
                });
            }


            // Create user
            User? newUser = new User
            {
                Email = requestData.Email,
                UserName = requestData.Email,
                Birthday = DateOnly.Parse(requestData.Birthday),
            };

            IdentityResult? isCreated = await _userManager.CreateAsync(newUser, requestData.Password);

            if (isCreated.Succeeded)
            {
                // Generate the token
                var token = await GenerateJwtToken(newUser);
                return Ok(new AuthResult
                {
                    Result = true,
                    Token = token
                });
            }

            return BadRequest(new AuthResult
            {
                Errors = new List<string> {
                    "Server error"
                },
                Result = false
            });
        }

        return BadRequest();
    }

    /// <summary>
    /// Get logged in
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginRequest)
    {
        if (ModelState.IsValid)
        {
            // Check if the user exists
            User? existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (existingUser is null)
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Invalid payload" },
                    Result = false
                });



            bool isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginRequest.Password);

            if (!isCorrect)
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Invalid crendentials" },
                    Result = false
                });


            var jwtToken = await GenerateJwtToken(existingUser);

            return Ok(new AuthResult
            {
                Token = jwtToken,
                Result = true
            });

        }

        return BadRequest(new AuthResult
        {
            Errors = new List<string> { "Invalid payload" },
            Result = false
        });
    }

    /// <summary>
    /// Generate a web token from user credential 
    /// </summary>
    /// <returns></returns>
    private async Task<string> GenerateJwtToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value!);

        var claims = await GetAllValidClaims(user);

        // Token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),

            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
             SecurityAlgorithms.HmacSha256)
        };


        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        return jwtTokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Get all valid claims for corresponding user
    /// </summary>
    /// <returns></returns>
    private async Task<List<Claim>> GetAllValidClaims(User user)
    {
        var options = new IdentityOptions();

        var claims = new List<Claim> {
             new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
        };


        // Getting the claims that we have assigned to a user
        var userClaims = await _userManager.GetClaimsAsync(user);

        claims.AddRange(userClaims);

        // Get the user role and add it to the claims
        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var userRole in userRoles)
        {

            var role = await _roleManager.FindByNameAsync(userRole);

            if (role is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    claims.Add(roleClaim);
                }
            }
        }

        return claims;
    }

}
