import {useCallback, useEffect, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLazySearchVideosQuery} from "@/store/apis/videoApi.ts";
import {useAppDispatch} from "@/store/hooks.ts";
import {cacheVideos} from "@/store/slices/videosCacheSlice.ts";
import type {VideoDto} from "@/types/Video.ts";

export function useInfiniteSearch(query: string, pageSize: number = 12) {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const [trigger, {isFetching}] = useLazySearchVideosQuery();
    const [videos, setVideos] = useState<VideoDto[]>([]);
    const [hasNext, setHasNext] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const nextPageRef = useRef(1);
    const seenIdsRef = useRef<Set<string>>(new Set());
    const isLoadingRef = useRef(false);
    const previousQueryRef = useRef(query);

    useEffect(() => {
        if (previousQueryRef.current === query) {
            return;
        }
        previousQueryRef.current = query;
        nextPageRef.current = 1;
        seenIdsRef.current = new Set();
        isLoadingRef.current = false;
        setVideos([]);
        setHasNext(true);
        setError(null);
    }, [query]);

    const loadMore = useCallback(async () => {
        const trimmed = query.trim();
        if (!trimmed || isLoadingRef.current || !hasNext) {
            return;
        }
        isLoadingRef.current = true;
        try {
            const response = await trigger({
                query: trimmed,
                pageNumber: nextPageRef.current,
                pageSize,
            }).unwrap();

            const {items, metadata} = response.data;
            const newItems = items.filter((video) => !seenIdsRef.current.has(video.id));
            newItems.forEach((video) => seenIdsRef.current.add(video.id));

            dispatch(cacheVideos(newItems));
            setVideos((prev) => [...prev, ...newItems]);
            setHasNext(metadata.hasNext);
            nextPageRef.current += 1;
        } catch {
            setError(t("search.loadError"));
        } finally {
            isLoadingRef.current = false;
        }
    }, [trigger, query, pageSize, hasNext, t, dispatch]);

    return {videos, loadMore, hasNext, isFetching, error};
}