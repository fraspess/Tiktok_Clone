import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {PagedResult} from "@/types/Pagination.ts";
import type {VideoDto} from "@/types/Video.ts";

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
    }),
});

export const {useLazyGetFypQuery, useLazyGetUserVideosQuery, useReportVideoMutation} = videoApi;
