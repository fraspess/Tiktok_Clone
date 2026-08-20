import {createSlice, type PayloadAction} from "@reduxjs/toolkit";

interface MessagesState {
    isOpen: boolean;
    openWithUsername: string | null;
    openWithUserId: string | null;
}

const initialState: MessagesState = {isOpen: false, openWithUsername: null, openWithUserId: null};

const messagesSlice = createSlice({
    name: "messages",
    initialState,
    reducers: {
        openMessages(state) {
            state.isOpen = true;
        },
        openMessagesWith(state, action: PayloadAction<{username: string; userId: string}>) {
            state.isOpen = true;
            state.openWithUsername = action.payload.username;
            state.openWithUserId = action.payload.userId;
        },
        closeMessages(state) {
            state.isOpen = false;
        },
        clearOpenWith(state) {
            state.openWithUsername = null;
            state.openWithUserId = null;
        },
    },
});

export const {openMessages, openMessagesWith, closeMessages, clearOpenWith} = messagesSlice.actions;
export default messagesSlice.reducer;
