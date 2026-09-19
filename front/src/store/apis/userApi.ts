import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {SimpleUser, UserProfile} from "@/types/User.ts";
import type {PagedResult} from "@/types/Pagination.ts";

interface FollowUserParams {
    followingId: string;
    username: string;
}

interface FollowListParams {
    username: string;
    pageNumber: number;
    pageSize: number;
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
                method: "GET",
            }),
            providesTags: (result) =>
                result?.data ? [{type: "UserProfile", id: result.data.username}] : [],
        }),
        getUserProfile: build.query<ApiResponse<UserProfile>, string>({
            query: (username) => ({
                url: `api/users/${username}`,
                method: "GET",
            }),
            providesTags: (_result, _error, username) => [{type: "UserProfile", id: username}],
        }),
        followUser: build.mutation<ApiResponse<null>, FollowUserParams>({
            query: ({followingId}) => ({
                url: `api/users/follow?following=${followingId}`,
                method: "POST",
            }),
            invalidatesTags: (_result, _error, {username}) => [{type: "UserProfile", id: username}],
        }),
        unfollowUser: build.mutation<ApiResponse<null>, FollowUserParams>({
            query: ({followingId}) => ({
                url: `api/users/follow?following=${followingId}`,
                method: "DELETE",
            }),
            invalidatesTags: (_result, _error, {username}) => [{type: "UserProfile", id: username}],
        }),
        updateUser: build.mutation<ApiResponse<null>, UpdateUserParams>({
            query: ({formData}) => ({
                url: `api/users`,
                method: "PATCH",
                body: formData,
            }),
            invalidatesTags: (_result, _error, {username}) => [{type: "UserProfile", id: username}],
        }),
        changeUsername: build.mutation<ApiResponse<null>, ChangeUsernameParams>({
            query: ({newUsername}) => ({
                url: `api/users/change-username`,
                method: "PATCH",
                body: {newUsername},
            }),
            invalidatesTags: (_result, _error, {currentUsername}) => [{type: "UserProfile", id: currentUsername}],
        }),
        getFollowers: build.query<ApiResponse<PagedResult<SimpleUser>>, FollowListParams>({
            query: ({username, pageNumber, pageSize}) => ({
                url: `api/users/${username}/followers?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
        }),
        getFollowing: build.query<ApiResponse<PagedResult<SimpleUser>>, FollowListParams>({
            query: ({username, pageNumber, pageSize}) => ({
                url: `api/users/${username}/following?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
        }),
    }),
});

export const {
    useGetMeQuery,
    useGetUserProfileQuery,
    useFollowUserMutation,
    useUnfollowUserMutation,
    useUpdateUserMutation,
    useChangeUsernameMutation,
    useLazyGetFollowersQuery,
    useLazyGetFollowingQuery,
} = userApi;
