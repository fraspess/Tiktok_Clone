import {useEffect, useMemo, useRef} from "react";
import {useTranslation} from "react-i18next";
import {Heart, Play} from "lucide-react";
import {useInfiniteUserVideos} from "@/hooks/useInfiniteUserVideos.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";

interface ProfileVideoGridProps {
    userId: string;
}

function formatCount(count: number): string {
    if (count >= 1_000_000) return `${(count / 1_000_000).toFixed(1)}M`;
    if (count >= 1_000) return `${(count / 1_000).toFixed(1)}K`;
    return `${count}`;
}

const ProfileVideoGrid = ({userId}: ProfileVideoGridProps) => {
    const {t} = useTranslation();
    const sentinelRef = useRef<HTMLDivElement>(null);
    const {videos, loadMore, hasNext, isFetching, error} = useInfiniteUserVideos(userId);

    useEffect(() => {
        loadMore();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [userId]);

    const sentinelOptions = useMemo(() => ({rootMargin: "600px"}), []);
    const isSentinelVisible = useIntersectionObserver(sentinelRef, sentinelOptions);

    useEffect(() => {
        if (isSentinelVisible && hasNext && !isFetching) {
            loadMore();
        }
    }, [isSentinelVisible, hasNext, isFetching, loadMore]);

    if (videos.length === 0 && isFetching) {
        return (
            <div className="flex w-full items-center justify-center py-16 text-muted-foreground">
                {t("profile.videosLoading")}
            </div>
        );
    }

    if (videos.length === 0 && error) {
        return (
            <div className="flex w-full items-center justify-center py-16 text-muted-foreground">
                {error}
            </div>
        );
    }

    if (videos.length === 0) {
        return (
            <div className="flex w-full items-center justify-center py-16 text-muted-foreground">
                {t("profile.videosEmpty")}
            </div>
        );
    }

    return (
        <div className="px-2 pb-8 sm:px-4">
            <div className="grid grid-cols-2 gap-1 sm:grid-cols-3 sm:gap-2 md:grid-cols-4 lg:grid-cols-5">
                {videos.map((video) => (
                    <div
                        key={video.id}
                        className="group relative aspect-[9/16] overflow-hidden rounded-md bg-neutral-200 dark:bg-neutral-900"
                    >
                        {video.thumbnailUrl ? (
                            <img
                                src={video.thumbnailUrl}
                                alt={video.description}
                                loading="lazy"
                                className="h-full w-full object-cover"
                            />
                        ) : (
                            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                                <Play className="h-8 w-8"/>
                            </div>
                        )}

                        <div className="pointer-events-none absolute inset-x-0 bottom-0 flex items-center gap-1 bg-gradient-to-t from-black/70 to-transparent px-2 py-2 text-xs font-medium text-white">
                            <Heart className="h-3.5 w-3.5 fill-white"/>
                            {formatCount(video.likeCount)}
                        </div>
                    </div>
                ))}
            </div>
            {hasNext && <div ref={sentinelRef} className="h-1 w-full"/>}
        </div>
    );
};

export default ProfileVideoGrid;
