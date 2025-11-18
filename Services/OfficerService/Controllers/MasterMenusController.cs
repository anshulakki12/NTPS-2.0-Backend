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
    public class MasterMenusController : ControllerBase
    {
        private readonly IMasterMenuService _menuService;

        public MasterMenusController(IMasterMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Get all menus
        /// </summary>
        [HttpGet("GetAllMenus")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAllMenus()
        {
            try
            {
                var menus = await _menuService.GetAllMenusAsync();
                return Ok(new
                {
                    message = "Menus retrieved successfully",
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
        /// Get active menus only
        /// </summary>
        [HttpGet("GetActiveMenus")]
        public async Task<IActionResult> GetActiveMenus()
        {
            try
            {
                var menus = await _menuService.GetActiveMenusAsync();
                return Ok(new
                {
                    message = "Active menus retrieved successfully",
                    menus = menus,
                    count = menus.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get menu by ID
        /// </summary>
        [HttpGet("GetMenuById/{id:int}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            try
            {
                var menu = await _menuService.GetMenuByIdAsync(id);
                if (menu == null)
                {
                    return NotFound(new { message = $"Menu with ID {id} not found." });
                }

                return Ok(new
                {
                    message = "Menu retrieved successfully",
                    menu = menu
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get parent menus
        /// </summary>
        [HttpGet("GetParentMenus")]
        public async Task<IActionResult> GetParentMenus()
        {
            try
            {
                var menus = await _menuService.GetParentMenusAsync();
                return Ok(new
                {
                    message = "Parent menus retrieved successfully",
                    menus = menus,
                    count = menus.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get child menus
        /// </summary>
        [HttpGet("GetChildMenus/{parentId:int}")]
        public async Task<IActionResult> GetChildMenus(int parentId)
        {
            try
            {
                var menus = await _menuService.GetChildMenusAsync(parentId);
                return Ok(new
                {
                    message = "Child menus retrieved successfully",
                    menus = menus,
                    count = menus.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new menu
        /// </summary>
        [HttpPost("CreateMenu")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto createDto)
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

                var menu = await _menuService.CreateMenuAsync(createDto);

                return CreatedAtAction(
                    nameof(GetMenuById),
                    new { id = menu.MenuId },
                    new
                    {
                        message = "Menu created successfully",
                        menu = menu
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the menu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing menu
        /// </summary>
        [HttpPut("UpdateMenu/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateDto.MenuId)
            {
                return BadRequest(new { message = "Menu ID mismatch." });
            }

            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                updateDto.UpdatedBy = loginId;

                var menu = await _menuService.UpdateMenuAsync(id, updateDto);
                return Ok(new
                {
                    message = "Menu updated successfully",
                    menu = menu
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the menu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate a menu
        /// </summary>
        [HttpPatch("ActivateMenu/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ActivateMenu(int id)
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                var menu = await _menuService.ActivateMenuAsync(id, loginId);
                return Ok(new
                {
                    message = "Menu activated successfully",
                    menu = menu
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the menu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a menu
        /// </summary>
        [HttpPatch("DeactivateMenu/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeactivateMenu(int id)
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                var menu = await _menuService.DeactivateMenuAsync(id, loginId);
                return Ok(new
                {
                    message = "Menu deactivated successfully",
                    menu = menu
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the menu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a menu
        /// </summary>
        [HttpDelete("DeleteMenu/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            try
            {
                var menuToDelete = await _menuService.GetMenuByIdAsync(id);
                
                var isDeleted = await _menuService.DeleteMenuAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Menu with ID {id} not found." });
                }

                return Ok(new 
                { 
                    message = "Menu deleted successfully",
                    deletedMenu = menuToDelete?.MenuName
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the menu.", error = ex.Message });
            }
        }
    }
}
