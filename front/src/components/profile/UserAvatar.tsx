import {useState} from "react";
import {UserRound} from "lucide-react";
import {cn} from "@/lib/utils.ts";
import type {VideoAuthorAvatar} from "@/types/Video.ts";

interface UserAvatarProps {
    avatar: VideoAuthorAvatar | null;
    size?: "small" | "medium" | "large";
    className?: string;
}

const sizeClasses = {
    small: "size-8",
    medium: "size-12",
    large: "size-28",
} as const;

const UserAvatar = ({avatar, size = "medium", className}: UserAvatarProps) => {
    const [failed, setFailed] = useState(false);
    const src = avatar?.[size];

    if (!src || failed) {
        return (
            <div
                className={cn(
                    "flex shrink-0 items-center justify-center rounded-full bg-muted text-muted-foreground",
                    sizeClasses[size],
                    className
                )}
            >
                <UserRound className="h-1/2 w-1/2"/>
            </div>
        );
    }

    return (
        <img
            src={src}
            alt=""
            onError={() => setFailed(true)}
            className={cn("shrink-0 rounded-full object-cover", sizeClasses[size], className)}
        />
    );
};

export default UserAvatar;
