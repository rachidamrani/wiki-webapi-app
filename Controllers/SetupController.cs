using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WikiApi.Data;
using WikiApi.Data.Models;

namespace WikiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SetupRolesController : ControllerBase
    {
        private readonly WikiDbContext _wikiDbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SetupRolesController> _logger;

        public SetupRolesController(
            WikiDbContext wikiDbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SetupRolesController> logger
            )
        {
            _wikiDbContext = wikiDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Get all available roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            List<IdentityRole>? roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            // Check if the role exist
            bool roleExist = await _roleManager.RoleExistsAsync(name);

            if (!roleExist) // Checks on the role exist status
            {
                IdentityResult? roleResult = await _roleManager.CreateAsync(new IdentityRole(name));

                // Need to check if the roles has been added successfully
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"The role {name} has been added successfully.");

                    return Ok(new
                    {
                        result = $"The role {name} has been added successfully"
                    });
                }
                else
                {
                    _logger.LogInformation($"The role {name} has not been added.");

                    return BadRequest(new
                    {
                        error = $"The role {name} has not been added added successfully"
                    });
                }

            }

            return BadRequest(new { error = "Role already exists" });

        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// Assign role to a given user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            // Check if the user exist

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogInformation($"The user with the {email} does not exist.");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            // Check if the role exist
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                _logger.LogInformation($"The role {roleName} does not exist");
                return BadRequest(new
                {
                    error = "Role does not exist"
                });
            }

            // Check if the user is assigned to the role successfully
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok(new { result = "Success, user has been added to the role" });
            }
            else
            {
                _logger.LogInformation("The user was not be able to be added to the role");
                return BadRequest(new
                {
                    error = "The user was not be able to be added to the role"
                });
            }
        }

        /// <summary>
        /// Get a user's role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            // Check if the user exists

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogInformation($"The user with the {email} does not exist.");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            // return the roles

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        /// <summary>
        ///  Remove a user from a role
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            // Check if the user exists
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogInformation($"The user with the {email} does not exist.");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            // Check if the role exists

            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                _logger.LogInformation($"The role {roleName} does not exist");
                return BadRequest(new
                {
                    error = "Role does not exist"
                });
            }

            // Remove 
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);


            if (result.Succeeded)
            {
                return Ok(new
                {
                    result = $"User {email} has been removed from role {roleName}"
                });
            }

            return BadRequest(new
            {
                error = $"Unable to remove user {email} from role {roleName}"
            });
        }


    }
}