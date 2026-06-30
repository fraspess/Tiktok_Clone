// Example: Creating a slice
import {createSlice} from '@reduxjs/toolkit';

export const authSlice = createSlice({
    name: 'auth',
    initialState: {accessToken: "", isAuth: false},
    reducers: {
        setAccessToken: (state, action) => {
            state.isAuth = true;
            state.accessToken = action.payload;
        },
        logout: (state) => {
            state.isAuth = false;
            state.accessToken = "";
        }
    },
});
export const {setAccessToken, logout} = authSlice.actions;
export default authSlice.reducer;
