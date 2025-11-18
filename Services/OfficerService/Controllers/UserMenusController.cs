using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.Services;
using System.Security.Claims;

namespace OfficerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserMenusController : ControllerBase
    {
        private readonly IRoleMenuService _roleMenuService;

        public UserMenusController(IRoleMenuService roleMenuService)
        {
            _roleMenuService = roleMenuService;
        }

        /// <summary>
        /// Get all menus for a specific user (flat list)
        /// </summary>
        [HttpGet("GetUserMenus/{loginId}")]
        public async Task<IActionResult> GetUserMenus(string loginId)
        {
            try
            {
                // Verify the user is requesting their own menus or is an admin
                var currentLoginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                  ?? User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("1");

                if (currentLoginId != loginId && !isAdmin)
                {
                    return Forbid();
                }

                var menus = await _roleMenuService.GetUserMenusAsync(loginId);
                
                return Ok(new
                {
                    message = "User menus retrieved successfully",
                    loginId = loginId,
                    menus = menus,
                    count = menus.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user menus.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get hierarchical menu tree for a user
        /// </summary>
        [HttpGet("GetUserMenuTree/{loginId}")]
        public async Task<IActionResult> GetUserMenuTree(string loginId)
        {
            try
            {
                // Verify the user is requesting their own menus or is an admin
                var currentLoginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                  ?? User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("1");

                if (currentLoginId != loginId && !isAdmin)
                {
                    return Forbid();
                }

                var menuTree = await _roleMenuService.GetUserMenuTreeAsync(loginId);
                
                return Ok(new
                {
                    message = "User menu tree retrieved successfully",
                    loginId = loginId,
                    menus = menuTree
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user menu tree.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get menus for the currently authenticated user
        /// </summary>
        [HttpGet("GetMyMenus")]
        public async Task<IActionResult> GetMyMenus()
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(loginId))
                {
                    return Unauthorized(new { message = "Unable to identify user." });
                }

                var menus = await _roleMenuService.GetUserMenusAsync(loginId);
                
                return Ok(new
                {
                    message = "Your menus retrieved successfully",
                    menus = menus,
                    count = menus.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving menus.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get menu tree for the currently authenticated user
        /// </summary>
        [HttpGet("GetMyMenuTree")]
        public async Task<IActionResult> GetMyMenuTree()
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(loginId))
                {
                    return Unauthorized(new { message = "Unable to identify user." });
                }

                var menuTree = await _roleMenuService.GetUserMenuTreeAsync(loginId);
                
                return Ok(new
                {
                    message = "Your menu tree retrieved successfully",
                    menus = menuTree
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving menu tree.", error = ex.Message });
            }
        }
    }
}
