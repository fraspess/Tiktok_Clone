import {useCallback, useEffect, useRef, useState} from "react";
import {ensureChatConnected, HubConnectionState, subscribeChatHub} from "@/lib/chatHub.ts";
import type {MessageDto} from "@/types/Message.ts";

interface UseChatConnectionOptions {
    accessToken: string;
    onMessagesReceived: (data?: MessageDto | MessageDto[]) => void;
}

export function useChatConnection({accessToken, onMessagesReceived}: UseChatConnectionOptions) {
    const accessTokenRef = useRef(accessToken);
    const onMessagesReceivedRef = useRef(onMessagesReceived);
    const connectionRef = useRef<ReturnType<typeof subscribeChatHub>["connection"] | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        accessTokenRef.current = accessToken;
    }, [accessToken]);

    useEffect(() => {
        onMessagesReceivedRef.current = onMessagesReceived;
    }, [onMessagesReceived]);

    useEffect(() => {
        if (!accessToken) {
            setIsConnected(false);
            return;
        }

        const handleMessage = (data?: MessageDto | MessageDto[]) => onMessagesReceivedRef.current(data);
        const subscription = subscribeChatHub({
            getAccessToken: () => accessTokenRef.current,
            onMessage: handleMessage,
            onPendingMessages: handleMessage,
            onStatusChange: setIsConnected,
        });
        connectionRef.current = subscription.connection;

        // Token refresh / re-open: ensure socket is up with the latest token provider.
        void ensureChatConnected();

        return () => {
            connectionRef.current = null;
            subscription.unsubscribe();
        };
    }, [accessToken]);

    const sendMessage = useCallback(async (conversationId: string, content: string) => {
        const getConnection = connectionRef.current;
        const connection = getConnection?.() ?? null;
        if (!connection || connection.state !== HubConnectionState.Connected) {
            const connected = await ensureChatConnected();
            const retry = connectionRef.current?.() ?? null;
            if (!connected || !retry || retry.state !== HubConnectionState.Connected) {
                throw new Error("Chat connection is unavailable");
            }
            await retry.invoke("SendMessage", conversationId, content);
            return;
        }
        await connection.invoke("SendMessage", conversationId, content);
    }, []);

    const markAsRead = useCallback(async (messageId: string) => {
        const connection = connectionRef.current?.() ?? null;
        if (connection?.state === HubConnectionState.Connected) {
            await connection.invoke("MarkAsRead", messageId);
        }
    }, []);

    return {isConnected, sendMessage, markAsRead};
}
