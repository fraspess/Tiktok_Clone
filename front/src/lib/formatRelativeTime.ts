export function formatRelativeTime(dateStr: string, locale?: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMin = Math.floor(diffMs / 60_000);
    const diffHour = Math.floor(diffMin / 60);
    const diffDay = Math.floor(diffHour / 24);

    if (diffMin < 1) {
        return "now";
    }
    if (diffMin < 60) {
        return `${diffMin}m`;
    }
    if (diffHour < 24) {
        return `${diffHour}h`;
    }
    if (diffDay === 1) {
        return "yesterday";
    }
    if (diffDay < 7) {
        return date.toLocaleDateString(locale, {weekday: "short"});
    }
    return date.toLocaleDateString(locale, {month: "short", day: "numeric"});
}
