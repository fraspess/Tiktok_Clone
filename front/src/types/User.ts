export interface UserAvatar {
    small: string;
    medium: string;
    large: string;
}

export interface UserProfile {
    id: string;
    username: string;
    description: string;
    followersCount: number;
    followingCount: number;
    isOwnProfile: boolean;
    avatar: UserAvatar | null;
    isFollowing: boolean;
}
