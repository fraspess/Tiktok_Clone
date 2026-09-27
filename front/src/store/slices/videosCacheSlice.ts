import {createSlice, type PayloadAction} from "@reduxjs/toolkit";
import type {VideoDto} from "@/types/Video.ts";

interface VideosCacheState {
    videos: Record<string, VideoDto>;
}

const initialState: VideosCacheState = {videos: {}};

const videosCacheSlice = createSlice({
    name: "videosCache",
    initialState,
    reducers: {
        removeVideo: (state, action: PayloadAction<string>) => { delete state.videos[action.payload]; },
        cacheVideos: (state, action: PayloadAction<VideoDto[]>) => {
            for (const v of action.payload) {
                state.videos[v.id] = v;
            }
        },
        updateVideo: (state, action: PayloadAction<{id: string; changes: Partial<VideoDto>}>) => {
            const existing = state.videos[action.payload.id];
            if (existing) {
                Object.assign(existing, action.payload.changes);
            }
        },
    },
});

export const {cacheVideos, updateVideo, removeVideo} = videosCacheSlice.actions;
export default videosCacheSlice.reducer;
