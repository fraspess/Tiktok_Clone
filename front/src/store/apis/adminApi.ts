import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {PagedResult} from "@/types/Pagination.ts";
import type {
    AdminReportDto,
    AdminUserDto,
    EnumValueDto,
    ReportType,
    SimpleUserDto,
    SimpleVideoDto,
} from "@/types/Admin.ts";

export interface PageParams {
    pageNumber: number;
    pageSize: number;
}

export interface BanUserParams {
    id: string;
    reason: number;
}

export interface BanVideoParams {
    id: string;
    reason: number;
}

export interface GetReportsParams extends PageParams {
    reportType: ReportType;
}

export const adminApi = createApi({
    reducerPath: "adminApi",
    baseQuery: baseQueryWithReauth,
    tagTypes: ["AdminUsers", "AdminVideos", "AdminReports"],
    endpoints: (build) => ({
        getAdminUsers: build.query<ApiResponse<PagedResult<SimpleUserDto>>, PageParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/admin-panel/users?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
            providesTags: (result) =>
                result?.data?.items
                    ? [...result.data.items.map((u) => ({type: "AdminUsers" as const, id: u.id})), "AdminUsers"]
                    : ["AdminUsers"],
        }),
        getAdminUser: build.query<ApiResponse<AdminUserDto>, string>({
            query: (id) => ({
                url: `api/admin-panel/users/${id}`,
                method: "get",
            }),
            providesTags: (_result, _err, id) => [{type: "AdminUsers", id}],
        }),
        banUser: build.mutation<ApiResponse<null>, BanUserParams>({
            query: ({id, reason}) => ({
                url: `api/admin-panel/users/ban?id=${id}&reason=${reason}`,
                method: "post",
            }),
            invalidatesTags: (_result, _err, {id}) => [{type: "AdminUsers", id}],
        }),
        unbanUser: build.mutation<ApiResponse<null>, string>({
            query: (id) => ({
                url: `api/admin-panel/users/unban?id=${id}`,
                method: "post",
            }),
            invalidatesTags: (_result, _err, id) => [{type: "AdminUsers", id}],
        }),
        getAdminVideos: build.query<ApiResponse<PagedResult<SimpleVideoDto>>, PageParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/admin-panel/videos?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
            providesTags: (result) =>
                result?.data?.items
                    ? [...result.data.items.map((v) => ({type: "AdminVideos" as const, id: v.id})), "AdminVideos"]
                    : ["AdminVideos"],
        }),
        banVideo: build.mutation<ApiResponse<null>, BanVideoParams>({
            query: ({id, reason}) => ({
                url: `api/admin-panel/videos/ban?id=${id}&reason=${reason}`,
                method: "post",
            }),
            invalidatesTags: (_result, _err, {id}) => [{type: "AdminVideos", id}],
        }),
        unbanVideo: build.mutation<ApiResponse<null>, string>({
            query: (id) => ({
                url: `api/admin-panel/videos/unban?id=${id}`,
                method: "post",
            }),
            invalidatesTags: (_result, _err, id) => [{type: "AdminVideos", id}],
        }),
        getAdminReports: build.query<ApiResponse<PagedResult<AdminReportDto>>, GetReportsParams>({
            query: ({reportType, pageNumber, pageSize}) => ({
                url: `api/admin-panel/reports?reportType=${reportType}&pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
            providesTags: (_result, _err, {reportType}) => [{type: "AdminReports", id: reportType}],
        }),
        getReportReasons: build.query<ApiResponse<EnumValueDto[]>, ReportType>({
            query: (contentType) => ({
                url: `api/enums/report-reasons?contentType=${contentType}`,
                method: "get",
            }),
        }),
        deleteAdminComment: build.mutation<ApiResponse<null>, string>({
            query: (id) => ({
                url: `api/admin-panel/comments/${id}`,
                method: "delete",
            }),
            invalidatesTags: (_result, _err, id) => [{type: "AdminReports", id}],
        }),
    }),
});

export const {
    useGetAdminUsersQuery,
    useGetAdminUserQuery,
    useBanUserMutation,
    useUnbanUserMutation,
    useGetAdminVideosQuery,
    useBanVideoMutation,
    useUnbanVideoMutation,
    useGetAdminReportsQuery,
    useGetReportReasonsQuery,
    useDeleteAdminCommentMutation,
} = adminApi;