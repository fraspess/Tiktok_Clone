import type {VideoAuthorAvatar} from "@/types/Video.ts";

export interface UserProfileDto {
    id: string;
    username: string;
    description: string;
    followersCount: number;
    followingCount: number;
    isOwnProfile: boolean;
    isFollowing: boolean;
    avatar: VideoAuthorAvatar | null;
}
