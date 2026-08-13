import {HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel} from "@microsoft/signalr";
import {API_BASE_URL} from "@/env.ts";
import type {MessageDto} from "@/types/Message.ts";

function getChatHubUrl() {
    const baseUrl = API_BASE_URL || window.location.origin;
    return new URL("hubs/chat", `${baseUrl.replace(/\/$/, "")}/`).toString();
}

/** Latest access token for the shared hub (read on every negotiate/reconnect). */
let accessTokenProvider: (() => string) | null = null;

let sharedConnection: HubConnection | null = null;
let startPromise: Promise<void> | null = null;
let subscriberCount = 0;

type MessageHandler = (message: MessageDto) => void;
type PendingMessagesHandler = (messages: MessageDto[]) => void;

const messageHandlers = new Set<MessageHandler>();
const pendingHandlers = new Set<PendingMessagesHandler>();
const statusListeners = new Set<(connected: boolean) => void>();

function notifyStatus(connected: boolean) {
    statusListeners.forEach((listener) => listener(connected));
}

function ensureConnection(): HubConnection {
    if (sharedConnection) {
        return sharedConnection;
    }

    const connection = new HubConnectionBuilder()
        .withUrl(getChatHubUrl(), {
            accessTokenFactory: () => accessTokenProvider?.() ?? "",
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Warning)
        .build();

    connection.on("ReceivedMessage", (message: MessageDto) => {
        messageHandlers.forEach((handler) => handler(message));
    });
    connection.on("ReceivedPendingMessages", (messages: MessageDto[]) => {
        pendingHandlers.forEach((handler) => handler(messages));
    });
    connection.onreconnecting(() => notifyStatus(false));
    connection.onreconnected(() => notifyStatus(true));
    connection.onclose(() => {
        startPromise = null;
        notifyStatus(false);
    });

    sharedConnection = connection;
    return connection;
}

async function startSharedConnection(): Promise<void> {
    const connection = ensureConnection();

    if (connection.state === HubConnectionState.Connected) {
        notifyStatus(true);
        return;
    }

    if (startPromise) {
        await startPromise;
        return;
    }

    startPromise = connection
        .start()
        .then(() => {
            notifyStatus(true);
        })
        .catch((error: unknown) => {
            startPromise = null;
            notifyStatus(false);
            // React Strict Mode stops the first attempt mid-negotiate; ignore that noise.
            const message = error instanceof Error ? error.message : String(error);
            if (!message.includes("stopped during negotiation")) {
                console.warn("[chatHub] failed to start", error);
            }
            throw error;
        });

    await startPromise;
}

/**
 * Subscribe to the shared chat hub. Safe under React Strict Mode:
 * mount/unmount only adjusts a ref-count; the socket is not torn down
 * while another subscriber (or the remount) still needs it.
 */
export function subscribeChatHub(options: {
    getAccessToken: () => string;
    onMessage?: MessageHandler;
    onPendingMessages?: PendingMessagesHandler;
    onStatusChange?: (connected: boolean) => void;
}): {unsubscribe: () => void; connection: () => HubConnection | null} {
    accessTokenProvider = options.getAccessToken;
    subscriberCount += 1;

    if (options.onMessage) messageHandlers.add(options.onMessage);
    if (options.onPendingMessages) pendingHandlers.add(options.onPendingMessages);
    if (options.onStatusChange) {
        statusListeners.add(options.onStatusChange);
        options.onStatusChange(sharedConnection?.state === HubConnectionState.Connected);
    }

    const token = options.getAccessToken();
    if (token) {
        void startSharedConnection().catch(() => {
            // Status already notified; caller can retry via token change.
        });
    }

    return {
        connection: () => sharedConnection,
        unsubscribe: () => {
            if (options.onMessage) messageHandlers.delete(options.onMessage);
            if (options.onPendingMessages) pendingHandlers.delete(options.onPendingMessages);
            if (options.onStatusChange) statusListeners.delete(options.onStatusChange);

            subscriberCount = Math.max(0, subscriberCount - 1);

            // Defer stop so Strict Mode remount can re-subscribe without killing negotiate.
            window.setTimeout(() => {
                if (subscriberCount > 0) return;

                const connection = sharedConnection;
                if (!connection) return;

                sharedConnection = null;
                startPromise = null;
                accessTokenProvider = null;
                void connection.stop().catch(() => undefined);
                notifyStatus(false);
            }, 0);
        },
    };
}

export async function ensureChatConnected(): Promise<boolean> {
    try {
        await startSharedConnection();
        return sharedConnection?.state === HubConnectionState.Connected;
    } catch {
        return false;
    }
}

export {HubConnectionState};
