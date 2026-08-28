import {createSlice} from "@reduxjs/toolkit";

const messagesDrawerSlice = createSlice({
    name: "messagesDrawer",
    initialState: {isOpened: false},
    reducers: {
        openDrawer: (state) => {
            state.isOpened = true;
        },
        closeDrawer: (state) => {
            state.isOpened = false;
        },
    },
});

export const {openDrawer, closeDrawer} = messagesDrawerSlice.actions;
export default messagesDrawerSlice.reducer;
