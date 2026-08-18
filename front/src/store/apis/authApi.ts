import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQuery, baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";

interface CurrentUserDto {
    id: string;
    username: string;
}

export const authApi = createApi({
    reducerPath: 'authApi',
    baseQuery: baseQueryWithReauth,
    endpoints: (build) => ({
        login: build.mutation({
            query: (data) => ({url: "api/users/login", method: "post", body: data})
        }),
        register: build.mutation({
            query: (data) => ({url: "api/users/register", method: "post", body: data})
        }),
        confirmEmail: build.mutation({
            query: (data) => ({url: "api/users/confirm-email", method: "post", body: data})
        }),
        resendConfirmationCode: build.mutation({
            query: (email) => ({url: "api/users/resend-confirmation-email", method: "post", body: email})
        }),
        refreshToken: build.mutation({
            queryFn: async (_arg, api, extraOptions) => {
                return baseQuery({url: "api/users/refresh", method: "post"}, api, extraOptions);
            }
        }),
        getCurrentUser: build.query<ApiResponse<CurrentUserDto>, void>({
            query: () => ({url: "api/users/me", method: "get"}),
        }),
        googleAuth: build.mutation({
            query: (token) => ({url: "api/users/google", method: "post", body: token})
        }),
        logout: build.mutation({
            query: () => ({url: "api/users/logout", method: "post"})
        })
    })
});

export const {
    useLoginMutation,
    useRegisterMutation,
    useConfirmEmailMutation,
    useResendConfirmationCodeMutation,
    useRefreshTokenMutation,
    useGetCurrentUserQuery,
    useGoogleAuthMutation,
    useLogoutMutation,
} = authApi;

