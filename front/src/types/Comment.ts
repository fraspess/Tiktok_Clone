import type {AvatarDto} from "@/types/Admin.ts";

export interface CommentDto {
    id: string;
    text: string;
    avatarUrl: AvatarDto;
    authorUsername: string;
    isLiked: boolean;
    likesCount: number;
    createdAt: string;
    isOwn: boolean;
}
