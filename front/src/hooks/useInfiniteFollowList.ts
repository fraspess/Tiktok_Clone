import {useCallback, useEffect, useRef, useState} from "react";
import {useLazyGetFollowersQuery, useLazyGetFollowingQuery} from "@/store/apis/userApi.ts";
import type {SimpleUser} from "@/types/User.ts";

type FollowListType = "followers" | "following";

export function useInfiniteFollowList(username: string | undefined, type: FollowListType, pageSize: number = 20) {
    const [triggerFollowers, {isFetching: isFetchingFollowers}] = useLazyGetFollowersQuery();
    const [triggerFollowing, {isFetching: isFetchingFollowing}] = useLazyGetFollowingQuery();
    const trigger = type === "followers" ? triggerFollowers : triggerFollowing;
    const isFetching = type === "followers" ? isFetchingFollowers : isFetchingFollowing;

    const [users, setUsers] = useState<SimpleUser[]>([]);
    const [hasNext, setHasNext] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const nextPageRef = useRef(1);
    const seenIdsRef = useRef<Set<string>>(new Set());
    const isLoadingRef = useRef(false);

    useEffect(() => {
        nextPageRef.current = 1;
        seenIdsRef.current = new Set();
        isLoadingRef.current = false;
        setUsers([]);
        setHasNext(true);
        setError(null);
    }, [username, type]);

    const loadMore = useCallback(async () => {
        if (!username || isLoadingRef.current || !hasNext) {
            return;
        }
        isLoadingRef.current = true;
        try {
            const response = await trigger({
                username,
                pageNumber: nextPageRef.current,
                pageSize,
            }).unwrap();

            const {items, metadata} = response.data;
            const newItems = items.filter((u) => !seenIdsRef.current.has(u.id));
            newItems.forEach((u) => seenIdsRef.current.add(u.id));

            setUsers((prev) => [...prev, ...newItems]);
            setHasNext(metadata.hasNext);
            nextPageRef.current += 1;
        } catch {
            setError("error");
        } finally {
            isLoadingRef.current = false;
        }
    }, [trigger, username, pageSize, hasNext]);

    return {users, loadMore, hasNext, isFetching, error};
}