import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {SimpleUser, UserProfile} from "@/types/User.ts";
import type {PagedResult} from "@/types/Pagination.ts";
import {parseMessagePrivacy, type MessagePrivacy} from "@/lib/messagePrivacy.ts";

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
    tagTypes: ["UserProfile", "MessagePrivacy"],
    endpoints: (build) => ({
        getMessagePrivacy: build.query<ApiResponse<MessagePrivacy | null>, void>({
            query: () => "api/users/settings/message-privacy",
            transformResponse: (response: ApiResponse<unknown>) => ({
                ...response,
                data: parseMessagePrivacy(response.data),
            }),
            providesTags: ["MessagePrivacy"],
        }),
        changeMessagePrivacy: build.mutation<ApiResponse<null>, MessagePrivacy>({
            query: (newPrivacy) => ({
                url: "api/users/settings/message-privacy",
                method: "POST",
                params: {newPrivacy},
            }),
            invalidatesTags: (_result, error) => error ? [] : ["MessagePrivacy"],
        }),
        getMe: build.query<ApiResponse<UserProfile>, void>({
            query: () => ({
                url: `api/users/me`,
                method: "GET",
            }),
            providesTags: (result) =>
                result?.data
                    ? [{type: "UserProfile", id: result.data.username}, {type: "UserProfile", id: "ME"}]
                    : [{type: "UserProfile", id: "ME"}],
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
            invalidatesTags: () => [{type: "UserProfile", id: "ME"}],
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
    useGetMessagePrivacyQuery,
    useChangeMessagePrivacyMutation,
    useGetMeQuery,
    useGetUserProfileQuery,
    useFollowUserMutation,
    useUnfollowUserMutation,
    useUpdateUserMutation,
    useChangeUsernameMutation,
    useLazyGetFollowersQuery,
    useLazyGetFollowingQuery,
} = userApi;
