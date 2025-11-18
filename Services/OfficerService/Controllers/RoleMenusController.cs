using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.DtoModels;
using OfficerService.Services;
using System.Security.Claims;

namespace OfficerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleMenusController : ControllerBase
    {
        private readonly IRoleMenuService _roleMenuService;

        public RoleMenusController(IRoleMenuService roleMenuService)
        {
            _roleMenuService = roleMenuService;
        }

        /// <summary>
        /// Get all menus assigned to a role
        /// </summary>
        [HttpGet("GetRoleMenus/{roleId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetRoleMenus(int roleId)
        {
            try
            {
                var roleMenus = await _roleMenuService.GetRoleMenusAsync(roleId);
                return Ok(new
                {
                    message = "Role menus retrieved successfully",
                    roleId = roleId,
                    menus = roleMenus,
                    count = roleMenus.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving role menus.", error = ex.Message });
            }
        }

        /// <summary>
        /// Assign menus to a role
        /// </summary>
        [HttpPost("AssignMenus")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AssignMenus([FromBody] CreateRoleMenuDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                createDto.CreatedBy = loginId;

                var roleMenus = await _roleMenuService.AssignMenusToRoleAsync(createDto);

                return Ok(new
                {
                    message = "Menus assigned successfully",
                    roleId = createDto.RoleId,
                    menus = roleMenus,
                    count = roleMenus.Count()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning menus.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update a role-menu assignment
        /// </summary>
        [HttpPut("UpdateRoleMenu")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateRoleMenu([FromBody] UpdateRoleMenuDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                updateDto.UpdatedBy = loginId;

                var roleMenu = await _roleMenuService.UpdateRoleMenuAsync(updateDto);

                return Ok(new
                {
                    message = "Role menu updated successfully",
                    roleMenu = roleMenu
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating role menu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove a menu from a role
        /// </summary>
        [HttpDelete("RemoveMenu/{roleId:int}/{menuId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> RemoveMenu(int roleId, int menuId)
        {
            try
            {
                var isRemoved = await _roleMenuService.RemoveMenuFromRoleAsync(roleId, menuId);
                if (!isRemoved)
                {
                    return NotFound(new { message = $"Menu assignment not found for role {roleId} and menu {menuId}." });
                }

                return Ok(new
                {
                    message = "Menu removed from role successfully",
                    roleId = roleId,
                    menuId = menuId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing menu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Reorder menus for a role
        /// </summary>
        [HttpPut("ReorderMenus/{roleId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ReorderMenus(int roleId, [FromBody] ReorderMenusDto reorderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _roleMenuService.ReorderRoleMenusAsync(roleId, reorderDto);
                return Ok(new
                {
                    message = "Menus reordered successfully",
                    roleId = roleId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while reordering menus.", error = ex.Message });
            }
        }
    }
}
