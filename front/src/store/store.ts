import {configureStore} from '@reduxjs/toolkit'
import {setupListeners} from '@reduxjs/toolkit/query'
import authModalReducer from "@/store/slices/authModalSlice";
import authReducer from "@/store/slices/authSlice"
import playerReducer from "@/store/slices/playerSlice"
import {authApi} from "@/store/apis/authApi.ts";
import {conversationApi} from "@/store/apis/conversationApi.ts";
import {videoApi} from "@/store/apis/videoApi.ts";
import {userApi} from "@/store/apis/userApi.ts";
import {commentApi} from "@/store/apis/commentApi.ts";

export const store = configureStore({
    reducer: {
        authModal: authModalReducer,
        auth: authReducer,
        player: playerReducer,
        [authApi.reducerPath]: authApi.reducer,
        [conversationApi.reducerPath]: conversationApi.reducer,
        [videoApi.reducerPath]: videoApi.reducer,
        [userApi.reducerPath]: userApi.reducer,
        [commentApi.reducerPath]: commentApi.reducer,
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(authApi.middleware, videoApi.middleware),
})

setupListeners(store.dispatch)

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;