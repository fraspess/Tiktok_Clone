export interface VideoAuthorAvatar {
    small: string;
    medium: string;
    large: string;
}

export interface VideoAuthor {
    id: string;
    username: string;
    avatar: VideoAuthorAvatar | null;
}

export interface VideoDto {
    id: string;
    videoUrl: string;
    description: string;
    hashTags: string[];
    thumbnailUrl: string;
    likeCount: number;
    commentsCount: number;
    favoriteCount: number;
    viewCount: number;
    isFavorited: boolean;
    isLiked: boolean;
    author: VideoAuthor | null;
    createdAt: string;
}
