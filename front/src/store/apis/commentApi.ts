import {createApi} from "@reduxjs/toolkit/query/react";
import {baseQueryWithReauth} from "@/store/baseQueryWithReauth.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import type {PagedResult} from "@/types/Pagination.ts";
import type {CommentDto} from "@/types/Comment.ts";

interface GetCommentsParams {
    videoId: string;
    pageNumber: number;
    pageSize: number;
}

interface GetRepliesParams {
    commentId: string;
    pageNumber: number;
    pageSize: number;
}

interface CreateCommentParams {
    text: string;
    videoId: string;
    parentCommentId?: string;
}

export const commentApi = createApi({
    reducerPath: "commentApi",
    baseQuery: baseQueryWithReauth,
    tagTypes: ["Comments", "Replies"],
    endpoints: (build) => ({
        getComments: build.query<ApiResponse<PagedResult<CommentDto>>, GetCommentsParams>({
            query: ({videoId, pageNumber, pageSize}) => ({
                url: `api/comments/${videoId}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
            providesTags: (result, _err, {videoId}) =>
                result
                    ? [
                        ...result.data.items.map((c) => ({type: "Comments" as const, id: c.id})),
                        {type: "Comments" as const, id: videoId},
                    ]
                    : [{type: "Comments" as const, id: videoId}],
        }),
        getReplies: build.query<ApiResponse<PagedResult<CommentDto>>, GetRepliesParams>({
            query: ({commentId, pageNumber, pageSize}) => ({
                url: `api/comments/replies?commentId=${commentId}&pageNumber=${pageNumber}&pageSize=${pageSize}`,
                method: "get",
            }),
            providesTags: (_result, _err, {commentId}) => [{type: "Replies" as const, id: commentId}],
        }),
        createComment: build.mutation<ApiResponse<null>, CreateCommentParams>({
            query: ({text, videoId, parentCommentId}) => ({
                url: "api/comments",
                method: "post",
                body: {text, videoId, parentCommentId},
            }),
            invalidatesTags: (_result, _err, {videoId, parentCommentId}) =>
                parentCommentId
                    ? [{type: "Replies" as const, id: parentCommentId}, {type: "Comments" as const, id: videoId}]
                    : [{type: "Comments" as const, id: videoId}],
        }),
        deleteComment: build.mutation<ApiResponse<null>, {
            commentId: string;
            videoId: string;
            parentCommentId?: string
        }>({
            query: ({commentId}) => ({
                url: `api/comments?commentId=${commentId}`,
                method: "delete",
            }),
            invalidatesTags: (_result, _err, {videoId, parentCommentId}) =>
                parentCommentId
                    ? [{type: "Replies" as const, id: parentCommentId}, {type: "Comments" as const, id: videoId}]
                    : [{type: "Comments" as const, id: videoId}],
        }),
        likeComment: build.mutation<ApiResponse<null>, { commentId: string }>({
            query: ({commentId}) => ({
                url: `api/comments/like?commentId=${commentId}`,
                method: "post",
            }),
        }),
    }),
});

export const {
    useGetCommentsQuery,
    useLazyGetRepliesQuery,
    useCreateCommentMutation,
    useDeleteCommentMutation,
    useLikeCommentMutation,
} = commentApi;
