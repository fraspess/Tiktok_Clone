import {useEffect, useMemo, useRef} from "react";
import {useTranslation} from "react-i18next";
import {Loader2} from "lucide-react";
import {useInfiniteConversations} from "@/hooks/useInfiniteConversations.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import ConversationListItem from "@/components/chat/ConversationListItem.tsx";
import {Separator} from "@/components/ui/separator.tsx";
import type {ConversationDto} from "@/types/Conversation.ts";

interface ConversationListProps {
    selectedConversationId: string | null;
    onSelect: (conversation: ConversationDto) => void;
    currentUser?: {id: string; username: string};
}

const ConversationList = ({selectedConversationId, onSelect, currentUser}: ConversationListProps) => {
    const {t} = useTranslation();
    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);
    const {conversations, loadMore, hasNext, isFetching, error} = useInfiniteConversations(20);

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
        if (isSentinelVisible && hasNext && !isFetching) {
            loadMore();
        }
    }, [isSentinelVisible, hasNext, isFetching, loadMore]);

    if (conversations.length === 0 && isFetching) {
        return (
            <div className="flex flex-1 items-center justify-center text-muted-foreground">
                <Loader2 className="mr-2 h-5 w-5 animate-spin"/>
                {t("chat.loading")}
            </div>
        );
    }

    if (conversations.length === 0 && error) {
        return (
            <div className="flex flex-1 items-center justify-center px-6 text-center text-muted-foreground">
                {error}
            </div>
        );
    }

    if (conversations.length === 0) {
        return (
            <div className="flex flex-1 flex-col items-center justify-center gap-2 px-6 text-center text-muted-foreground">
                <p className="text-base font-medium text-foreground">{t("chat.emptyTitle")}</p>
                <p className="text-sm">{t("chat.empty")}</p>
            </div>
        );
    }

    return (
        <div ref={containerRef} className="flex-1 overflow-y-auto px-2 py-2">
            {conversations.map((conversation, index) => (
                <div key={conversation.id}>
                    <ConversationListItem
                        conversation={conversation}
                        isSelected={conversation.id === selectedConversationId}
                        onClick={() => onSelect(conversation)}
                        currentUser={currentUser}
                    />
                    {index < conversations.length - 1 && <Separator className="ml-[76px] bg-white/8"/>}
                </div>
            ))}

            {hasNext && (
                <div ref={sentinelRef} className="flex h-12 items-center justify-center">
                    {isFetching && <Loader2 className="h-5 w-5 animate-spin text-muted-foreground"/>}
                </div>
            )}
        </div>
    );
};

export default ConversationList;
