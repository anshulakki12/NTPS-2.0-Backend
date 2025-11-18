using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;
using OfficerService.Repositories;
using System.ComponentModel.DataAnnotations;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowMasterController : ControllerBase
    {
        private readonly IWorkflowRepository _repository;
        private readonly AppDbContext _context;

        public WorkflowMasterController(
            IWorkflowRepository repository,
            AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // Master Level Endpoints
        [HttpGet("GetAllMasterLevels")]
        public async Task<IActionResult> GetAllMasterLevels()
        {
            try
            {
                var levels = await _repository.GetAllMasterLevelsAsync();
                var result = levels.Select(l => new
                {
                    level_ID = l.LevelId,
                    level_Name = l.LevelName,
                    is_Active = l.IsActive
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Role Endpoints
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _repository.GetAllRolesAsync();
                var result = roles.Select(r => new
                {
                    role_ID = r.RoleId,
                    role_Name = r.RoleName,
                    is_Active = r.IsActive
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Designation Endpoints
        [HttpGet("GetAllDesignations")]
        public async Task<IActionResult> GetAllDesignations()
        {
            try
            {
                var designations = await _repository.GetAllDesignationsAsync();
                var result = designations.Select(d => new
                {
                    designation_ID = d.DesignationId,
                    designation_Name = d.DesignationName,
                    is_Active = d.IsActive
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // State Endpoints
        [HttpGet("GetAllStates")]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var states = await _repository.GetAllStatesAsync();
                var result = states.Select(s => new
                {
                    stateId = s.Id,
                    stateName = s.StName,
                    stCode = s.StCode,
                    stUt = s.StUt,
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Master Workflow Endpoints
        [HttpGet("GetAllMasterWorkflows")]
        public async Task<IActionResult> GetAllMasterWorkflows()
        {
            try
            {
                var workflows = await _repository.GetAllMasterWorkflowsAsync();
                var result = workflows.Select(wf => new MasterWorkflowResponseDto
                {
                    WorkFlow_ID = wf.WorkFlowId,
                    WorkFlow_Name = wf.WorkFlowName,
                    StateID = wf.StateId,
                    StateName = wf.State?.StName,
                    StCode = wf.State?.StCode ?? 0,
                    Is_Default = wf.IsDefault,
                    Is_Active = wf.IsActive,
                    CreatedOn = wf.CreatedOn
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateMasterWorkflow")]
        public async Task<IActionResult> CreateMasterWorkflow([FromBody] CreateMasterWorkflowDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if workflow name already exists for this state
                var existingWorkflow = await _context.MasterWorkFlows
                    .FirstOrDefaultAsync(wf => wf.WorkFlowName == createDto.WorkFlow_Name && wf.StateId == createDto.StateID);

                if (existingWorkflow != null)
                {
                    return Conflict($"Workflow with name '{createDto.WorkFlow_Name}' already exists in this state.");
                }

                var workflow = new MasterWorkFlow
                {
                    WorkFlowName = createDto.WorkFlow_Name,
                    StateId = createDto.StateID,
                    IsDefault = createDto.Is_Default,
                    IsActive = createDto.Is_Active,
                    CreatedOn = DateTime.Now
                };

                var result = await _repository.CreateMasterWorkflowAsync(workflow);

                // If this is set as default, update other workflows
                if (createDto.Is_Default)
                {
                    await _repository.SetDefaultWorkflowAsync(result.WorkFlowId);
                }

                // Include State in the response
                await _context.Entry(result).Reference(wf => wf.State).LoadAsync();

                var response = new MasterWorkflowResponseDto
                {
                    WorkFlow_ID = result.WorkFlowId,
                    WorkFlow_Name = result.WorkFlowName,
                    StateID = result.StateId,
                    StateName = result.State?.StName,
                    StCode = result.State?.StCode ?? 0,
                    Is_Default = result.IsDefault,
                    Is_Active = result.IsActive,
                    CreatedOn = result.CreatedOn
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateMasterWorkflow")]
        public async Task<IActionResult> UpdateMasterWorkflow([FromBody] UpdateMasterWorkflowDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingWorkflow = await _repository.GetMasterWorkflowByIdAsync(updateDto.WorkFlow_ID);
                if (existingWorkflow == null)
                    return NotFound($"Master Workflow with ID {updateDto.WorkFlow_ID} not found");

                // Check if workflow name already exists (excluding current workflow)
                var duplicateWorkflow = await _context.MasterWorkFlows
                    .FirstOrDefaultAsync(wf => wf.WorkFlowName == updateDto.WorkFlow_Name
                                            && wf.StateId == updateDto.StateID
                                            && wf.WorkFlowId != updateDto.WorkFlow_ID);

                if (duplicateWorkflow != null)
                {
                    return Conflict($"Workflow with name '{updateDto.WorkFlow_Name}' already exists in this state.");
                }

                existingWorkflow.WorkFlowName = updateDto.WorkFlow_Name;
                existingWorkflow.StateId = updateDto.StateID;
                existingWorkflow.IsDefault = updateDto.Is_Default;
                existingWorkflow.IsActive = updateDto.Is_Active;

                // If this is set as default, update other workflows
                if (updateDto.Is_Default)
                {
                    await _repository.SetDefaultWorkflowAsync(existingWorkflow.WorkFlowId);
                }

                var result = await _repository.UpdateMasterWorkflowAsync(existingWorkflow);

                // Include State in the response
                await _context.Entry(result).Reference(wf => wf.State).LoadAsync();

                var response = new MasterWorkflowResponseDto
                {
                    WorkFlow_ID = result.WorkFlowId,
                    WorkFlow_Name = result.WorkFlowName,
                    StateID = result.StateId,
                    StateName = result.State?.StName,
                    StCode = result.State?.StCode ?? 0,
                    Is_Default = result.IsDefault,
                    Is_Active = result.IsActive,
                    CreatedOn = result.CreatedOn
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteMasterWorkflow/{workflowId}")]
        public async Task<IActionResult> DeleteMasterWorkflow(int workflowId)
        {
            try
            {
                var result = await _repository.DeleteMasterWorkflowAsync(workflowId);
                if (!result)
                    return NotFound($"Master Workflow with ID {workflowId} not found");

                return Ok(new { message = "Master Workflow deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ToggleMasterWorkflowStatus/{workflowId}/{isActive}")]
        public async Task<IActionResult> ToggleMasterWorkflowStatus(int workflowId, bool isActive)
        {
            try
            {
                var result = await _repository.ToggleMasterWorkflowStatusAsync(workflowId, isActive);
                if (!result)
                    return NotFound($"Master Workflow with ID {workflowId} not found");

                return Ok(new { message = $"Master Workflow {(isActive ? "activated" : "deactivated")} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Workflow Steps Endpoints
        [HttpGet("GetAllWorkflowSteps")]
        public async Task<IActionResult> GetAllWorkflowSteps()
        {
            try
            {
                var steps = await _repository.GetAllWorkflowStepsAsync();
                var result = steps.Select(ws => new WorkflowStepResponseDto
                {
                    Step_ID = ws.StepId,
                    WorkFlow_ID = ws.WorkFlowId,
                    WorkFlow_Name = ws.MasterWorkFlow?.WorkFlowName,
                    Level_ID = ws.LevelId,
                    Level_Name = ws.MasterLevel?.LevelName,
                    Step_Order = ws.StepOrder,
                    Role_ID = ws.RoleId,
                    Role_Name = ws.Role?.RoleName, // Fixed: Now using navigation property
                    Designation_ID = ws.DesignationId,
                    Designation_Name = ws.Designation?.DesignationName, // Fixed: Now using navigation property
                    IsFinal_Step = ws.IsFinalStep,
                    Created_On = ws.CreatedOn
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateWorkflowStep")]
        public async Task<IActionResult> CreateWorkflowStep([FromBody] CreateWorkflowStepDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var step = new WorkFlowSteps
                {
                    WorkFlowId = createDto.WorkFlow_ID,
                    LevelId = createDto.Level_ID,
                    StepOrder = createDto.Step_Order,
                    RoleId = createDto.Role_ID,
                    DesignationId = createDto.Designation_ID,
                    IsFinalStep = createDto.IsFinal_Step,
                    CreatedOn = DateTime.Now
                };

                var result = await _repository.CreateWorkflowStepAsync(step);

                // Load navigation properties
                await _context.Entry(result)
                    .Reference(ws => ws.MasterWorkFlow)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(ws => ws.MasterLevel)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(ws => ws.Role)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(ws => ws.Designation)
                    .LoadAsync();

                var response = new WorkflowStepResponseDto
                {
                    Step_ID = result.StepId,
                    WorkFlow_ID = result.WorkFlowId,
                    WorkFlow_Name = result.MasterWorkFlow?.WorkFlowName,
                    Level_ID = result.LevelId,
                    Level_Name = result.MasterLevel?.LevelName,
                    Step_Order = result.StepOrder,
                    Role_ID = result.RoleId,
                    Role_Name = result.Role?.RoleName,
                    Designation_ID = result.DesignationId,
                    Designation_Name = result.Designation?.DesignationName,
                    IsFinal_Step = result.IsFinalStep,
                    Created_On = result.CreatedOn
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateWorkflowStepsBulk")]
        public async Task<IActionResult> CreateWorkflowStepsBulk([FromBody] List<CreateWorkflowStepDto> createDtos)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var steps = createDtos.Select(dto => new WorkFlowSteps
                {
                    WorkFlowId = dto.WorkFlow_ID,
                    LevelId = dto.Level_ID,
                    StepOrder = dto.Step_Order,
                    RoleId = dto.Role_ID,
                    DesignationId = dto.Designation_ID,
                    IsFinalStep = dto.IsFinal_Step,
                    CreatedOn = DateTime.Now
                }).ToList();

                var result = await _repository.CreateWorkflowStepsBulkAsync(steps);

                // Load navigation properties
                foreach (var step in result)
                {
                    await _context.Entry(step)
                        .Reference(ws => ws.MasterWorkFlow)
                        .LoadAsync();
                    await _context.Entry(step)
                        .Reference(ws => ws.MasterLevel)
                        .LoadAsync();
                    await _context.Entry(step)
                        .Reference(ws => ws.Role)
                        .LoadAsync();
                    await _context.Entry(step)
                        .Reference(ws => ws.Designation)
                        .LoadAsync();
                }

                var response = result.Select(ws => new WorkflowStepResponseDto
                {
                    Step_ID = ws.StepId,
                    WorkFlow_ID = ws.WorkFlowId,
                    WorkFlow_Name = ws.MasterWorkFlow?.WorkFlowName,
                    Level_ID = ws.LevelId,
                    Level_Name = ws.MasterLevel?.LevelName,
                    Step_Order = ws.StepOrder,
                    Role_ID = ws.RoleId,
                    Role_Name = ws.Role?.RoleName,
                    Designation_ID = ws.DesignationId,
                    Designation_Name = ws.Designation?.DesignationName,
                    IsFinal_Step = ws.IsFinalStep,
                    Created_On = ws.CreatedOn
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateWorkflowStep")]
        public async Task<IActionResult> UpdateWorkflowStep([FromBody] UpdateWorkflowStepDto updateDto)
        {
            try
            {
                var existingStep = await _repository.GetWorkflowStepByIdAsync(updateDto.Step_ID);
                if (existingStep == null)
                    return NotFound($"Workflow Step with ID {updateDto.Step_ID} not found");

                existingStep.WorkFlowId = updateDto.WorkFlow_ID;
                existingStep.LevelId = updateDto.Level_ID;
                existingStep.StepOrder = updateDto.Step_Order;
                existingStep.RoleId = updateDto.Role_ID;
                existingStep.DesignationId = updateDto.Designation_ID;
                existingStep.IsFinalStep = updateDto.IsFinal_Step;

                var result = await _repository.UpdateWorkflowStepAsync(existingStep);

                // Load navigation properties
                await _context.Entry(result)
                    .Reference(ws => ws.MasterWorkFlow)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(ws => ws.MasterLevel)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(ws => ws.Role)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(ws => ws.Designation)
                    .LoadAsync();

                var response = new WorkflowStepResponseDto
                {
                    Step_ID = result.StepId,
                    WorkFlow_ID = result.WorkFlowId,
                    WorkFlow_Name = result.MasterWorkFlow?.WorkFlowName,
                    Level_ID = result.LevelId,
                    Level_Name = result.MasterLevel?.LevelName,
                    Step_Order = result.StepOrder,
                    Role_ID = result.RoleId,
                    Role_Name = result.Role?.RoleName,
                    Designation_ID = result.DesignationId,
                    Designation_Name = result.Designation?.DesignationName,
                    IsFinal_Step = result.IsFinalStep,
                    Created_On = result.CreatedOn
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[HttpDelete("DeleteWorkflowStep/{stepId}")]
        //public async Task<IActionResult> DeleteWorkflowStep(int stepId)
        //{
        //    try
        //    {
        //        var result = await _repository.DeleteWorkflowStepAsync(stepId);
        //        if (!result)
        //            return NotFound($"Workflow Step with ID {stepId} not found");

        //        return Ok(new { message = "Workflow Step deleted successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        [HttpDelete("DeleteWorkflowStepsByWorkflow/{workflowId}")]
        public async Task<IActionResult> DeleteWorkflowStepsByWorkflow(int workflowId)
        {
            try
            {
                var steps = await _context.WorkFlowSteps
                    .Where(ws => ws.WorkFlowId == workflowId)
                    .ToListAsync();

                if (steps.Any())
                {
                    _context.WorkFlowSteps.RemoveRange(steps);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Workflow steps deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ToggleWorkflowStepStatus/{stepId}/{isActive}")]
        public async Task<IActionResult> ToggleWorkflowStepStatus(int stepId, bool isActive)
        {
            try
            {
                // Note: This would require adding IsActive field to WorkFlowSteps table
                // For now, we'll return not implemented
                return StatusCode(501, "Toggle status not implemented for workflow steps");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetWorkflowStepsByWorkflow/{workflowId}")]
        public async Task<IActionResult> GetWorkflowStepsByWorkflow(int workflowId)
        {
            try
            {
                var steps = await _repository.GetWorkflowStepsByWorkflowIdAsync(workflowId);
                var result = steps.Select(ws => new WorkflowStepResponseDto
                {
                    Step_ID = ws.StepId,
                    WorkFlow_ID = ws.WorkFlowId,
                    WorkFlow_Name = ws.MasterWorkFlow?.WorkFlowName,
                    Level_ID = ws.LevelId,
                    Level_Name = ws.MasterLevel?.LevelName,
                    Step_Order = ws.StepOrder,
                    Role_ID = ws.RoleId,
                    Role_Name = ws.Role?.RoleName,
                    Designation_ID = ws.DesignationId,
                    Designation_Name = ws.Designation?.DesignationName,
                    IsFinal_Step = ws.IsFinalStep,
                    Created_On = ws.CreatedOn
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // DTO Classes (keep the same as before)
    public class CreateMasterWorkflowDto
    {
        [Required]
        [StringLength(100)]
        public string WorkFlow_Name { get; set; }

        [Required]
        public int StateID { get; set; }

        public bool Is_Default { get; set; } = false;

        public bool Is_Active { get; set; } = true;
    }

    public class UpdateMasterWorkflowDto
    {
        [Required]
        public int WorkFlow_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string WorkFlow_Name { get; set; }

        [Required]
        public int StateID { get; set; }

        public bool Is_Default { get; set; }

        public bool Is_Active { get; set; }
    }

    public class MasterWorkflowResponseDto
    {
        public int WorkFlow_ID { get; set; }
        public string WorkFlow_Name { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int StCode { get; set; }
        public bool Is_Default { get; set; }
        public bool Is_Active { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CreateWorkflowStepDto
    {
        [Required]
        public int WorkFlow_ID { get; set; }

        [Required]
        public int Level_ID { get; set; }

        [Required]
        [Range(1, 100)]
        public int Step_Order { get; set; }

        [Required]
        public int Role_ID { get; set; }

        [Required]
        public int Designation_ID { get; set; }

        public bool IsFinal_Step { get; set; } = false;
    }

    public class UpdateWorkflowStepDto
    {
        [Required]
        public int Step_ID { get; set; }

        [Required]
        public int WorkFlow_ID { get; set; }

        [Required]
        public int Level_ID { get; set; }

        [Required]
        [Range(1, 100)]
        public int Step_Order { get; set; }

        [Required]
        public int Role_ID { get; set; }

        [Required]
        public int Designation_ID { get; set; }

        public bool IsFinal_Step { get; set; }
    }

    public class WorkflowStepResponseDto
    {
        public int Step_ID { get; set; }
        public int WorkFlow_ID { get; set; }
        public string WorkFlow_Name { get; set; }
        public int Level_ID { get; set; }
        public string Level_Name { get; set; }
        public int Step_Order { get; set; }
        public int Role_ID { get; set; }
        public string Role_Name { get; set; }
        public int Designation_ID { get; set; }
        public string Designation_Name { get; set; }
        public bool IsFinal_Step { get; set; }
        public DateTime Created_On { get; set; }
    }
}