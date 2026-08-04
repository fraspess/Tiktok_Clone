import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {UserProfileDto} from "@/types/User.ts";

interface FollowUserParams {
    userId: string;
    username: string;
}

export const userApi = createApi({
    reducerPath: "userApi",
    baseQuery: baseQueryWithReauth,
    tagTypes: ["UserProfile"],
    endpoints: (build) => ({
        getUserProfile: build.query<ApiResponse<UserProfileDto>, string>({
            query: (username) => ({
                url: `api/users/${encodeURIComponent(username)}`,
                method: "get",
            }),
            providesTags: (_result, _error, username) => [{type: "UserProfile", id: username}],
        }),
        followUser: build.mutation<ApiResponse<null>, FollowUserParams>({
            query: ({userId}) => ({
                url: `api/users/follow?following=${userId}`,
                method: "post",
            }),
            async onQueryStarted({username}, {dispatch, queryFulfilled}) {
                const patchResult = dispatch(
                    userApi.util.updateQueryData("getUserProfile", username, (draft) => {
                        if (!draft.data) return;
                        draft.data.isFollowing = !draft.data.isFollowing;
                        draft.data.followersCount += draft.data.isFollowing ? 1 : -1;
                    })
                );
                try {
                    await queryFulfilled;
                } catch {
                    patchResult.undo();
                }
            },
        }),
    }),
});

export const {useGetUserProfileQuery, useFollowUserMutation} = userApi;
