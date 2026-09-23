export interface AvatarDto {
    small: string;
    medium: string;
    large: string;
}

export interface SimpleUserDto {
    id: string;
    avatar: AvatarDto | object | string | null;
    username: string | null;
    isBanned: boolean;
}

export interface AdminUserDto {
    id: string;
    username: string | null;
    description: string;
    followersCount: number;
    followingCount: number;
    avatar: object | string | null;
    createdAt: string;
    isBanned: boolean;
    banReason: number | null;
}

export type SimpleVideoAuthor = {
    id: string;
    username: string;
    avatar: object | string | null;
    isFollowing: boolean;
} | null;

export interface SimpleVideoDto {
    id: string;
    videoUrl: string | null;
    description: string | null;
    hashTags: string[] | null;
    author: SimpleVideoAuthor;
    createdAt: string;
    viewCount: number | null;
    isBanned: boolean;
    thumbnailUrl: string | null;
}

export interface ReportUserDto {
    id: string;
    username: string;
    avatar: object | string | null;
}

export interface ReportedContentDto {
    id: string;
    title?: string | null;
    thumbnail?: object | string | null;
    contentUrl?: string | null;
}

export interface AdminReportDto {
    id: string;
    createdAt: string;
    status: number;
    reason: string | null;
    reportedBy: ReportUserDto | null;
    reportedContent: ReportedContentDto | null;
}

export type ReportType = "Video" | "Comment" | "User";

export interface EnumValueDto {
    id: number;
    description?: string;
    name?: string;
}
