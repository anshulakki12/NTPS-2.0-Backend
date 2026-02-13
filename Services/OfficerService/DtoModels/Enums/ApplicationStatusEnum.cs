namespace OfficerService.DtoModels.Enums
{
    public enum ApplicationStatusEnum : int
    {
        Submitted = 1,
        InspectionCompleted = 2,
        SentForIssue = 3,
        TPIssued = 4,
        TPDownloadedAndProcessCompleted = 5,
        NOCDownloadedAndProcessCompleted = 6,
        Expired = 7,
        NotRecommended = 8,
        SentForPriorApproval = 9,
        Approved = 10
    }

    public static class ApplicationStatusExtensions
    {
        public static string ToDisplayString(this ApplicationStatusEnum status)
        {
            return status switch
            {
                ApplicationStatusEnum.Submitted => "Submitted",
                ApplicationStatusEnum.InspectionCompleted => "Inspection Completed",
                ApplicationStatusEnum.SentForIssue => "Sent for Issue",
                ApplicationStatusEnum.TPIssued => "TP Issued",
                ApplicationStatusEnum.TPDownloadedAndProcessCompleted => "TP Downloaded & Process Completed",
                ApplicationStatusEnum.NOCDownloadedAndProcessCompleted => "NOC Downloaded & Process Completed",
                ApplicationStatusEnum.Expired => "Expired",
                ApplicationStatusEnum.NotRecommended => "Not Recommended",
                ApplicationStatusEnum.SentForPriorApproval => "Sent for Prior Approval",
                ApplicationStatusEnum.Approved => "Approved",
                _ => status.ToString()
            };
        }
    }
}
