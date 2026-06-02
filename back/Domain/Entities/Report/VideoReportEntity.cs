using Domain.Entities.Video;

namespace Domain.Entities.Report
{
    public class VideoReportEntity : ReportEntity
    {
        public Guid VideoId { get; set; }
        public VideoReportReasons? Reason { get; set; }
        public VideoEntity? Video { get; set; }
    }
}