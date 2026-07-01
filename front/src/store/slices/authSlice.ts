// Example: Creating a slice
import {createSlice} from '@reduxjs/toolkit';

export const authSlice = createSlice({
    name: 'auth',
    initialState: {accessToken: "", isAuth: false, isLoading: false},
    reducers: {
        setAccessToken: (state, action) => {
            state.isAuth = true;
            state.accessToken = action.payload;
            state.isLoading = false;
        },
        logout: (state) => {
            state.isAuth = false;
            state.accessToken = "";
            state.isLoading = false;
        },
        setIsLoading: (state, payload) => {
            state.isLoading = payload as unknown as boolean;
        }
    },
});
export const {setAccessToken, logout, setIsLoading} = authSlice.actions;
export default authSlice.reducer;
