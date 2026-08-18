import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {UserProfile} from "@/types/User.ts";

interface FollowUserParams {
    followingId: string;
    username: string;
}

interface UpdateUserParams {
    username: string;
    formData: FormData;
}

interface ChangeUsernameParams {
    currentUsername: string;
    newUsername: string;
}

export const userApi = createApi({
    reducerPath: "userApi",
    baseQuery: baseQueryWithReauth,
    tagTypes: ["UserProfile"],
    endpoints: (build) => ({
        getMe: build.query<ApiResponse<UserProfile>, void>({
            query: () => ({
                url: `api/users/me`,
                method: "get",
            }),
            providesTags: (result) =>
                result?.data ? [{type: "UserProfile", id: result.data.username}] : [],
        }),
        getUserProfile: build.query<ApiResponse<UserProfile>, string>({
            query: (username) => ({
                url: `api/users/${username}`,
                method: "get",
            }),
            providesTags: (_result, _error, username) => [{type: "UserProfile", id: username}],
        }),
        followUser: build.mutation<ApiResponse<null>, FollowUserParams>({
            query: ({followingId}) => ({
                url: `api/users/follow?following=${followingId}`,
                method: "post",
            }),
            invalidatesTags: (_result, _error, {username}) => [{type: "UserProfile", id: username}],
        }),
        updateUser: build.mutation<ApiResponse<null>, UpdateUserParams>({
            query: ({formData}) => ({
                url: `api/users`,
                method: "patch",
                body: formData,
            }),
            invalidatesTags: (_result, _error, {username}) => [{type: "UserProfile", id: username}],
        }),
        changeUsername: build.mutation<ApiResponse<null>, ChangeUsernameParams>({
            query: ({newUsername}) => ({
                url: `api/users/change-username`,
                method: "patch",
                body: {newUsername},
            }),
            invalidatesTags: (_result, _error, {currentUsername}) => [{type: "UserProfile", id: currentUsername}],
        }),
    }),
});

export const {
    useGetMeQuery,
    useGetUserProfileQuery,
    useFollowUserMutation,
    useUpdateUserMutation,
    useChangeUsernameMutation,
} = userApi;
