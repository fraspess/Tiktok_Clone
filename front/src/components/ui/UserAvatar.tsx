import {useState} from "react";
import {cn} from "@/lib/utils.ts";
import {getAvatarUrl} from "@/lib/getAvatarUrl.ts";

interface UserAvatarProps {
    username: string;
    avatar?: Parameters<typeof getAvatarUrl>[0];
    size?: "sm" | "md" | "lg";
    className?: string;
}

const sizeClasses = {
    sm: "h-10 w-10 text-sm",
    md: "h-12 w-12 text-base",
    lg: "h-14 w-14 text-lg",
};

const fallbackColors = [
    "bg-rose-700 text-white",
    "bg-violet-700 text-white",
    "bg-blue-700 text-white",
    "bg-teal-700 text-white",
    "bg-amber-800 text-white",
    "bg-fuchsia-700 text-white",
];

const UserAvatar = ({username, avatar, size = "md", className}: UserAvatarProps) => {
    const avatarUrl = getAvatarUrl(avatar);
    const [failedUrl, setFailedUrl] = useState<string | null>(null);
    const showImage = Boolean(avatarUrl) && avatarUrl !== failedUrl;
    const name = username?.trim().replace(/^@+/, "") || "?";
    const initial = Array.from(name)[0].toUpperCase();
    const colorHash = Array.from(name.toLowerCase()).reduce(
        (hash, character) => (hash * 31 + character.codePointAt(0)!) >>> 0,
        0
    );

    return (
        <div
            role="img"
            aria-label={name}
            className={cn(
                "relative shrink-0 overflow-hidden rounded-full ring-1 ring-border",
                fallbackColors[colorHash % fallbackColors.length],
                sizeClasses[size],
                className
            )}
        >
            <div aria-hidden="true" className="flex h-full w-full items-center justify-center font-semibold">
                {initial}
            </div>
            {showImage && (
                <img
                    key={avatarUrl}
                    src={avatarUrl!}
                    alt=""
                    className="absolute inset-0 h-full w-full object-cover"
                    onError={() => setFailedUrl(avatarUrl)}
                />
            )}
        </div>
    );
};

export default UserAvatar;
