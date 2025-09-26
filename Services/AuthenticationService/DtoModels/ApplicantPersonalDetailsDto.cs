namespace AuthenticationService.DtoModels
{
    public class ApplicantPersonalDetailsDto
    {
        public string? LoginId { get; set; }
        public Enums.ApplicantEnums.IdentityProof IDProof { get; set; }
        public string? IDNumber { get; set; }
        public string? IDUpload { get; set; }
        public int? StateId { get; set; }
        public int? CircleId { get; set; }
        public int? DivisionId { get; set; }
        public int? SubDivisionId { get; set; }
        public int? RangeId { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Enums.ApplicantEnums.SourceType SourceType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
