import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";

export const authApi = createApi({
    reducerPath: 'authApi',
    baseQuery: baseQueryWithReauth,
    endpoints: (build) => ({
        login: build.mutation({
            query: (formData) => ({url: "users/login", method: "post", body: formData})
        })
    })
});

export const {useLoginMutation} = authApi;
