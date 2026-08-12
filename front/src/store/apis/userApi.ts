import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {UserProfile} from "@/types/User.ts";

interface FollowUserParams {
    followingId: string;
    username: string;
}

export const userApi = createApi({
    reducerPath: "userApi",
    baseQuery: baseQueryWithReauth,
    tagTypes: ["UserProfile"],
    endpoints: (build) => ({
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
    }),
});

export const {useGetUserProfileQuery, useFollowUserMutation} = userApi;
