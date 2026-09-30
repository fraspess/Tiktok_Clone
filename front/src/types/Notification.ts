import type {SimpleUserDto} from "@/types/Conversation";

export const notificationTypes = [
    "YourVideoLiked", "YourCommentLiked", "YourVideoAddedToFavorites", "NewDMMessage",
    "YourVideoReposted", "YourVideoCommented", "YourCommentReplied", "NewFollower",
] as const;

export interface NotificationDto {
    id: string;
    createdAt: string;
    recipientId: string;
    actorId: string | null;
    actor: SimpleUserDto | null;
    type: typeof notificationTypes[number] | number;
    resourceId: string | null;
    conversationId: string | null;
    videoShortId: string | null;
    readAt: string | null;
}

export function notificationType(notification: NotificationDto) {
    return typeof notification.type === "number" ? notificationTypes[notification.type] : notification.type;
}
