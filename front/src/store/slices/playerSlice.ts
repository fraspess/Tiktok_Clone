import {createSlice} from "@reduxjs/toolkit";

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
