import {useCallback, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLazyGetFavoriteVideosQuery} from "@/store/apis/videoApi.ts";
import {useAppDispatch} from "@/store/hooks.ts";
import {cacheVideos} from "@/store/slices/videosCacheSlice.ts";

export function useInfiniteFavoriteVideos(userId: string | undefined, enabled: boolean, pageSize: number = 12) {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const [trigger, {isFetching}] = useLazyGetFavoriteVideosQuery();
    const [videoIds, setVideoIds] = useState<string[]>([]);
    const [hasNext, setHasNext] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const nextPageRef = useRef(1);
    const seenIdsRef = useRef<Set<string>>(new Set());
    const isLoadingRef = useRef(false);

    const loadMore = useCallback(async () => {
        if (!userId || !enabled || isLoadingRef.current || !hasNext) {
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

            dispatch(cacheVideos(items));

            setVideoIds((prev) => [...prev, ...newItems.map((v) => v.id)]);
            setHasNext(metadata.hasNext);
            nextPageRef.current += 1;
        } catch {
            setError(t("profile.favoritesLoadError"));
        } finally {
            isLoadingRef.current = false;
        }
    }, [trigger, userId, enabled, pageSize, hasNext, t, dispatch]);

    return {videoIds, loadMore, hasNext, isFetching, error};
}