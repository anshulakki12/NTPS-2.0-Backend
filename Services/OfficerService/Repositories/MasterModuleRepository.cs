using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class MasterModuleRepository : IMasterModuleRepository
    {
        private readonly AppDbContext _context;

        public MasterModuleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MasterModule> CreateModuleAsync(CreateModuleDto createDto)
        {
            // Check if module code already exists
            var existingModule = await _context.Set<MasterModule>()
                .FirstOrDefaultAsync(m => m.ModuleCode == createDto.ModuleCode);

            if (existingModule != null)
            {
                throw new InvalidOperationException($"Module with code '{createDto.ModuleCode}' already exists.");
            }

            var module = new MasterModule
            {
                ModuleName = createDto.ModuleName,
                ModuleCode = createDto.ModuleCode,
                Description = createDto.Description,
                Icon = createDto.Icon,
                RoutePath = createDto.RoutePath,
                DisplayOrder = createDto.DisplayOrder,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createDto.CreatedBy
            };

            _context.Set<MasterModule>().Add(module);
            await _context.SaveChangesAsync();

            return module;
        }

        public async Task<MasterModule?> GetModuleByIdAsync(int moduleId)
        {
            return await _context.Set<MasterModule>()
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId);
        }

        public async Task<MasterModule?> GetModuleByCodeAsync(string moduleCode)
        {
            return await _context.Set<MasterModule>()
                .FirstOrDefaultAsync(m => m.ModuleCode == moduleCode);
        }

        public async Task<IEnumerable<MasterModule>> GetAllModulesAsync()
        {
            return await _context.Set<MasterModule>()
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.ModuleName)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterModule>> GetActiveModulesAsync()
        {
            return await _context.Set<MasterModule>()
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.ModuleName)
                .ToListAsync();
        }

        public async Task<MasterModule> UpdateModuleAsync(MasterModule module)
        {
            module.ModifiedDate = DateTime.UtcNow;
            _context.Set<MasterModule>().Update(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteModuleAsync(int moduleId)
        {
            var module = await GetModuleByIdAsync(moduleId);
            if (module == null)
            {
                return false;
            }

            // Check if module is referenced in permissions
            var hasPermissions = await _context.Set<Permission>()
                .AnyAsync(p => p.Module == module.ModuleCode);

            if (hasPermissions)
            {
                throw new InvalidOperationException("Cannot delete module that has associated permissions. Please delete or reassign permissions first.");
            }

            _context.Set<MasterModule>().Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
