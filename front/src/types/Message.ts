import type {UserAvatar} from "@/types/Conversation.ts";

export interface MessageDto {
    id: string;
    conversationId?: string;
    senderId: string;
    senderUsername: string;
    senderAvatar?: UserAvatar | string | null;
    senderAvatarUrl?: string;
    content: string;
    createdAt: string;
    isOwn: boolean;
}
