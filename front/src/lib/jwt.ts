interface JwtPayload {
    [key: string]: unknown;
}

export function decodeJwtPayload(token: string): JwtPayload | null {
    if (!token) return null;
    const parts = token.split(".");
    if (parts.length < 2) return null;
    try {
        const payload = parts[1].replace(/-/g, "+").replace(/_/g, "/");
        const padded = payload.padEnd(Math.ceil(payload.length / 4) * 4, "=");
        const binary = atob(padded);
        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
        const raw = new TextDecoder("utf-8").decode(bytes);
        return JSON.parse(raw) as JwtPayload;
    } catch {
        return null;
    }
}

export function getJwtRoles(token: string): string[] {
    const payload = decodeJwtPayload(token);
    if (!payload) return [];
    const role = payload["role"];
    if (Array.isArray(role)) {
        return role.filter((r): r is string => typeof r === "string");
    }
    if (typeof role === "string") {
        return [role];
    }
    return [];
}

export function hasAdminRole(token: string): boolean {
    const roles = getJwtRoles(token);
    return roles.includes("Admin") || roles.includes("SuperAdmin");
}