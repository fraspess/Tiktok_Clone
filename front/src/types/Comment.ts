export interface CommentDto {
    id: string;
    text: string;
    repliesCount: number;
    avatarUrl: string;
    ownerUsername: string;
    isLiked: boolean;
    likesCount: number;
    createdAt: string;
    isOwn: boolean;
}
