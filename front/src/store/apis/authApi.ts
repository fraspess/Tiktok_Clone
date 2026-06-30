import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";

export const authApi = createApi({
    reducerPath: 'authApi',
    baseQuery: baseQueryWithReauth,
    endpoints: (build) => ({
        login: build.mutation({
            query: (data) => ({url: "users/login", method: "post", body: data})
        }),
        register: build.mutation({
            query: (data) => ({url: "users/register", method: "post", body: data})
        }),
        confirmEmail: build.mutation({
            query: (data) => ({url: "users/confirm-email", method: "post", body: data})
        })
    })
});

export const {useLoginMutation, useRegisterMutation, useConfirmEmailMutation} = authApi;
