const userProfiles = new Map<string, {username: string; avatar?: string | null}>();

export function saveUserProfile(id: string, username: string, avatar?: string | null) {
    if (!id || !username) return;
    const existing = userProfiles.get(id);
    userProfiles.set(id, {
        username: username || existing?.username || "",
        avatar: avatar ?? existing?.avatar ?? null,
    });
}

export function getUserProfile(id: string) {
    return userProfiles.get(id);
}
