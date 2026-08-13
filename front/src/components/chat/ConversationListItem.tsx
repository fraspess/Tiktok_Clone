import { useTranslation } from "react-i18next";
import { cn } from "@/lib/utils.ts";
import UserAvatar from "@/components/chat/UserAvatar.tsx";
import { getConversationDisplayName, getConversationPeer } from "@/lib/conversationHelpers.ts";
import type { ConversationDto } from "@/types/Conversation.ts";

interface ConversationListItemProps {
    conversation: ConversationDto;
    isSelected?: boolean;
    onClick?: () => void;
    currentUser?: { id: string; username: string };
}

const ConversationListItem = ({ conversation, isSelected = false, onClick, currentUser }: ConversationListItemProps) => {
    const { t } = useTranslation();
    const displayName = getConversationDisplayName(
        conversation,
        t("chat.unnamedConversation"),
        currentUser
    );
    const peer = getConversationPeer(conversation, currentUser?.id);

    return (
        <button
            type="button"
            onClick={onClick}
            className={cn(
                "group flex w-full items-center gap-3 rounded-lg px-3 py-3 text-left transition-colors hover:bg-white/10 active:bg-white/15",
                isSelected && "bg-white/10"
            )}
        >
            <UserAvatar
                username={peer?.username || displayName}
                avatar={peer?.avatar}
            />

            <div className="min-w-0 flex-1">
                <div className="flex items-center gap-2">
                    <span className="truncate text-[15px] font-semibold text-white/95">
                        {displayName.startsWith("@") ? displayName : `@${displayName}`}
                    </span>
                </div>
            </div>
        </button>
    );
};

export default ConversationListItem;
