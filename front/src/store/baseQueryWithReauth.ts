import type {BaseQueryFn, FetchArgs, FetchBaseQueryError,} from "@reduxjs/toolkit/query";
import {fetchBaseQuery,} from "@reduxjs/toolkit/query";
import {Mutex} from "async-mutex";
import type {RootState} from "@/store/store.ts";
import {logout, setAccessToken} from "@/store/slices/authSlice.ts";
import {API_BASE_URL} from "@/env.ts";


const baseQuery = fetchBaseQuery({
    baseUrl: `${API_BASE_URL}/`,
    credentials: "include",
    prepareHeaders: (headers, {getState}) => {
        const token = (getState() as RootState).auth.accessToken;
        if (token) headers.set("authorization", `Bearer ${token}`);
        return headers;
    },
});

// prevents multiple parallel refresh calls when several requests 401 at once
const mutex = new Mutex();

export const baseQueryWithReauth: BaseQueryFn<
    string | FetchArgs,
    unknown,
    FetchBaseQueryError
> = async (args, api, extraOptions) => {
    await mutex.waitForUnlock();
    let result = await baseQuery(args, api, extraOptions);
    if (result.error?.status === 401) {
        if (!mutex.isLocked()) {
            const release = await mutex.acquire();
            try {
                const refreshResult = await baseQuery(
                    "/users/refresh",
                    api,
                    extraOptions
                );
                if (refreshResult.data) {
                    const {accessToken} = refreshResult.data as { accessToken: string };
                    api.dispatch(setAccessToken(accessToken));
                    result = await baseQuery(args, api, extraOptions);
                } else {
                    api.dispatch(logout());
                }
            } finally {
                release();
            }
        } else {
            await mutex.waitForUnlock();
            result = await baseQuery(args, api, extraOptions);
        }
    }
    return result;
};