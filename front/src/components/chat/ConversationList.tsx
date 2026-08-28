import {useEffect, useMemo, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {Loader2} from "lucide-react";
import {useInfiniteConversations} from "@/hooks/useInfiniteConversations.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import {useLazySearchConversationsQuery} from "@/store/apis/conversationApi.ts";
import ConversationListItem from "@/components/chat/ConversationListItem.tsx";
import {Separator} from "@/components/ui/separator.tsx";
import type {ConversationDto} from "@/types/Conversation.ts";

interface ConversationListProps {
    selectedConversationId: string | null;
    onSelect: (conversation: ConversationDto) => void;
    currentUser?: {id: string; username: string};
    searchQuery?: string;
    newConversation?: ConversationDto | null;
}

const ConversationList = ({selectedConversationId, onSelect, currentUser, searchQuery = "", newConversation}: ConversationListProps) => {
    const {t} = useTranslation();
    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);
    const {conversations, loadMore, hasNext, isFetching, error} = useInfiniteConversations(20);
    const [searchConversations, {isFetching: isSearchFetching}] = useLazySearchConversationsQuery();
    const [searchResults, setSearchResults] = useState<ConversationDto[]>([]);

    const isSearching = searchQuery.trim().length > 0;

    useEffect(() => {
        if (!isSearching) {
            setSearchResults([]);
            return;
        }
        let cancelled = false;
        void searchConversations({query: searchQuery.trim(), pageNumber: 1, pageSize: 20}).unwrap()
            .then((res) => {
                if (!cancelled) setSearchResults(res.data.items);
            })
            .catch(() => {
                if (!cancelled) setSearchResults([]);
            });
        return () => { cancelled = true; };
    }, [isSearching, searchQuery, searchConversations]);

    useEffect(() => {
        loadMore();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const sentinelOptions = useMemo(
        () => ({root: containerRef, rootMargin: "200px"}),
        []
    );

    const isSentinelVisible = useIntersectionObserver(sentinelRef, sentinelOptions);

    useEffect(() => {
        if (!isSearching && isSentinelVisible && hasNext && !isFetching) {
            loadMore();
        }
    }, [isSentinelVisible, hasNext, isFetching, loadMore, isSearching]);

    const displayConversations = (() => {
        const base = isSearching ? searchResults : conversations;
        if (newConversation && !base.some((c) => c.id === newConversation.id)) {
            return [newConversation, ...base];
        }
        return base;
    })();
    const listLoading = isSearching ? isSearchFetching : isFetching;

    if (displayConversations.length === 0 && listLoading) {
        return (
            <div className="flex flex-1 items-center justify-center text-muted-foreground">
                <Loader2 className="mr-2 h-5 w-5 animate-spin"/>
                {isSearching ? t("chat.searchUsers") : t("chat.loading")}
            </div>
        );
    }

    if (displayConversations.length === 0 && !listLoading && isSearching) {
        return (
            <div className="flex flex-1 flex-col items-center justify-center gap-2 px-6 text-center text-muted-foreground">
                <p className="text-sm">{t("chat.noUsersFound")}</p>
            </div>
        );
    }

    if (displayConversations.length === 0 && error) {
        return (
            <div className="flex flex-1 items-center justify-center px-6 text-center text-muted-foreground">
                {error}
            </div>
        );
    }

    if (displayConversations.length === 0) {
        return (
            <div className="flex flex-1 flex-col items-center justify-center gap-2 px-6 text-center text-muted-foreground">
                <p className="text-base font-medium text-foreground">{t("chat.emptyTitle")}</p>
                <p className="text-sm">{t("chat.empty")}</p>
            </div>
        );
    }

    return (
        <div ref={containerRef} className="flex-1 overflow-y-auto px-2 py-2">
            {displayConversations.map((conversation, index) => (
                <div key={conversation.id}>
                    <ConversationListItem
                        conversation={conversation}
                        isSelected={conversation.id === selectedConversationId}
                        onClick={() => onSelect(conversation)}
                        currentUser={currentUser}
                    />
                    {index < displayConversations.length - 1 && <Separator className="ml-[76px] bg-white/8"/>}
                </div>
            ))}

            {!isSearching && hasNext && (
                <div ref={sentinelRef} className="flex h-12 items-center justify-center">
                    {isFetching && <Loader2 className="h-5 w-5 animate-spin text-muted-foreground"/>}
                </div>
            )}
        </div>
    );
};

export default ConversationList;
