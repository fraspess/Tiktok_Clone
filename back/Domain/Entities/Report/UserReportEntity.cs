using Domain.Entities.Identity;

namespace Domain.Entities.Report
{
    public class UserReportEntity : ReportEntity
    {
        public Guid UserId { get; set; }
        public UserReportReasons? Reason { get; set; }
        public UserEntity? User { get; set; }
    }
}