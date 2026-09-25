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

export interface GetAdminUsersParams extends PageParams {
    search?: string;
    isBanned?: boolean;
}

export interface GetAdminVideosParams extends PageParams {
    isBanned?: boolean;
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
        getAdminUsers: build.query<ApiResponse<PagedResult<SimpleUserDto>>, GetAdminUsersParams>({
            query: ({pageNumber, pageSize, search, isBanned}) => ({
                url: `api/admin-panel/users?${new URLSearchParams({
                    pageNumber: String(pageNumber),
                    pageSize: String(pageSize),
                    ...(search ? {search} : {}),
                    ...(isBanned === undefined ? {} : {isBanned: String(isBanned)}),
                })}`,
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
            invalidatesTags: () => ["AdminUsers"],
        }),
        unbanUser: build.mutation<ApiResponse<null>, string>({
            query: (id) => ({
                url: `api/admin-panel/users/unban?id=${id}`,
                method: "post",
            }),
            invalidatesTags: () => ["AdminUsers"],
        }),
        getAdminUserVideos: build.query<ApiResponse<PagedResult<SimpleVideoDto>>, PageParams & {id: string}>({
            query: params => ({url: "api/admin-panel/users/video", params}),
            providesTags: result => result?.data.items.map(v => ({type: "AdminVideos", id: v.id})) ?? [],
        }),
        getAdminVideos: build.query<ApiResponse<PagedResult<SimpleVideoDto>>, GetAdminVideosParams>({
            query: ({pageNumber, pageSize, isBanned}) => ({
                url: `api/admin-panel/videos?${new URLSearchParams({
                    pageNumber: String(pageNumber),
                    pageSize: String(pageSize),
                    ...(isBanned === undefined ? {} : {isBanned: String(isBanned)}),
                })}`,
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
        markReportAsResolved: build.mutation<ApiResponse<null>, string>({
            query: (id) => ({
                url: `api/admin-panel/reports/mark-as-resolved/${id}`,
                method: "PATCH",
            }),
            invalidatesTags: () => [{type: "AdminReports", id: "Video"}, {type: "AdminReports", id: "User"}, {type: "AdminReports", id: "Comment"}],
        })
    }),
});

export const {
    useGetAdminUsersQuery,
    useGetAdminUserQuery,
    useBanUserMutation,
    useUnbanUserMutation,
    useGetAdminVideosQuery,
    useGetAdminUserVideosQuery,
    useBanVideoMutation,
    useUnbanVideoMutation,
    useGetAdminReportsQuery,
    useGetReportReasonsQuery,
    useDeleteAdminCommentMutation,
    useMarkReportAsResolvedMutation,
} = adminApi;
