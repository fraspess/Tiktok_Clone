import {useCallback, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLazyGetConversationsQuery} from "@/store/apis/conversationApi.ts";
import type {ConversationDto} from "@/types/Conversation.ts";

export function useInfiniteConversations(pageSize: number = 20) {
    const {t} = useTranslation();
    const [trigger, {isFetching}] = useLazyGetConversationsQuery();
    const [conversations, setConversations] = useState<ConversationDto[]>([]);
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
            const newItems = items.filter((conversation) => !seenIdsRef.current.has(conversation.id));
            newItems.forEach((conversation) => seenIdsRef.current.add(conversation.id));

            setConversations((prev) => [...prev, ...newItems]);
            setHasNext(metadata.hasNext);
            nextPageRef.current += 1;
        } catch {
            setError(t("chat.loadError"));
        } finally {
            isLoadingRef.current = false;
        }
    }, [trigger, pageSize, hasNext, t]);

    const refresh = useCallback(async () => {
        try {
            const response = await trigger({
                pageNumber: 1,
                pageSize: Math.max(20, conversations.length),
            }).unwrap();
            const {items, metadata} = response.data;
            seenIdsRef.current = new Set(items.map((c) => c.id));
            setConversations(items);
            setHasNext(metadata.hasNext);
        } catch {
            // Keep current conversations on error
        }
    }, [trigger, conversations.length]);

    return {conversations, loadMore, refresh, hasNext, isFetching, error};
}
