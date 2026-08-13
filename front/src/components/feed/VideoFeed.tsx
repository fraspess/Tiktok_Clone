import {useEffect, useMemo, useRef} from "react";
import {useTranslation} from "react-i18next";
import {useInfiniteFyp} from "@/hooks/useInfiniteFyp.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import VideoCard from "@/components/feed/VideoCard.tsx";

const VideoFeed = () => {
    const {t} = useTranslation();
    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);
    const {videos, loadMore, hasNext, isFetching, error} = useInfiniteFyp(5);

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
                {t("feed.loading")}
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
                {t("feed.empty")}
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

export default VideoFeed;
