import {useCallback, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLazyGetFypQuery} from "@/store/apis/videoApi.ts";
import type {VideoDto} from "@/types/Video.ts";

export function useInfiniteFyp(pageSize: number = 5) {
    const {t} = useTranslation();
    const [trigger, {isFetching}] = useLazyGetFypQuery();
    const [videos, setVideos] = useState<VideoDto[]>([]);
    const [hasNext, setHasNext] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const nextPageRef = useRef(1);
    const seenIdsRef = useRef<Set<string>>(new Set());
    const isLoadingRef = useRef(false);

    const loadMore = useCallback(async () => {
        if (isLoadingRef.current || !hasNext) {
            return;
        }
        isLoadingRef.current = true;
        try {
            const response = await trigger({
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
            setError(t("feed.loadError"));
        } finally {
            isLoadingRef.current = false;
        }
    }, [trigger, pageSize, hasNext, t]);

    return {videos, loadMore, hasNext, isFetching, error};
}
