import {useEffect, useMemo, useRef} from "react";
import {useNavigate} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Play} from "lucide-react";
import {useInfiniteUserVideos} from "@/hooks/useInfiniteUserVideos.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import {formatCount} from "@/lib/utils.ts";

interface ProfileVideoGridProps {
    userId: string | undefined;
    username: string;
}


const ProfileVideoGrid = ({userId, username}: ProfileVideoGridProps) => {
    const {t} = useTranslation();
    const navigate = useNavigate();
    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);
    const {videos, loadMore, hasNext, isFetching, error} = useInfiniteUserVideos(userId, 12);

    useEffect(() => {
        if (userId) {
            loadMore();
        }
    }, [userId]);

    const sentinelOptions = useMemo(
        () => ({root: containerRef, rootMargin: "600px"}),
        [containerRef]
    );
    const isSentinelVisible = useIntersectionObserver(sentinelRef, sentinelOptions);

    useEffect(() => {
        if (isSentinelVisible && hasNext && !isFetching) {
            loadMore();
        }
    }, [isSentinelVisible, hasNext, isFetching, loadMore]);

    if (videos.length === 0 && isFetching) {
        return (
            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                {t("profile.loadingVideos")}
            </div>
        );
    }

    if (videos.length === 0 && error) {
        return (
            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                {error}
            </div>
        );
    }

    if (videos.length === 0) {
        return (
            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                {t("profile.emptyVideos")}
            </div>
        );
    }

    return (
        <div ref={containerRef} className="h-full w-full overflow-y-auto px-4 pb-8">
            <div
                className="grid gap-2 grid-cols-2 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6 xl:grid-cols-7 2xl:grid-cols-8"
            >
                {videos.map((video) => (
                    <div
                        key={video.id}
                        role="button"
                        tabIndex={0}
                        onClick={() =>
                            navigate(`/@${username}/video/${video.id}`, {
                                state: {userId},
                            })
                        }
                        onKeyDown={(e) => {
                            if (e.key === "Enter" || e.key === " ") {
                                e.preventDefault();
                                navigate(`/@${username}/video/${video.id}`, {
                                    state: {userId},
                                });
                            }
                        }}
                        className="group relative aspect-[3/4] cursor-pointer overflow-hidden rounded-md bg-neutral-800"
                    >
                        {video.thumbnailUrl ? (
                            <img
                                src={video.thumbnailUrl}
                                alt={video.description || video.id}
                                className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
                            />
                        ) : (
                            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                                <Play size={28}/>
                            </div>
                        )}
                        <div
                            className="absolute inset-x-0 bottom-0 flex items-center gap-1 bg-gradient-to-t from-black/70 to-transparent px-2 py-1.5 text-xs font-medium text-white">
                            <Play size={12} className="fill-white"/>
                            {formatCount(video.viewCount)}
                        </div>
                    </div>
                ))}
            </div>
            {hasNext && <div ref={sentinelRef} className="h-1 w-full"/>}
        </div>
    );
};

export default ProfileVideoGrid;
