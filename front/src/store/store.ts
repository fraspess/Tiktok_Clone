import {configureStore} from '@reduxjs/toolkit'
// Or from '@reduxjs/toolkit/query/react'
import {setupListeners} from '@reduxjs/toolkit/query'
import authModalReducer from "@/store/slices/authModalSlice";
import authReducer from "@/store/slices/authSlice"
import {authApi} from "@/store/apis/authApi.ts";
import {videoApi} from "@/store/apis/videoApi.ts";
import {userApi} from "@/store/apis/userApi.ts";

export const store = configureStore({
    reducer: {
        // Add the generated reducer as a specific top-level slice
        authModal: authModalReducer,
        auth: authReducer,
        [authApi.reducerPath]: authApi.reducer,
        [videoApi.reducerPath]: videoApi.reducer,
        [userApi.reducerPath]: userApi.reducer,
    },
    // Adding the api middleware enables caching, invalidation, polling,
    // and other useful features of `rtk-query`.
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(authApi.middleware, videoApi.middleware, userApi.middleware),
})

// optional, but required for refetchOnFocus/refetchOnReconnect behaviors
// see `setupListeners` docs - takes an optional callback as the 2nd arg for customization
setupListeners(store.dispatch)

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;