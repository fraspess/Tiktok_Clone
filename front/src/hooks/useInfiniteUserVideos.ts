import {useCallback, useEffect, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLazyGetUserVideosQuery} from "@/store/apis/videoApi.ts";
import type {VideoDto} from "@/types/Video.ts";

interface UseInfiniteUserVideosOptions {
    seedVideos?: VideoDto[];
    seedNextPage?: number;
    seedHasNext?: boolean;
}

export function useInfiniteUserVideos(
    userId: string | undefined,
    pageSize: number = 12,
    options?: UseInfiniteUserVideosOptions
) {
    const {t} = useTranslation();
    const [trigger, {isFetching}] = useLazyGetUserVideosQuery();
    const [videos, setVideos] = useState<VideoDto[]>(() => options?.seedVideos ?? []);
    const [hasNext, setHasNext] = useState<boolean>(options?.seedHasNext ?? true);
    const [error, setError] = useState<string | null>(null);

    const nextPageRef = useRef(options?.seedNextPage ?? 1);
    const seenIdsRef = useRef<Set<string>>(new Set((options?.seedVideos ?? []).map((v) => v.id)));
    const isLoadingRef = useRef(false);
    const previousUserIdRef = useRef(userId);


    useEffect(() => {
        if (previousUserIdRef.current === userId) {
            return;
        }
        previousUserIdRef.current = userId;
        nextPageRef.current = 1;
        seenIdsRef.current = new Set();
        isLoadingRef.current = false;
        setVideos([]);
        setHasNext(true);
        setError(null);
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
