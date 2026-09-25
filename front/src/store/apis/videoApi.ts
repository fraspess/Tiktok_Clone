import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {PagedResult} from "@/types/Pagination.ts";
import type {MyVideoDto, VideoDto} from "@/types/Video.ts";
import type {CompleteUploadData, InitUploadData, InitUploadRequest} from "@/types/types.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";

export type CollectionKind = "liked" | "reposts" | "favorites";

interface FypParams {
    pageNumber: number;
    pageSize: number;
}

interface UserVideosParams {
    userId: string;
    pageNumber: number;
    pageSize: number;
}

export interface ReportVideoParams {
    contentId: string;
    reason?: number;
    customReason?: string;
}

interface FavoriteVideosParams {
    userId: string;
    pageNumber: number;
    pageSize: number;
}

interface SearchVideosParams {
    query: string;
    pageNumber: number;
    pageSize: number;
}

export const videoApi = createApi({
    reducerPath: "videoApi",
    baseQuery: baseQueryWithReauth,
    tagTypes: ["Videos"],
    endpoints: (build) => ({
        getVideoCollection: build.query<ApiResponse<PagedResult<VideoDto>>, UserVideosParams & {kind: CollectionKind}>({
            query: ({userId, kind, ...params}) => ({url: `api/users/${userId}/${kind}`, params}),
            providesTags: ["Videos"],
        }),
        repostVideo: build.mutation<ApiResponse<null>, string>({
            query: id => ({url: `api/videos/${id}/repost`, method: "POST"}),
            invalidatesTags: (_result, error) => error ? [] : ["Videos"],
        }),
        unrepostVideo: build.mutation<ApiResponse<null>, string>({
            query: id => ({url: `api/videos/${id}/repost`, method: "DELETE"}),
            invalidatesTags: (_result, error) => error ? [] : ["Videos"],
        }),
        deleteVideo: build.mutation<ApiResponse<unknown>, string>({
            query: id => ({url: `api/videos/${id}`, method: "DELETE"}),
            invalidatesTags: (_result, error) => error ? [] : ["Videos"],
        }),
        viewVideo: build.mutation<ApiResponse<null>, string>({
            query: id => ({url: `api/videos/${id}/view`, method: "POST"}),
        }),
        reportContent: build.mutation<ApiResponse<null>, ReportVideoParams & {contentType: "Video" | "User" | "Comment"}>({
            query: body => ({url: "api/reports", method: "POST", body}),
        }),
        getFyp: build.query<ApiResponse<PagedResult<VideoDto>>, FypParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/videos/fyp?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
            providesTags: ["Videos"],
        }),
        getVideoById: build.query<ApiResponse<VideoDto>, string>({
            query: (id) => ({
                url: `api/videos/${id}`,
                method: "get",
            }),
        }),
        getUserVideos: build.query<ApiResponse<PagedResult<VideoDto>>, UserVideosParams>({
            query: ({userId, pageNumber, pageSize}) => ({
                url: `api/videos/user/${userId}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
            providesTags: ["Videos"],
        }),
        getMyVideos: build.query<ApiResponse<PagedResult<MyVideoDto>>, FypParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/videos/my?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
            providesTags: ["Videos"],
        }),
        reportVideo: build.mutation<ApiResponse<null>, ReportVideoParams>({
            query: ({contentId, reason, customReason}) => ({
                url: "api/reports",
                method: "POST",
                body: {
                    contentType: "Video",
                    contentId,
                    reason,
                    customReason,
                },
            }),
        }),
        likeVideo: build.mutation<ApiResponse<null>, string>({
            query: (videoId) => ({
                url: `api/videos/${videoId}/like`,
                method: "POST",
            }),
            invalidatesTags: (_result, error) => error ? [] : ["Videos"],
        }),
        unlikeVideo: build.mutation<ApiResponse<null>, string>({
            query: (videoId) => ({
                url: `api/videos/${videoId}/like`,
                method: "DELETE",
            }),
            invalidatesTags: (_result, error) => error ? [] : ["Videos"],
        }),
        favoriteVideo: build.mutation<ApiResponse<null>, string>({
            query: (videoId) => ({
                url: `api/videos/${videoId}/favorite`,
                method: "POST",
            }),
        }),
        unfavoriteVideo: build.mutation<ApiResponse<null>, string>({
            query: (videoId) => ({
                url: `api/videos/${videoId}/favorite`,
                method: "DELETE",
            }),
        }),
        initUpload: build.mutation<InitUploadData, InitUploadRequest>({
            query: (body) => ({
                url: "api/videos",
                method: "POST",
                body,
            }),
            transformResponse: (response: ApiResponse<InitUploadData>) => {
                if (!response.isSuccess || !response.data) {
                    throw response;
                }
                return response.data;
            },
            transformErrorResponse: (response) => response.data as ApiResponse<InitUploadData>,

        }),
        getFavoriteVideos: build.query<ApiResponse<PagedResult<VideoDto>>, FavoriteVideosParams>({
            query: ({userId, pageNumber, pageSize}) => ({
                url: `api/users/${userId}/favorites?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
            providesTags: ["Videos"],
        }),
        confirmUpload: build.mutation<ApiResponse<null>, CompleteUploadData>({
            query: (body) => ({
                url: "api/videos/upload-complete",
                method: "POST",
                body
            }),
        }),
        searchVideos: build.query<ApiResponse<PagedResult<VideoDto>>, SearchVideosParams>({
            query: ({query, pageNumber, pageSize}) => ({
                url: `api/videos/search/${encodeURIComponent(query)}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
            providesTags: ["Videos"],
        }),
        getFypFollowing: build.query<ApiResponse<PagedResult<VideoDto>>, FypParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/videos/fyp/following?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "GET",
            }),
            providesTags: ["Videos"],
        }),
    }),
});

export const {
    useGetVideoCollectionQuery,
    useLazyGetVideoCollectionQuery,
    useRepostVideoMutation,
    useUnrepostVideoMutation,
    useDeleteVideoMutation,
    useViewVideoMutation,
    useReportContentMutation,
    useLazyGetFypQuery,
    useLazyGetFypFollowingQuery,
    useLazyGetUserVideosQuery,
    useReportVideoMutation,
    useLikeVideoMutation,
    useUnlikeVideoMutation,
    useFavoriteVideoMutation,
    useUnfavoriteVideoMutation,
    useInitUploadMutation,
    useConfirmUploadMutation,
    useLazyGetFavoriteVideosQuery,
    useLazySearchVideosQuery,
    useLazyGetVideoByIdQuery,
    useGetMyVideosQuery,
} = videoApi;
