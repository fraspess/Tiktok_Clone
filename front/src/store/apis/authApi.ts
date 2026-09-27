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
            query: (data) => ({url: "api/users/login", method: "POST", body: data})
        }),
        register: build.mutation({
            query: (data) => ({url: "api/users/register", method: "POST", body: data})
        }),
        confirmEmail: build.mutation({
            query: (data) => ({url: "api/users/confirm-email", method: "POST", body: data})
        }),
        resendConfirmationCode: build.mutation({
            query: (email) => ({url: "api/users/resend-confirmation-email", method: "POST", body: email})
        }),
        refreshToken: build.mutation({
            queryFn: async (_arg, api, extraOptions) => {
                return baseQuery({url: "api/users/refresh", method: "POST"}, api, extraOptions);
            }
        }),
        getCurrentUser: build.query<ApiResponse<CurrentUserDto>, void>({
            query: () => ({url: "api/users/me", method: "GET"}),
        }),
        googleAuth: build.mutation({
            query: (token) => ({url: "api/users/google", method: "POST", body: token})
        }),
        logoutAll: build.mutation<ApiResponse<null>, void>({
            query: () => ({url: "api/users/logout/all", method: "POST"}),
        }),
        logout: build.mutation({
            query: () => ({url: "api/users/logout", method: "POST"})
        }),
        forgotPassword: build.mutation({
            query: (data) => ({url: "api/users/forgot-password", method: "POST", body: data})
        }),
        resetPassword: build.mutation({
            query: (data) => ({url: "api/users/reset-password", method: "POST", body: data})
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
    useLogoutAllMutation,
    useForgotPasswordMutation,
    useResetPasswordMutation,
} = authApi;
