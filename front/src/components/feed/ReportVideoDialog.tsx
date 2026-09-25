import ReportContentDialog from "@/components/feed/ReportContentDialog.tsx";

export default function ReportVideoDialog({videoId, ...props}: {
    videoId: string; open: boolean; onOpenChange: (open: boolean) => void;
}) {
    return <ReportContentDialog contentId={videoId} contentType="Video" {...props}/>;
}
