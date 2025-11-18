using OfficerService.Models;
using System.Data;

namespace OfficerService.Repositories
{
    public interface IWorkflowRepository
    {
        // Master Level methods
        Task<IEnumerable<MasterLevel>> GetAllMasterLevelsAsync();

        // Role methods
        Task<IEnumerable<MasterRoles>> GetAllRolesAsync();

        // Designation methods
        Task<IEnumerable<MasterDesignation>> GetAllDesignationsAsync();

        // State methods
        Task<IEnumerable<State>> GetAllStatesAsync();

        // Master Workflow methods
        Task<IEnumerable<MasterWorkFlow>> GetAllMasterWorkflowsAsync();
        Task<MasterWorkFlow> GetMasterWorkflowByIdAsync(int workflowId);
        Task<MasterWorkFlow> CreateMasterWorkflowAsync(MasterWorkFlow workflow);
        Task<MasterWorkFlow> UpdateMasterWorkflowAsync(MasterWorkFlow workflow);
        Task<bool> DeleteMasterWorkflowAsync(int workflowId);
        Task<bool> ToggleMasterWorkflowStatusAsync(int workflowId, bool isActive);
        Task<bool> SetDefaultWorkflowAsync(int workflowId);

        // Workflow Steps methods
        Task<IEnumerable<WorkFlowSteps>> GetAllWorkflowStepsAsync();
        Task<WorkFlowSteps> GetWorkflowStepByIdAsync(int stepId);
        Task<IEnumerable<WorkFlowSteps>> CreateWorkflowStepsBulkAsync(List<WorkFlowSteps> steps);
        Task<WorkFlowSteps> CreateWorkflowStepAsync(WorkFlowSteps step);
        Task<WorkFlowSteps> UpdateWorkflowStepAsync(WorkFlowSteps step);
        Task<bool> DeleteWorkflowStepAsync(int stepId);
        Task<bool> DeleteWorkflowStepsByWorkflowAsync(int workflowId);
        Task<bool> ToggleWorkflowStepStatusAsync(int stepId, bool isActive);
        Task<IEnumerable<WorkFlowSteps>> GetWorkflowStepsByWorkflowIdAsync(int workflowId);
    }
}