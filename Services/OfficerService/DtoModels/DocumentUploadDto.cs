namespace OfficerService.DtoModels
{
    public class DocumentUploadDto
    {
        // Model classes
        public class DocumentDto
        {
            public string RegistrationNo { get; set; }
            public string DocumentTypeId { get; set; }
            public int CategoryId { get; set; }
            public int ApplicationId { get; set; }
            public List<IFormFile>? Files { get; set; }
            public IFormFile? File { get; set; }
        }

        public class DocumentResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }
    }
}
