import {useState} from "react";
import {Bookmark, Flag, Heart, MessageCircle, Plus, Share2} from "lucide-react";
import {useTranslation} from "react-i18next";
import type {VideoDto} from "@/types/Video.ts";
import ReportVideoDialog from "@/components/feed/ReportVideoDialog.tsx";
import {formatCount} from "@/lib/utils.ts";

interface VideoActionsSidebarProps {
    video: VideoDto;
}

const VideoActionsSidebar = ({video}: VideoActionsSidebarProps) => {
    const {t} = useTranslation();
    const [isLiked, setIsLiked] = useState(video.isLiked);
    const [likeCount, setLikeCount] = useState(video.likeCount);
    const [isSaved, setIsSaved] = useState(video.isFavorited);
    const [saveCount, setSaveCount] = useState(video.favoriteCount);
    const [isReportOpen, setIsReportOpen] = useState(false);

    const toggleLike = () => {
        setLikeCount((prev) => (isLiked ? prev - 1 : prev + 1));
        setIsLiked((prev) => !prev);
    };

    const toggleSave = () => {
        setSaveCount((prev) => (isSaved ? prev - 1 : prev + 1));
        setIsSaved((prev) => !prev);
    };

    return (
        <div className="flex flex-col items-center gap-5">

            <div className="relative mb-1">
                <div className="h-11 w-11 overflow-hidden rounded-full border-2 border-white bg-neutral-700">
                    {video.author?.avatar?.small ? (
                        <img
                            src={video.author.avatar.small}
                            alt={video.author.username}
                            className="h-full w-full object-cover"
                        />
                    ) : (
                        <div className="flex h-full w-full items-center justify-center text-sm font-semibold text-black dark:text-white">
                            {video.author?.username?.[0]?.toUpperCase() ?? "?"}
                        </div>
                    )}
                </div>
                <button
                    type="button"
                    className="absolute -bottom-1.5 left-1/2 flex h-5 w-5 -translate-x-1/2 items-center justify-center rounded-full bg-red-500 text-white"
                >
                    <Plus size={12} strokeWidth={3}/>
                </button>
            </div>

            <button
                type="button"
                onClick={toggleLike}
                className="flex flex-col items-center gap-1 text-white"
            >
                <span className="flex h-11 w-11 items-center justify-center rounded-full bg-black/40 backdrop-blur-sm transition-transform active:scale-90">
                    <Heart size={24} className={isLiked ? "fill-red-500 text-red-500" : "text-white"}/>
                </span>
                <span className="text-xs font-medium text-black dark:text-white">{formatCount(likeCount)}</span>
            </button>

            <button type="button" className="flex flex-col items-center gap-1 text-white">
                <span className="flex h-11 w-11 items-center justify-center rounded-full bg-black/40 backdrop-blur-sm transition-transform active:scale-90">
                    <MessageCircle size={24}/>
                </span>
                <span className="text-xs font-medium text-black dark:text-white">{formatCount(video.commentsCount)}</span>
            </button>

            <button
                type="button"
                onClick={toggleSave}
                className="flex flex-col items-center gap-1 text-white"
            >
                <span className="flex h-11 w-11 items-center justify-center rounded-full bg-black/40 backdrop-blur-sm transition-transform active:scale-90">
                    <Bookmark size={24} className={isSaved ? "fill-white" : ""}/>
                </span>
                <span className="text-xs font-medium text-black dark:text-white">{formatCount(saveCount)}</span>
            </button>

            <button type="button" className="flex flex-col items-center gap-1 text-white">
                <span className="flex h-11 w-11 items-center justify-center rounded-full bg-black/40 backdrop-blur-sm transition-transform active:scale-90">
                    <Share2 size={24}/>
                </span>
                <span className="text-xs font-medium text-black dark:text-white">{t("feed.share")}</span>
            </button>

            <button
                type="button"
                onClick={() => setIsReportOpen(true)}
                className="flex flex-col items-center gap-1 text-white"
            >
                <span className="flex h-11 w-11 items-center justify-center rounded-full bg-black/40 backdrop-blur-sm transition-transform active:scale-90">
                    <Flag size={22}/>
                </span>
                <span className="text-xs font-medium text-black dark:text-white">{t("report.reportButton")}</span>
            </button>

            <ReportVideoDialog
                videoId={video.id}
                open={isReportOpen}
                onOpenChange={setIsReportOpen}
            />
        </div>
    );
};

export default VideoActionsSidebar;
