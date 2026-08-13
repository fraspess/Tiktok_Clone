import type {UserAvatar} from "@/types/Conversation.ts";

type AvatarLike =
    | UserAvatar
    | string
    | {
        small?: string;
        medium?: string;
        large?: string;
        Small?: string;
        Medium?: string;
        Large?: string;
    }
    | null
    | undefined;

export function getAvatarUrl(avatar: AvatarLike): string | null {
    if (!avatar) {
        return null;
    }
    if (typeof avatar === "string") {
        const trimmed = avatar.trim();
        return trimmed.length > 0 ? trimmed : null;
    }

    const record = avatar as Record<string, string | undefined>;
    const url =
        record.small ||
        record.Small ||
        record.medium ||
        record.Medium ||
        record.large ||
        record.Large ||
        null;

    if (!url || typeof url !== "string") {
        return null;
    }

    const trimmed = url.trim();
    return trimmed.length > 0 ? trimmed : null;
}
