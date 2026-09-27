// Example: Creating a slice
import { createSlice } from '@reduxjs/toolkit';
export const authModalSlice = createSlice({
    name: 'authModal',
    initialState: { isOpened: false },
    reducers: {
        openModal: (state) => { state.isOpened = true; },
        closeModal: (state) => { state.isOpened = false; },
    },
});
export const {openModal, closeModal} = authModalSlice.actions;
export default authModalSlice.reducer;
