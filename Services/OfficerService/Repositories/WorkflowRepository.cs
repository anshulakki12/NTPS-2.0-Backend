using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly AppDbContext _context;

        public WorkflowRepository(AppDbContext context)
        {
            _context = context;
        }

        // Master Level methods
        public async Task<IEnumerable<MasterLevel>> GetAllMasterLevelsAsync()
        {
            return await _context.MasterLevels
                .Where(ml => ml.IsActive)
                .OrderBy(ml => ml.LevelName)
                .ToListAsync();
        }

        // Role methods
        public async Task<IEnumerable<MasterRoles>> GetAllRolesAsync()
        {
            return await _context.MasterRoles
                .Where(r => r.IsActive)
                .OrderBy(r => r.RoleName) // Changed from RoleId to RoleName for better ordering
                .ToListAsync();
        }

        // Designation methods
        public async Task<IEnumerable<MasterDesignation>> GetAllDesignationsAsync()
        {
            return await _context.MasterDesignations
                .Where(d => d.IsActive)
                .OrderBy(d => d.DesignationName)
                .ToListAsync();
        }

        // State methods
        public async Task<IEnumerable<State>> GetAllStatesAsync()
        {
            return await _context.States
                .OrderBy(s => s.StateName)
                .ToListAsync();
        }

        // Master Workflow methods
        public async Task<IEnumerable<MasterWorkFlow>> GetAllMasterWorkflowsAsync()
        {
            return await _context.MasterWorkFlows
                .Include(mw => mw.State)
                .Where(mw => mw.IsActive)
                .OrderBy(mw => mw.WorkFlowName)
                .ToListAsync();
        }

        public async Task<MasterWorkFlow> GetMasterWorkflowByIdAsync(int workflowId)
        {
            return await _context.MasterWorkFlows
                .Include(mw => mw.State)
                .FirstOrDefaultAsync(mw => mw.WorkFlowId == workflowId);
        }

        public async Task<MasterWorkFlow> CreateMasterWorkflowAsync(MasterWorkFlow workflow)
        {
            _context.MasterWorkFlows.Add(workflow);
            await _context.SaveChangesAsync();
            return workflow;
        }

        public async Task<MasterWorkFlow> UpdateMasterWorkflowAsync(MasterWorkFlow workflow)
        {
            _context.MasterWorkFlows.Update(workflow);
            await _context.SaveChangesAsync();
            return workflow;
        }

        public async Task<bool> DeleteMasterWorkflowAsync(int workflowId)
        {
            var workflow = await _context.MasterWorkFlows.FindAsync(workflowId);
            if (workflow == null) return false;

            // First delete all associated steps
            await DeleteWorkflowStepsByWorkflowAsync(workflowId);

            _context.MasterWorkFlows.Remove(workflow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleMasterWorkflowStatusAsync(int workflowId, bool isActive)
        {
            var workflow = await _context.MasterWorkFlows.FindAsync(workflowId);
            if (workflow == null) return false;

            workflow.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultWorkflowAsync(int workflowId)
        {
            // Reset all workflows to non-default
            var allWorkflows = await _context.MasterWorkFlows.ToListAsync();
            foreach (var wf in allWorkflows)
            {
                wf.IsDefault = false;
            }

            // Set the specified workflow as default
            var defaultWorkflow = await _context.MasterWorkFlows.FindAsync(workflowId);
            if (defaultWorkflow != null)
            {
                defaultWorkflow.IsDefault = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Workflow Steps methods
        public async Task<IEnumerable<WorkFlowSteps>> GetAllWorkflowStepsAsync()
        {
            return await _context.WorkFlowSteps
                .Include(ws => ws.MasterWorkFlow)
                .Include(ws => ws.MasterLevel)
                .Include(ws => ws.Role)          // Use navigation property, not foreign key
                .Include(ws => ws.Designation)   // Use navigation property, not foreign key
                .OrderBy(ws => ws.MasterWorkFlow.WorkFlowName)
                .ThenBy(ws => ws.StepOrder)
                .ToListAsync();
        }

        public async Task<WorkFlowSteps> GetWorkflowStepByIdAsync(int stepId)
        {
            return await _context.WorkFlowSteps
                .Include(ws => ws.MasterWorkFlow)
                .Include(ws => ws.MasterLevel)
                .Include(ws => ws.Role)          // Use navigation property, not foreign key
                .Include(ws => ws.Designation)   // Use navigation property, not foreign key
                .FirstOrDefaultAsync(ws => ws.StepId == stepId);
        }

        public async Task<IEnumerable<WorkFlowSteps>> CreateWorkflowStepsBulkAsync(List<WorkFlowSteps> steps)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.WorkFlowSteps.AddRange(steps);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return steps;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WorkFlowSteps> CreateWorkflowStepAsync(WorkFlowSteps step)
        {
            _context.WorkFlowSteps.Add(step);
            await _context.SaveChangesAsync();
            return step;
        }

        public async Task<WorkFlowSteps> UpdateWorkflowStepAsync(WorkFlowSteps step)
        {
            _context.WorkFlowSteps.Update(step);
            await _context.SaveChangesAsync();
            return step;
        }

        public async Task<bool> DeleteWorkflowStepAsync(int stepId)
        {
            var step = await _context.WorkFlowSteps.FindAsync(stepId);
            if (step == null) return false;

            _context.WorkFlowSteps.Remove(step);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWorkflowStepsByWorkflowAsync(int workflowId)
        {
            var steps = await _context.WorkFlowSteps
                .Where(ws => ws.WorkFlowId == workflowId)
                .ToListAsync();

            if (!steps.Any()) return false;

            _context.WorkFlowSteps.RemoveRange(steps);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleWorkflowStepStatusAsync(int stepId, bool isActive)
        {
            // Note: WorkFlowSteps doesn't have IsActive field in current schema
            // You might need to add it or handle status differently
            return await Task.FromResult(true); // Placeholder implementation
        }

        public async Task<IEnumerable<WorkFlowSteps>> GetWorkflowStepsByWorkflowIdAsync(int workflowId)
        {
            return await _context.WorkFlowSteps
                .Include(ws => ws.MasterWorkFlow)
                .Include(ws => ws.MasterLevel)
                .Include(ws => ws.Role)          // Use navigation property, not foreign key
                .Include(ws => ws.Designation)   // Use navigation property, not foreign key
                .Where(ws => ws.WorkFlowId == workflowId)
                .OrderBy(ws => ws.StepOrder)
                .ToListAsync();
        }
    }
}