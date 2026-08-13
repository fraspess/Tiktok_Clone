export interface UserAvatar {
    small: string;
    medium: string;
    large: string;
}

export interface SimpleUserDto {
    id: string;
    username: string;
    avatar: UserAvatar | string;
}

export interface LastMessagePreviewDto {
    content: string;
    createdAt: string;
    senderId: string;
    senderUsername: string;
    isOwn: boolean;
}

export interface ConversationDto {
    id: string;
    participants: SimpleUserDto[];
}
