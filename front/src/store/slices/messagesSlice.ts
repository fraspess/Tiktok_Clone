import {createSlice, type PayloadAction} from "@reduxjs/toolkit";
import type {ConversationDto} from "@/types/Conversation.ts";

interface MessagesState {
    isOpen: boolean;
    pendingConversation: ConversationDto | null;
}

const initialState: MessagesState = {isOpen: false, pendingConversation: null};

const messagesSlice = createSlice({
    name: "messages",
    initialState,
    reducers: {
        openMessages(state) {
            state.isOpen = true;
        },
        openMessagesWith(state, action: PayloadAction<ConversationDto>) {
            state.isOpen = true;
            state.pendingConversation = action.payload;
        },
        closeMessages(state) {
            state.isOpen = false;
        },
        clearOpenWith(state) {
            state.pendingConversation = null;
        },
    },
});

export const {openMessages, openMessagesWith, closeMessages, clearOpenWith} = messagesSlice.actions;
export default messagesSlice.reducer;