import {useEffect} from "react";
import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {API_BASE_URL} from "@/env";
import {useAppDispatch, useAppSelector} from "@/store/hooks";
import {notificationApi} from "@/store/apis/notificationApi";

export function useNotificationConnection() {
    const accessToken = useAppSelector(state => state.auth.accessToken);
    const dispatch = useAppDispatch();

    useEffect(() => {
        if (!accessToken) return;
        let disposed = false;
        let retry: ReturnType<typeof setTimeout> | undefined;
        const base = API_BASE_URL || window.location.origin;
        const connection = new HubConnectionBuilder()
            .withUrl(`${base.replace(/\/$/, "")}/hubs/notification`, {accessTokenFactory: () => accessToken})
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Error)
            .build();
        const refresh = () => {
            if (!disposed) dispatch(notificationApi.util.invalidateTags(["Notifications"]));
        };
        const start = async () => {
            try {
                await connection.start();
                if (disposed) { await connection.stop(); return; }
                refresh();
            } catch {
                if (!disposed) retry = setTimeout(() => void start(), 15000);
            }
        };
        connection.on("ReceiveNotification", refresh);
        connection.onreconnected(refresh);
        connection.onclose(() => {
            if (!disposed) retry = setTimeout(() => void start(), 15000);
        });
        void start();
        return () => {
            disposed = true;
            clearTimeout(retry);
            void connection.stop();
        };
    }, [accessToken, dispatch]);
}
