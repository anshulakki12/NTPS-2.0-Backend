using OfficerService.Models;
using System.ComponentModel.DataAnnotations;

namespace OfficerService.DtoModels
{
    public class OfficerMastersDTO
    {
        // Request DTOs for Transport Mode
        public class TransportModeCreateRequest
        {
            [Required(ErrorMessage = "Transport Mode is required")]
            [StringLength(100, ErrorMessage = "Transport Mode cannot exceed 100 characters.")]
            public string TransportMode { get; set; } = null!;

            public bool IsActive { get; set; } = true;

            [Required(ErrorMessage = "Officer ID is required")]
            public string OfficerId { get; set; } = null!;
        }

        public class TransportModeUpdateRequest
        {
            [Required(ErrorMessage = "Transport Mode is required")]
            [StringLength(100, ErrorMessage = "Transport Mode cannot exceed 100 characters.")]
            public string TransportMode { get; set; } = null!;

            public bool IsActive { get; set; }

            [Required(ErrorMessage = "Officer ID is required")]
            public string OfficerId { get; set; } = null!;
        }

        public class StatusToggleRequest
        {
            public bool IsActive { get; set; }

            [Required(ErrorMessage = "Officer ID is required")]
            public string OfficerId { get; set; } = null!;
        }

        public class BaseResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }

    }
}
