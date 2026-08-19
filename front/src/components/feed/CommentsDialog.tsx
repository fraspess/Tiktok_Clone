import {type FormEvent, useEffect, useMemo, useState} from "react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Heart, Loader2, Send, Trash2} from "lucide-react";
import {Dialog, DialogContent, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {Button} from "@/components/ui/button.tsx";
import {cn, formatCount, formatRelativeTime} from "@/lib/utils.ts";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {
    useCreateCommentMutation,
    useDeleteCommentMutation,
    useGetCommentsQuery,
    useLazyGetRepliesQuery,
    useLikeCommentMutation,
} from "@/store/apis/commentApi.ts";
import type {CommentDto} from "@/types/Comment.ts";

interface CommentsDialogProps {
    videoId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
    onCommentsCountChange?: (delta: number) => void;
}

const PAGE_SIZE = 20;
const REPLIES_PAGE_SIZE = 5;

const CommentRow = ({
    comment,
    videoId,
    onDeleted,
}: {
    comment: CommentDto;
    videoId: string;
    onDeleted: () => void;
}) => {
    const {t, i18n} = useTranslation();
    const dispatch = useAppDispatch();
    const isAuth = useAppSelector((s) => s.auth.isAuth);

    const [isLiked, setIsLiked] = useState(comment.isLiked);
    const [likesCount, setLikesCount] = useState(comment.likesCount);
    const [likeComment] = useLikeCommentMutation();
    const [deleteComment, {isLoading: isDeleting}] = useDeleteCommentMutation();

    const [repliesOpen, setRepliesOpen] = useState(false);
    const [replies, setReplies] = useState<CommentDto[]>([]);
    const [repliesPage, setRepliesPage] = useState(1);
    const [hasMoreReplies, setHasMoreReplies] = useState(true);
    const [fetchReplies, {isFetching: isFetchingReplies}] = useLazyGetRepliesQuery();

    const [isReplying, setIsReplying] = useState(false);
    const [replyText, setReplyText] = useState("");
    const [createComment, {isLoading: isSendingReply}] = useCreateCommentMutation();
    const [localReplyCount, setLocalReplyCount] = useState(comment.repliesCount);

    useEffect(() => {
        setIsLiked(comment.isLiked);
        setLikesCount(comment.likesCount);
    }, [comment.isLiked, comment.likesCount]);

    const handleToggleLike = async () => {
        if (!isAuth) {
            dispatch(openModal());
            return;
        }
        const nextLiked = !isLiked;
        setIsLiked(nextLiked);
        setLikesCount((prev) => (nextLiked ? prev + 1 : prev - 1));
        try {
            await likeComment({commentId: comment.id}).unwrap();
        } catch {
            setIsLiked(!nextLiked);
            setLikesCount((prev) => (nextLiked ? prev - 1 : prev + 1));
            toast.error(t("comments.likeError"));
        }
    };

    const loadReplies = async (page: number) => {
        try {
            const res = await fetchReplies({commentId: comment.id, pageNumber: page, pageSize: REPLIES_PAGE_SIZE}).unwrap();
            setReplies((prev) => (page === 1 ? res.data.items : [...prev, ...res.data.items]));
            setHasMoreReplies(res.data.metadata.hasNext);
            setRepliesPage(page);
        } catch {
            toast.error(t("comments.loadError"));
        }
    };

    const handleToggleReplies = () => {
        const next = !repliesOpen;
        setRepliesOpen(next);
        if (next && replies.length === 0) {
            void loadReplies(1);
        }
    };

    const handleDelete = async () => {
        try {
            await deleteComment({commentId: comment.id, videoId}).unwrap();
            onDeleted();
        } catch {
            toast.error(t("comments.deleteError"));
        }
    };

    const handleSendReply = async (e: FormEvent) => {
        e.preventDefault();
        if (!isAuth) {
            dispatch(openModal());
            return;
        }
        const text = replyText.trim();
        if (!text) return;
        try {
            await createComment({text, videoId, parentCommentId: comment.id}).unwrap();
            setReplyText("");
            setIsReplying(false);
            setLocalReplyCount((prev) => prev + 1);
            setRepliesOpen(true);
            void loadReplies(1);
        } catch {
            toast.error(t("comments.sendError"));
        }
    };

    return (
        <div className="flex flex-col gap-2">
            <div className="flex gap-3">
                <div className="h-9 w-9 shrink-0 overflow-hidden rounded-full bg-muted">
                    {comment.avatarUrl ? (
                        <img src={comment.avatarUrl} alt={comment.ownerUsername} className="h-full w-full object-cover"/>
                    ) : (
                        <div className="flex h-full w-full items-center justify-center text-xs font-semibold">
                            {comment.ownerUsername?.[0]?.toUpperCase() ?? "?"}
                        </div>
                    )}
                </div>
                <div className="flex-1 min-w-0">
                    <div className="flex items-baseline gap-2">
                        <span className="text-sm font-semibold">{comment.ownerUsername}</span>
                        <span className="text-xs text-muted-foreground">
                            {formatRelativeTime(comment.createdAt, i18n.language)}
                        </span>
                    </div>
                    <p className="mt-0.5 whitespace-pre-wrap break-words text-sm">{comment.text}</p>

                    <div className="mt-1 flex items-center gap-4 text-xs text-muted-foreground">
                        <button
                            type="button"
                            onClick={() => setIsReplying((prev) => !prev)}
                            className="font-medium hover:text-foreground"
                        >
                            {t("comments.reply")}
                        </button>
                        {localReplyCount > 0 && (
                            <button type="button" onClick={handleToggleReplies} className="font-medium hover:text-foreground">
                                {repliesOpen
                                    ? t("comments.hideReplies")
                                    : t("comments.showReplies", {count: localReplyCount})}
                            </button>
                        )}
                        {comment.isOwn && (
                            <button
                                type="button"
                                onClick={handleDelete}
                                disabled={isDeleting}
                                className="flex items-center gap-1 font-medium text-destructive hover:opacity-80"
                            >
                                <Trash2 size={12}/>
                                {t("comments.delete")}
                            </button>
                        )}
                    </div>

                    {isReplying && (
                        <form onSubmit={handleSendReply} className="mt-2 flex items-center gap-2">
                            <input
                                autoFocus
                                value={replyText}
                                onChange={(e) => setReplyText(e.target.value)}
                                placeholder={t("comments.replyPlaceholder", {username: comment.ownerUsername})}
                                maxLength={500}
                                className="w-full min-w-0 rounded-full border border-input bg-transparent px-3 py-1.5 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/50"
                            />
                            <Button type="submit" size="icon-sm" disabled={isSendingReply || !replyText.trim()}>
                                {isSendingReply ? <Loader2 className="h-4 w-4 animate-spin"/> : <Send className="h-4 w-4"/>}
                            </Button>
                        </form>
                    )}

                    {repliesOpen && (
                        <div className="mt-3 flex flex-col gap-3 border-l border-border pl-4">
                            {replies.map((reply) => (
                                <CommentRow
                                    key={reply.id}
                                    comment={reply}
                                    videoId={videoId}
                                    onDeleted={() => setReplies((prev) => prev.filter((r) => r.id !== reply.id))}
                                />
                            ))}
                            {isFetchingReplies && (
                                <Loader2 className="h-4 w-4 animate-spin text-muted-foreground"/>
                            )}
                            {hasMoreReplies && !isFetchingReplies && (
                                <button
                                    type="button"
                                    onClick={() => loadReplies(repliesPage + 1)}
                                    className="self-start text-xs font-medium text-muted-foreground hover:text-foreground"
                                >
                                    {t("comments.loadMoreReplies")}
                                </button>
                            )}
                        </div>
                    )}
                </div>

                <button
                    type="button"
                    onClick={handleToggleLike}
                    className="flex shrink-0 flex-col items-center gap-0.5 pt-1"
                >
                    <Heart size={16} className={isLiked ? "fill-red-500 text-red-500" : "text-muted-foreground"}/>
                    <span className="text-[11px] text-muted-foreground">{formatCount(likesCount)}</span>
                </button>
            </div>
        </div>
    );
};

const CommentsDialog = ({videoId, open, onOpenChange, onCommentsCountChange}: CommentsDialogProps) => {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const isAuth = useAppSelector((s) => s.auth.isAuth);

    const [page, setPage] = useState(1);
    const [items, setItems] = useState<CommentDto[]>([]);
    const [hasMore, setHasMore] = useState(false);
    const [text, setText] = useState("");
    const [createComment, {isLoading: isSending}] = useCreateCommentMutation();

    const {data, isFetching, isError} = useGetCommentsQuery(
        {videoId, pageNumber: page, pageSize: PAGE_SIZE},
        {skip: !open || !videoId}
    );

    useEffect(() => {
        if (!open) {
            setPage(1);
            setItems([]);
            setText("");
        }
    }, [open]);

    useEffect(() => {
        if (!open || !data) return;
        setItems((prev) => (page === 1 ? data.data.items : [...prev, ...data.data.items]));
        setHasMore(data.data.metadata.hasNext);
    }, [data, page, open]);

    const handleDeleted = (commentId: string) => {
        setItems((prev) => prev.filter((c) => c.id !== commentId));
        onCommentsCountChange?.(-1);
    };

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        if (!isAuth) {
            dispatch(openModal());
            return;
        }
        const value = text.trim();
        if (!value) return;
        try {
            await createComment({text: value, videoId}).unwrap();
            setText("");
            if (page === 1) {
                setPage(1);
                // re-trigger fetch by forcing a fresh page 1 load
                setItems([]);
                setPage(1);
            }
            onCommentsCountChange?.(1);
            toast.success(t("comments.sendSuccess"));
        } catch {
            toast.error(t("comments.sendError"));
        }
    };

    const emptyState = useMemo(
        () => !isFetching && items.length === 0 && !isError,
        [isFetching, items.length, isError]
    );

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="flex max-h-[85vh] flex-col sm:max-w-lg">
                <DialogHeader>
                    <DialogTitle>{t("comments.title")}</DialogTitle>
                </DialogHeader>

                <div className="flex-1 overflow-y-auto pr-1">
                    {items.length === 0 && isFetching && (
                        <div className="flex justify-center py-8">
                            <Loader2 className="h-5 w-5 animate-spin text-muted-foreground"/>
                        </div>
                    )}

                    {isError && (
                        <p className="py-8 text-center text-sm text-muted-foreground">{t("comments.loadError")}</p>
                    )}

                    {emptyState && (
                        <p className="py-8 text-center text-sm text-muted-foreground">{t("comments.empty")}</p>
                    )}

                    <div className="flex flex-col gap-4">
                        {items.map((c) => (
                            <CommentRow key={c.id} comment={c} videoId={videoId} onDeleted={() => handleDeleted(c.id)}/>
                        ))}
                    </div>

                    {hasMore && (
                        <div className="flex justify-center py-3">
                            <Button
                                variant="ghost"
                                size="sm"
                                disabled={isFetching}
                                onClick={() => setPage((prev) => prev + 1)}
                            >
                                {isFetching ? <Loader2 className="h-4 w-4 animate-spin"/> : t("comments.loadMore")}
                            </Button>
                        </div>
                    )}
                </div>

                <form onSubmit={handleSubmit} className="flex items-center gap-2 border-t border-border pt-3">
                    <input
                        value={text}
                        onChange={(e) => setText(e.target.value)}
                        placeholder={isAuth ? t("comments.placeholder") : t("comments.signInToComment")}
                        maxLength={500}
                        className={cn(
                            "w-full min-w-0 rounded-full border border-input bg-transparent px-3.5 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/50"
                        )}
                    />
                    <Button type="submit" size="icon" disabled={isSending || !text.trim()}>
                        {isSending ? <Loader2 className="h-4 w-4 animate-spin"/> : <Send className="h-4 w-4"/>}
                    </Button>
                </form>
            </DialogContent>
        </Dialog>
    );
};

export default CommentsDialog;
