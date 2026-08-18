import {createSlice} from "@reduxjs/toolkit";

// Sound preference is intentionally global (one value for the whole feed), not per-video.
// It always starts muted so the very first autoplay is guaranteed to work; it only ever
// flips to unmuted through a real click on the mute button, which is what lets the browser
// treat every following video's autoplay-with-sound as trusted (a genuine user gesture),
// instead of each VideoCard re-guessing for itself and getting silently blocked.
export const playerSlice = createSlice({
    name: "player",
    initialState: {isMuted: true},
    reducers: {
        setMuted: (state, action) => {
            state.isMuted = action.payload;
        },
    },
});

export const {setMuted} = playerSlice.actions;
export default playerSlice.reducer;
