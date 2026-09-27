import {createSlice, type PayloadAction} from "@reduxjs/toolkit";
import {videoApi} from "@/store/apis/videoApi.ts";
import {putWithProgress} from "@/lib/putWithProgress.ts";
import {decodeJwtPayload} from "@/lib/jwt.ts";
// TODO: підстав свій шлях до типу AppDispatch
import type {AppDispatch} from "@/store/store.ts";

export interface PendingUpload {
    id: string;
    videoId?: string;
    previewUrl: string;
    thumbnailUrl?: string;
    progress: number;           // відправка файлу (XHR)
    processingProgress: number; // обробка на сервері (SignalR)
    status: "uploading" | "processing" | "error";
    errorMessage?: string;
}

// події SignalR, що прийшли раніше, ніж confirmUpload повернув videoId
interface EarlyEvent {
    progress?: number;
    succeeded?: boolean;
    failedMessage?: string;
}

export interface UploadsState {
    items: PendingUpload[];
    finishedCount: number;
    early: Record<string, EarlyEvent>;
}

const initialState: UploadsState = {items: [], finishedCount: 0, early: {}};

const finish = (s: UploadsState, videoId: string) => {
    s.items = s.items.filter((i) => i.videoId !== videoId);
    s.finishedCount += 1; // сигнал профілю перезавантажити список
};

const uploadsSlice = createSlice({
    name: "uploads",
    initialState,
    reducers: {
        addUpload: (s, a: PayloadAction<PendingUpload>) => {
            s.items.unshift(a.payload);
        },
        setProgress: (s, a: PayloadAction<{id: string; progress: number}>) => {
            const u = s.items.find((i) => i.id === a.payload.id);
            if (u) u.progress = a.payload.progress;
        },
        setStatus: (s, a: PayloadAction<{id: string; status: PendingUpload["status"]}>) => {
            const u = s.items.find((i) => i.id === a.payload.id);
            if (u) u.status = a.payload.status;
        },
        setVideoId: (s, a: PayloadAction<{id: string; videoId: string}>) => {
            const u = s.items.find((i) => i.id === a.payload.id);
            if (!u) return;
            u.videoId = a.payload.videoId;

            const early = s.early[a.payload.videoId];
            if (!early) return;
            delete s.early[a.payload.videoId];

            if (early.succeeded) {
                finish(s, a.payload.videoId);
            } else if (early.failedMessage !== undefined) {
                u.status = "error";
                u.errorMessage = early.failedMessage;
            } else if (early.progress !== undefined) {
                u.processingProgress = early.progress;
            }
        },
        processingProgress: (s, a: PayloadAction<{videoId: string; progress: number}>) => {
            const u = s.items.find((i) => i.videoId === a.payload.videoId);
            if (u) u.processingProgress = a.payload.progress;
            else s.early[a.payload.videoId] = {...s.early[a.payload.videoId], progress: a.payload.progress};
        },
        processingSucceeded: (s, a: PayloadAction<string>) => {
            if (s.items.some((i) => i.videoId === a.payload)) finish(s, a.payload);
            else s.early[a.payload] = {succeeded: true};
        },
        processingFailed: (s, a: PayloadAction<{videoId: string; message: string}>) => {
            const u = s.items.find((i) => i.videoId === a.payload.videoId);
            if (u) {
                u.status = "error";
                u.errorMessage = a.payload.message;
            } else {
                s.early[a.payload.videoId] = {failedMessage: a.payload.message};
            }
        },
        setThumbnail: (s, a: PayloadAction<{videoId: string; thumbnailUrl: string}>) => {
            const u = s.items.find((i) => i.videoId === a.payload.videoId);
            if (u) u.thumbnailUrl = a.payload.thumbnailUrl;
        },
        removeUpload: (s, a: PayloadAction<string>) => {
            s.items = s.items.filter((i) => i.id !== a.payload);
        },
        // завантаження завершене без videoId: прибираємо рядок і просимо список оновитись
        completeUpload: (s, a: PayloadAction<string>) => {
            s.items = s.items.filter((i) => i.id !== a.payload);
            s.finishedCount += 1;
        },
    },
});

export const {
    addUpload,
    setProgress,
    setStatus,
    setVideoId,
    processingProgress,
    processingSucceeded,
    processingFailed,
    setThumbnail,
    removeUpload,
    completeUpload,
} = uploadsSlice.actions;

export default uploadsSlice.reducer;

export const startUpload =
    (file: File, description: string) => async (dispatch: AppDispatch) => {
        const id = crypto.randomUUID();
        const previewUrl = URL.createObjectURL(file);

        dispatch(
            addUpload({id, previewUrl, progress: 0, processingProgress: 0, status: "uploading"})
        );

        try {
            const init = await dispatch(
                videoApi.endpoints.initUpload.initiate({contentType: file.type})
            ).unwrap();

            // Confirmation returns no ID; use the ID supplied in the upload token.
            const videoId = decodeJwtPayload(init.uploadToken)?.videoId;
            if (typeof videoId !== "string" || !videoId) {
                throw new Error("Upload token is missing videoId");
            }
            dispatch(setVideoId({id, videoId}));

            await putWithProgress(init.url, file, (progress) =>
                dispatch(setProgress({id, progress}))
            );

            dispatch(setStatus({id, status: "processing"}));

            const res = await dispatch(
                videoApi.endpoints.confirmUpload.initiate({
                    token: init.uploadToken,
                    description,
                })
            ).unwrap();

            if (!res.isSuccess) {
                throw new Error(res.message ?? "Upload confirmation failed");
            }
            // Keep the progress row until SignalR reports completion.
        } catch (e) {
            console.error(e);
            dispatch(setStatus({id, status: "error"}));
        }
    };

export const fetchThumbnail =
    (videoId: string) => async (dispatch: AppDispatch) => {
        try {
            const res = await dispatch(
                videoApi.endpoints.getVideoById.initiate(videoId, {
                    forceRefetch: true,
                    subscribe: false,
                })
            ).unwrap();

            const thumbnailUrl = res?.data?.thumbnailUrl;
            if (thumbnailUrl) dispatch(setThumbnail({videoId, thumbnailUrl}));
        } catch (e) {
            console.error(e);
        }
    };
