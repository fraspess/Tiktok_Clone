import type {ConversationDto, SimpleUserDto} from "@/types/Conversation.ts";
import {getCachedMessages} from "@/lib/chatMessagesCache.ts";
import {getUserProfile, saveUserProfile} from "@/lib/userProfileCache.ts";

/** Other participants (exclude current user). */
export function getOtherParticipants(
    conversation: ConversationDto,
    currentUserId?: string
): SimpleUserDto[] {
    if (!currentUserId) return conversation.participants;
    return conversation.participants.filter((p) => p.id !== currentUserId);
}

/** Primary peer for a 1:1 chat — the other person, not yourself. */
export function getConversationPeer(
    conversation: ConversationDto,
    currentUserId?: string
): SimpleUserDto | undefined {
    const others = getOtherParticipants(conversation, currentUserId);
    const rawPeer = others[0] ?? conversation.participants[0];
    if (!rawPeer) return undefined;

    const cached = getUserProfile(rawPeer.id);
    const username = rawPeer.username || cached?.username || "";
    const avatar = rawPeer.avatar || cached?.avatar || "";

    return {
        ...rawPeer,
        username,
        avatar,
    };
}

/** Stable key for a 1:1 pair so we can keep only one chat per person. */
export function getDirectPeerKey(
    conversation: ConversationDto,
    currentUserId?: string
): string | null {
    const others = getOtherParticipants(conversation, currentUserId);
    if (others.length === 1) return others[0].id;
    if (others.length === 0 && conversation.participants.length === 1) {
        return conversation.participants[0].id;
    }
    // Group chats: key by sorted participant ids
    if (conversation.participants.length > 2) {
        return [...conversation.participants.map((p) => p.id)].sort().join("|");
    }
    return null;
}

/** Keep only the first (newest) conversation per peer. */
export function dedupeConversationsByPeer(
    conversations: ConversationDto[],
    currentUserId?: string
): ConversationDto[] {
    const seen = new Set<string>();
    const result: ConversationDto[] = [];

    for (const conversation of conversations) {
        const peerKey = getDirectPeerKey(conversation, currentUserId);
        if (peerKey) {
            if (seen.has(peerKey)) continue;
            seen.add(peerKey);
        } else if (seen.has(conversation.id)) {
            continue;
        } else {
            seen.add(conversation.id);
        }
        result.push(conversation);
    }

    return result;
}

export function findConversationWithUser(
    conversations: ConversationDto[],
    userId: string,
    currentUserId?: string
): ConversationDto | undefined {
    return conversations.find((conversation) => {
        const others = getOtherParticipants(conversation, currentUserId);
        // Direct 1:1 with exactly this user
        if (others.length === 1 && others[0].id === userId) return true;
        // Fallback: any conversation that includes this user as a participant
        return conversation.participants.some((p) => p.id === userId) &&
            conversation.participants.length <= 2;
    });
}

export function getConversationDisplayName(
    conversation: ConversationDto,
    fallback: string,
    currentUser?: {id: string; username: string}
): string {
    const peer = getConversationPeer(conversation, currentUser?.id);
    if (peer?.username) return peer.username;

    if (peer?.id) {
        const cachedProfile = getUserProfile(peer.id);
        if (cachedProfile?.username) return cachedProfile.username;

        const cachedMsgs = getCachedMessages(conversation.id);
        const peerMsg = cachedMsgs.find((m) => m.senderId === peer.id && m.senderUsername);
        if (peerMsg?.senderUsername) {
            saveUserProfile(peer.id, peerMsg.senderUsername);
            return peerMsg.senderUsername;
        }
    }

    const others = getOtherParticipants(conversation, currentUser?.id);
    const names = others
        .map((p) => p.username || getUserProfile(p.id)?.username)
        .filter(Boolean);

    if (names.length > 0) return names.join(", ");

    const cachedMsgs = getCachedMessages(conversation.id);
    const otherMsg = cachedMsgs.find((m) => m.senderId !== currentUser?.id && m.senderUsername);
    if (otherMsg?.senderUsername) {
        return otherMsg.senderUsername;
    }

    if (currentUser?.username && others.length === 0) return currentUser.username;

    return fallback;
}

/** Merge known user profile into a conversation participant (username/avatar). */
export function enrichConversationWithUser(
    conversation: ConversationDto,
    user: {id: string; username: string; avatar?: SimpleUserDto["avatar"] | null}
): ConversationDto {
    return {
        ...conversation,
        participants: conversation.participants.map((p) =>
            p.id === user.id
                ? {
                    ...p,
                    username: p.username || user.username,
                    avatar: p.avatar || user.avatar || p.avatar,
                }
                : p
        ),
    };
}
