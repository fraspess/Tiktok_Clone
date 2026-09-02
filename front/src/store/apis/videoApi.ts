import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {PagedResult} from "@/types/Pagination.ts";
import type {VideoDto} from "@/types/Video.ts";
import type {CompleteUploadData, InitUploadData, InitUploadRequest} from "@/types/types.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";

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

export const videoApi = createApi({
    reducerPath: "videoApi",
    baseQuery: baseQueryWithReauth,
    endpoints: (build) => ({
        getFyp: build.query<ApiResponse<PagedResult<VideoDto>>, FypParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/videos/fyp?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
        }),
        getUserVideos: build.query<ApiResponse<PagedResult<VideoDto>>, UserVideosParams>({
            query: ({userId, pageNumber, pageSize}) => ({
                url: `api/videos/user/${userId}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
        }),
        reportVideo: build.mutation<ApiResponse<null>, ReportVideoParams>({
            query: ({contentId, reason, customReason}) => ({
                url: "api/reports",
                method: "post",
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
                method: "post",
            }),
        }),
        unlikeVideo: build.mutation<ApiResponse<null>, string>({
            query: (videoId) => ({
                url: `api/videos/${videoId}/like`,
                method: "delete",
            }),
        }),
        favoriteVideo: build.mutation<ApiResponse<null>, string>({
            query: (videoId) => ({
                url: `api/videos/${videoId}/favorite`,
                method: "post",
            }),
        }),
        unfavoriteVideo: build.mutation<ApiResponse<null>, string>({
            query: (videoId) => ({
                url: `api/videos/${videoId}/favorite`,
                method: "delete",
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
                method: "get",
            }),
        }),
        confirmUpload: build.mutation<null, CompleteUploadData>({
            query: (body) => ({
                url: "api/videos/upload-complete",
                method: "POST",
                body
            }),
        }),
        getFypFollowing: build.query<ApiResponse<PagedResult<VideoDto>>, FypParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/videos/fyp/following?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
        }),
    }),
});

export const {
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
} = videoApi;
