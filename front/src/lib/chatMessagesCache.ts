import type {MessageDto} from "@/types/Message.ts";

const cacheKey = (conversationId: string) => `chat-messages:${conversationId}`;

function isMessage(value: unknown): value is MessageDto {
    if (!value || typeof value !== "object") return false;
    const message = value as Record<string, unknown>;
    return typeof message.id === "string" && typeof message.content === "string" &&
        typeof message.createdAt === "string" && typeof message.isOwn === "boolean";
}

export function getCachedMessages(conversationId: string): MessageDto[] {
    try {
        const stored = localStorage.getItem(cacheKey(conversationId));
        if (!stored) return [];
        const parsed: unknown = JSON.parse(stored);
        return Array.isArray(parsed) ? parsed.filter(isMessage) : [];
    } catch {
        return [];
    }
}

export function saveCachedMessages(conversationId: string, messages: MessageDto[]) {
    try {
        localStorage.setItem(cacheKey(conversationId), JSON.stringify(messages));
    } catch {
        // The active chat remains usable if browser storage is unavailable.
    }
}
