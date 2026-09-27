import {useCallback, useEffect, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLazyGetUserVideosQuery, useLazyGetVideoCollectionQuery, type CollectionKind} from "@/store/apis/videoApi.ts";
import {useAppDispatch} from "@/store/hooks.ts";
import {cacheVideos} from "@/store/slices/videosCacheSlice.ts";
import type {VideoDto} from "@/types/Video.ts";

interface UseInfiniteUserVideosOptions {
    seedVideos?: VideoDto[];
    seedNextPage?: number;
    seedHasNext?: boolean;
    collectionKind?: CollectionKind;
}

export function useInfiniteUserVideos(
    userId: string | undefined,
    pageSize: number = 12,
    options?: UseInfiniteUserVideosOptions
) {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const [triggerUserVideos, userVideosQuery] = useLazyGetUserVideosQuery();
    const [triggerCollection, collectionQuery] = useLazyGetVideoCollectionQuery();
    const [videos, setVideos] = useState<VideoDto[]>(() => options?.seedVideos ?? []);
    const [hasNext, setHasNext] = useState<boolean>(options?.seedHasNext ?? true);
    const [error, setError] = useState<string | null>(null);

    const nextPageRef = useRef(options?.seedNextPage ?? 1);
    const seenIdsRef = useRef<Set<string>>(new Set((options?.seedVideos ?? []).map((v) => v.id)));
    const isLoadingRef = useRef(false);
    const sourceKey = `${userId ?? ""}:${options?.collectionKind ?? "videos"}`;
    const previousSourceKeyRef = useRef(sourceKey);


    useEffect(() => {
        if (previousSourceKeyRef.current === sourceKey) {
            return;
        }
        previousSourceKeyRef.current = sourceKey;
        nextPageRef.current = 1;
        seenIdsRef.current = new Set();
        isLoadingRef.current = false;
        setVideos([]);
        setHasNext(true);
        setError(null);
    }, [sourceKey]);

    const loadMore = useCallback(async () => {
        if (!userId || isLoadingRef.current || !hasNext) {
            return;
        }
        isLoadingRef.current = true;
        try {
            const response = options?.collectionKind
                ? await triggerCollection({
                    userId,
                    kind: options.collectionKind,
                    pageNumber: nextPageRef.current,
                    pageSize,
                }).unwrap()
                : await triggerUserVideos({
                    userId,
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
            setError(t("profile.videosLoadError"));
        } finally {
            isLoadingRef.current = false;
        }
    }, [triggerCollection, triggerUserVideos, userId, pageSize, hasNext, t, dispatch, options?.collectionKind]);

    return {videos, loadMore, hasNext, isFetching: userVideosQuery.isFetching || collectionQuery.isFetching, error};
}
