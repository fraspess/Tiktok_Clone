using Domain.Entities.Comment;

namespace Domain.Entities.Report
{
    public class CommentReportEntity : ReportEntity
    {
        public Guid CommentId { get; set; }
        public CommentReportReasons? Reason { get; set; }
        public CommentEntity? Comment { get; set; }
    }
}