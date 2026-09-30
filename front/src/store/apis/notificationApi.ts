import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth";
import type {ApiResponse} from "@/types/ApiResponse";
import type {PagedResult} from "@/types/Pagination";
import type {NotificationDto} from "@/types/Notification";

export const notificationApi = createApi({
    reducerPath: "notificationApi",
    baseQuery: baseQueryWithReauth,
    tagTypes: ["Notifications"],
    endpoints: build => ({
        getNotifications: build.infiniteQuery<ApiResponse<PagedResult<NotificationDto>>, void, number>({
            infiniteQueryOptions: {
                initialPageParam: 1,
                getNextPageParam: lastPage => lastPage.data.metadata.hasNext
                    ? lastPage.data.metadata.currentPage + 1 : undefined,
            },
            query: ({pageParam}) => `api/notifications?pageNumber=${pageParam}&pageSize=20`,
            providesTags: ["Notifications"],
            keepUnusedDataFor: 0,
        }),
        getUnreadNotificationCount: build.query<ApiResponse<number>, void>({
            query: () => "api/notifications/unread-count",
            providesTags: ["Notifications"],
            keepUnusedDataFor: 0,
        }),
        markNotificationRead: build.mutation<void, string>({
            query: id => ({url: `api/notifications/${id}/read`, method: "PATCH"}),
            invalidatesTags: ["Notifications"],
        }),
        markAllNotificationsRead: build.mutation<void, void>({
            query: () => ({url: "api/notifications/read-all", method: "PATCH"}),
            invalidatesTags: ["Notifications"],
        }),
    }),
});

export const {useGetNotificationsInfiniteQuery, useGetUnreadNotificationCountQuery,
    useMarkNotificationReadMutation, useMarkAllNotificationsReadMutation} = notificationApi;
