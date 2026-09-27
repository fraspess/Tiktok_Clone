import {useCallback, useEffect, useMemo, useRef, useState} from "react";
import {Link} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Button} from "@/components/ui/button.tsx";
import {useLazyGetVideoCollectionQuery} from "@/store/apis/videoApi.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import type {VideoDto} from "@/types/Video.ts";

export default function ProfileCollectionGrid({userId, username, kind}: {userId: string; username: string; kind: "liked" | "reposts"}) {
    const {t} = useTranslation();
    const [trigger, {isFetching}] = useLazyGetVideoCollectionQuery();
    const [videos, setVideos] = useState<VideoDto[]>([]);
    const [hasNext, setHasNext] = useState(true);
    const [error, setError] = useState(false);
    const nextPageRef = useRef(1);
    const loadedIdsRef = useRef(new Set<string>());
    const loadingRef = useRef(false);
    const initialLoadStartedRef = useRef(false);
    const sentinelHandledRef = useRef(false);
    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);

    const loadMore = useCallback(async () => {
        if (loadingRef.current || !hasNext) return;
        loadingRef.current = true;
        setError(false);
        try {
            const response = await trigger({userId, kind, pageNumber: nextPageRef.current, pageSize: 24}).unwrap();
            const newVideos = response.data.items.filter((video) => !loadedIdsRef.current.has(video.id));
            newVideos.forEach((video) => loadedIdsRef.current.add(video.id));
            setVideos((current) => [...current, ...newVideos]);
            setHasNext(response.data.metadata.hasNext);
            nextPageRef.current += 1;
        } catch {
            setError(true);
        } finally {
            loadingRef.current = false;
        }
    }, [hasNext, kind, trigger, userId]);

    useEffect(() => {
        if (!initialLoadStartedRef.current) {
            initialLoadStartedRef.current = true;
            void loadMore();
        }
    }, [loadMore]);
    const sentinelOptions = useMemo(() => ({root: containerRef, rootMargin: "600px"}), []);
    const isSentinelVisible = useIntersectionObserver(sentinelRef, sentinelOptions);
    useEffect(() => {
        if (!isSentinelVisible) {
            sentinelHandledRef.current = false;
            return;
        }
        if (sentinelHandledRef.current) return;

        sentinelHandledRef.current = true;
        if (videos.length > 0 && !isFetching) void loadMore();
    }, [isFetching, isSentinelVisible, loadMore, videos.length]);

    return <div ref={containerRef} className="h-full overflow-y-auto px-4 pb-8">
        {videos.length === 0 && isFetching ? <p role="status">{t("profile.loadingVideos")}</p> : error && videos.length === 0 ?
            <div role="alert">{t("profile.videosLoadError")} <Button onClick={() => void loadMore()}>{t("chat.privacy.retry")}</Button></div> :
            videos.length === 0 ? <p>{t("profile.emptyVideos")}</p> :
                <div className="grid grid-cols-3 gap-2 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6 xl:grid-cols-7 2xl:grid-cols-8">
                    {videos.map(video => <Link key={video.id} to={`/@${username}/${kind}/video/${video.id}`}
                        aria-label={video.description || t("studio.noDescription")}
                        className="relative aspect-[3/4] overflow-hidden rounded-md bg-neutral-800">
                        {video.thumbnailUrl && <img src={video.thumbnailUrl} alt="" loading="lazy" className="h-full w-full object-cover"/>}
                    </Link>)}
                </div>}
        {error && videos.length > 0 && <div className="mt-4" role="alert">{t("profile.videosLoadError")} <Button onClick={() => void loadMore()}>{t("chat.privacy.retry")}</Button></div>}
        {hasNext && <div ref={sentinelRef} data-testid="collection-load-more" className="h-1 w-full" aria-hidden="true"/>}
        {isFetching && videos.length > 0 && <p className="mt-4 text-center" role="status">{t("profile.loadingVideos")}</p>}
    </div>;
}
