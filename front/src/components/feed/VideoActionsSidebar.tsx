import {useState} from "react";
import {Bookmark, Flag, Heart, MessageCircle, Plus, Share2} from "lucide-react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import type {VideoDto} from "@/types/Video.ts";
import ReportVideoDialog from "@/components/feed/ReportVideoDialog.tsx";
import CommentsDialog from "@/components/feed/CommentsDialog.tsx";
import {formatCount} from "@/lib/utils.ts";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {
    useFavoriteVideoMutation,
    useLikeVideoMutation,
    useUnfavoriteVideoMutation,
    useUnlikeVideoMutation,
} from "@/store/apis/videoApi.ts";

interface VideoActionsSidebarProps {
    video: VideoDto;
}

const VideoActionsSidebar = ({video}: VideoActionsSidebarProps) => {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const isAuth = useAppSelector((s) => s.auth.isAuth);

    const [isLiked, setIsLiked] = useState(video.isLiked);
    const [likeCount, setLikeCount] = useState(video.likeCount);
    const [isSaved, setIsSaved] = useState(video.isFavorited);
    const [saveCount, setSaveCount] = useState(video.favoriteCount);
    const [commentsCount, setCommentsCount] = useState(video.commentsCount);
    const [isReportOpen, setIsReportOpen] = useState(false);
    const [isCommentsOpen, setIsCommentsOpen] = useState(false);

    const [likeVideo] = useLikeVideoMutation();
    const [unlikeVideo] = useUnlikeVideoMutation();
    const [favoriteVideo] = useFavoriteVideoMutation();
    const [unfavoriteVideo] = useUnfavoriteVideoMutation();

    const requireAuth = () => {
        if (isAuth) return true;
        dispatch(openModal());
        return false;
    };

    const toggleLike = async () => {
        if (!requireAuth()) return;

        const nextLiked = !isLiked;
        setIsLiked(nextLiked);
        setLikeCount((prev) => (nextLiked ? prev + 1 : prev - 1));

        try {
            if (nextLiked) {
                await likeVideo(video.id).unwrap();
            } else {
                await unlikeVideo(video.id).unwrap();
            }
        } catch {
            setIsLiked(!nextLiked);
            setLikeCount((prev) => (nextLiked ? prev - 1 : prev + 1));
            toast.error(t("feed.likeError"));
        }
    };

    const toggleSave = async () => {
        if (!requireAuth()) return;

        const nextSaved = !isSaved;
        setIsSaved(nextSaved);
        setSaveCount((prev) => (nextSaved ? prev + 1 : prev - 1));

        try {
            if (nextSaved) {
                await favoriteVideo(video.id).unwrap();
            } else {
                await unfavoriteVideo(video.id).unwrap();
            }
        } catch {
            setIsSaved(!nextSaved);
            setSaveCount((prev) => (nextSaved ? prev - 1 : prev + 1));
            toast.error(t("feed.saveError"));
        }
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

            <button
                type="button"
                onClick={() => setIsCommentsOpen(true)}
                className="flex flex-col items-center gap-1 text-white"
            >
                <span className="flex h-11 w-11 items-center justify-center rounded-full bg-black/40 backdrop-blur-sm transition-transform active:scale-90">
                    <MessageCircle size={24}/>
                </span>
                <span className="text-xs font-medium text-black dark:text-white">{formatCount(commentsCount)}</span>
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

            <CommentsDialog
                videoId={video.id}
                open={isCommentsOpen}
                onOpenChange={setIsCommentsOpen}
                onCommentsCountChange={(delta) => setCommentsCount((prev) => prev + delta)}
            />
        </div>
    );
};

export default VideoActionsSidebar;
