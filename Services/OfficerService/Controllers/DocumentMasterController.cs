using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentMasterController : ControllerBase
    {
        private readonly IDocumentMasterRepository _repository;
        private readonly ILogger<DocumentMasterController> _logger;

        public DocumentMasterController(
            IDocumentMasterRepository repository,
            ILogger<DocumentMasterController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("by-state/{stateCode}")]
        public async Task<IActionResult> GetByStateCode(int stateCode)
        {
            try
            {
                var documents = await _repository.GetByStateCode(stateCode);
                return Ok(new BaseResponse<IEnumerable<DocumentMaster>>
                {
                    Success = true,
                    Data = documents
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching documents for state code: {StateCode}", stateCode);
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while fetching documents"
                });
            }
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultDocuments()
        {
            try
            {
                var documents = await _repository.GetDefaultDocuments();
                return Ok(new BaseResponse<IEnumerable<DocumentMaster>>
                {
                    Success = true,
                    Data = documents
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching default documents");
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while fetching default documents"
                });
            }
        }

        [HttpPost("create-or-update")]
        public async Task<IActionResult> CreateOrUpdate([FromBody] DocumentRequest request)
        {
            try
            {
                if (request == null || request.Documents == null || !request.Documents.Any())
                {
                    return BadRequest(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                // Get officerId from request or fallback
                var officerId = request.OfficerId ?? "System";

                var documents = request.Documents.Select(d => new DocumentMaster
                {
                    StateCode = request.StateCode,
                    StateName = request.StateName,
                    DocumentTypeId = d.DocumentTypeId,
                    DocumentTypeName = d.DocumentTypeName,
                    IsActive = d.IsActive,
                    IsMandatory = d.IsMandatory,
                    CreatedBy = officerId,
                    ModifiedBy = officerId
                });

                await _repository.CreateOrUpdateBulk(documents);

                return Ok(new BaseResponse<string>
                {
                    Success = true,
                    Message = "Documents saved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving documents for state code: {StateCode}", request?.StateCode);
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while saving documents"
                });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DocumentUpdateRequest request)
        {
            try
            {
                var document = await _repository.GetById(id);
                if (document == null)
                {
                    return NotFound(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Document not found"
                    });
                }

                document.IsActive = request.IsActive;
                document.IsMandatory = request.IsMandatory;
                document.ModifiedBy = request.OfficerId ?? "System";
                document.ModifiedDate = DateTime.UtcNow;

                await _repository.Update(document);

                return Ok(new BaseResponse<string>
                {
                    Success = true,
                    Message = "Document updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID: {Id}", id);
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating document"
                });
            }
        }
    }

    internal class BaseResponse<T>
    {
        public bool Success { get; set; }
        public IEnumerable<DocumentMaster> Data { get; set; }
        public string Message { get; internal set; }
    }
}
