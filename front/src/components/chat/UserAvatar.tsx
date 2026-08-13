import {useState} from "react";
import {cn} from "@/lib/utils.ts";
import {getAvatarUrl} from "@/lib/getAvatarUrl.ts";
import type {UserAvatar as UserAvatarType} from "@/types/Conversation.ts";

interface UserAvatarProps {
    username: string;
    avatar?: UserAvatarType | string | null;
    size?: "sm" | "md" | "lg";
    className?: string;
}

const sizeClasses = {
    sm: "h-10 w-10 text-sm",
    md: "h-12 w-12 text-base",
    lg: "h-14 w-14 text-lg",
};

const UserAvatar = ({username, avatar, size = "md", className}: UserAvatarProps) => {
    const avatarUrl = getAvatarUrl(avatar);
    const [hasError, setHasError] = useState(false);
    const showImage = Boolean(avatarUrl) && !hasError;
    const initial = username?.trim()?.[0]?.toUpperCase() || "?";

    return (
        <div
            className={cn(
                "shrink-0 overflow-hidden rounded-full bg-neutral-700 ring-1 ring-white/10",
                sizeClasses[size],
                className
            )}
        >
            {showImage ? (
                <img
                    src={avatarUrl!}
                    alt={username}
                    className="h-full w-full object-cover"
                    onError={() => setHasError(true)}
                />
            ) : (
                <div className="flex h-full w-full items-center justify-center font-semibold text-neutral-200">
                    {initial}
                </div>
            )}
        </div>
    );
};

export default UserAvatar;
