import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {ConversationDto} from "@/types/Conversation.ts";
import type {MessageDto} from "@/types/Message.ts";
import type {PagedResult} from "@/types/Pagination.ts";

export interface ChatUserDto {
    id: string;
    username: string;
    avatar?: { small?: string; medium?: string; large?: string } | string | null;
}

interface PaginationParams {
    pageNumber: number;
    pageSize: number;
}

export const conversationApi = createApi({
    reducerPath: "conversationApi",
    baseQuery: baseQueryWithReauth,
    endpoints: (build) => ({
        getConversations: build.query<ApiResponse<PagedResult<ConversationDto>>, PaginationParams>({
            query: ({pageNumber, pageSize}) => ({
                url: `api/conversations?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
        }),
        getMessages: build.query<ApiResponse<PagedResult<MessageDto>>, PaginationParams & { conversationId: string }>({
            query: ({conversationId, pageNumber, pageSize}) => ({
                url: `api/conversations/messages?conversationId=${conversationId}&pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
        }),
        createConversation: build.mutation<ApiResponse<ConversationDto>, { userId: string }>({
            query: (body) => ({url: "api/conversations", method: "post", body}),
        }),
        searchConversations: build.query<ApiResponse<PagedResult<ConversationDto>>, PaginationParams & { query: string }>({
            query: ({query, pageNumber, pageSize}) => ({
                url: `api/conversations/search?query=${encodeURIComponent(query)}&pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
        }),
    }),
});

export const {
    useLazyGetConversationsQuery,
    useLazyGetMessagesQuery,
    useCreateConversationMutation,
    useLazySearchConversationsQuery,
} = conversationApi;
