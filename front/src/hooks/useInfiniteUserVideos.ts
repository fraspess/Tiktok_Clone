import {useCallback, useEffect, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLazyGetUserVideosQuery} from "@/store/apis/videoApi.ts";
import type {VideoDto} from "@/types/Video.ts";

export function useInfiniteUserVideos(userId: string | undefined, pageSize: number = 12) {
    const {t} = useTranslation();
    const [trigger, {isFetching}] = useLazyGetUserVideosQuery();
    const [videos, setVideos] = useState<VideoDto[]>([]);
    const [hasNext, setHasNext] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const nextPageRef = useRef(1);
    const seenIdsRef = useRef<Set<string>>(new Set());
    const isLoadingRef = useRef(false);

    useEffect(() => {
        setVideos([]);
        setHasNext(true);
        setError(null);
        nextPageRef.current = 1;
        seenIdsRef.current = new Set();
    }, [userId]);

    const loadMore = useCallback(async () => {
        if (!userId || isLoadingRef.current || !hasNext) {
            return;
        }
        isLoadingRef.current = true;
        try {
            const response = await trigger({
                userId,
                pageNumber: nextPageRef.current,
                pageSize,
            }).unwrap();

            const {items, metadata} = response.data;
            const newItems = items.filter((video) => !seenIdsRef.current.has(video.id));
            newItems.forEach((video) => seenIdsRef.current.add(video.id));

            setVideos((prev) => [...prev, ...newItems]);
            setHasNext(metadata.hasNext);
            nextPageRef.current += 1;
        } catch {
            setError(t("profile.videosLoadError"));
        } finally {
            isLoadingRef.current = false;
        }
    }, [trigger, userId, pageSize, hasNext, t]);

    return {videos, loadMore, hasNext, isFetching, error};
}
