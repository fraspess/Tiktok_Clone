import {useEffect, useMemo, useRef} from "react";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import VideoCard from "@/components/feed/VideoCard.tsx";
import type {VideoDto} from "@/types/Video.ts";

interface VideoFeedListProps {
    videos: VideoDto[];
    loadMore: () => void;
    hasNext: boolean;
    isFetching: boolean;
    error: string | null;
    emptyMessage: string;
    loadingMessage: string;
}

const VideoFeedList =
    ({
         videos,
         loadMore,
         hasNext,
         isFetching,
         error,
         emptyMessage,
         loadingMessage,
     }: VideoFeedListProps) => {
    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        loadMore();
    }, []);

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
                {loadingMessage}
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
                {emptyMessage}
            </div>
        );
    }

    return (
        <div
            ref={containerRef}
            className="h-full w-full snap-y snap-mandatory overflow-y-scroll scroll-smooth [-ms-overflow-style:none] [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
        >
            {videos.map((video) => (
                <VideoCard key={video.id} video={video} containerRef={containerRef}/>
            ))}
            {hasNext && <div ref={sentinelRef} className="h-1 w-full"/>}
        </div>
    );
};

export default VideoFeedList;