import * as signalR from "@microsoft/signalr";

export const createVideoHubConnection = (getToken: () => string | null | undefined) => {
    const HUB_URL = `${import.meta.env.VITE_API_BASE_URL}/hubs/video-process-status`;

    return new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL, {
            accessTokenFactory: () => getToken() ?? "",
        })
        .withAutomaticReconnect()
        .build();
};