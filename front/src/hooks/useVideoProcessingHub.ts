import {useEffect} from "react";
import {useStore} from "react-redux";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {createVideoHubConnection} from "@/lib/videoHub.ts";
import {
    fetchThumbnail,
    processingFailed,
    processingProgress,
    processingSucceeded,
    type UploadsState,
} from "@/store/slices/uploadsSlice.ts";


export const useVideoProcessingHub = () => {
    const dispatch = useAppDispatch();
    const store = useStore();
    const isAuth = useAppSelector((s) => s.auth.isAuth);

    useEffect(() => {
        if (!isAuth) return;

        const getToken = () =>
            (store.getState() as { auth: { accessToken?: string | null } }).auth.accessToken;
        
        const connection = createVideoHubConnection(getToken);

        connection.on("SendVideoProcessingProgress", (videoId: string, progress: number) => {
            dispatch(processingProgress({videoId, progress}));

            const {items} = (store.getState() as { uploads: UploadsState }).uploads;
            const item = items.find((i) => i.videoId === videoId);
            if (item && !item.thumbnailUrl) {
                dispatch(fetchThumbnail(videoId));
            }
        });

        connection.on("SendVideoProcessingSucceded", (videoId: string) => {
            const {items} = (store.getState() as { uploads: UploadsState }).uploads;
            const item = items.find((i) => i.videoId === videoId);
            if (item) URL.revokeObjectURL(item.previewUrl);
            dispatch(processingSucceeded(videoId));
        });

        connection.on("SendVideoProcessingFailed", (videoId: string, message: string) => {
            console.log("[hub] failed:", videoId, message); // LOG

            dispatch(processingFailed({videoId, message}));
        });

        connection.start()
            .then(() => console.log("[hub] connected, state:", connection.state)) // LOG
            .catch((err) => console.error("[hub] connection failed:", err)); // LOG

        return () => {
            console.log("[hub] stopping connection"); // LOG
            connection.stop();
        };
    }, [isAuth, dispatch, store]);
};