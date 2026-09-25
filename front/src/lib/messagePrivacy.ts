export const messagePrivacyOptions = [
    {value: 0, key: "everyone", apiName: "Everyone"},
    {value: 1, key: "followers", apiName: "Followers"},
    {value: 2, key: "following", apiName: "Following"},
    {value: 3, key: "mutualFollowers", apiName: "MutualFollowers"},
    {value: 4, key: "nobody", apiName: "Nobody"},
] as const;

export type MessagePrivacy = typeof messagePrivacyOptions[number]["value"];

export function parseMessagePrivacy(value: unknown): MessagePrivacy | null {
    return messagePrivacyOptions.find(option => option.apiName === value || option.value === value)?.value ?? null;
}

export function isMessagePrivacyError(error: unknown): boolean {
    if (error instanceof Error) {
        return /\bMESSAGES_NOT_ACCEPTED\b/.test(error.message);
    }
    if (typeof error !== "object" || error === null || !("data" in error)) return false;
    const data = error.data;
    return typeof data === "object" && data !== null
        && "code" in data && data.code === "MESSAGES_NOT_ACCEPTED";
}
