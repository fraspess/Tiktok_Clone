import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog.tsx";
import {Button} from "@/components/ui/button.tsx";
import {
    useLazyGetFollowersQuery,
    useLazyGetFollowingQuery,
} from "@/store/apis/userApi.ts";
import type {SimpleUser} from "@/types/User.ts";
import {getAvatarUrl} from "@/lib/getAvatarUrl.ts";

const PAGE_SIZE = 20;

interface FollowListDialogProps {
    username: string;
    type: "followers" | "following";
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

const FollowListDialog = ({username, type, open, onOpenChange}: FollowListDialogProps) => {
    const {t} = useTranslation();
    const [users, setUsers] = useState<SimpleUser[]>([]);
    const [page, setPage] = useState(1);
    const [hasNext, setHasNext] = useState(false);
    const [loadFollowers, followersState] = useLazyGetFollowersQuery();
    const [loadFollowing, followingState] = useLazyGetFollowingQuery();
    const state = type === "followers" ? followersState : followingState;
    const isLoading = followersState.isFetching || followingState.isFetching;

    useEffect(() => {
        if (!open) return;

        setUsers([]);
        setPage(1);
        setHasNext(false);
        const load = type === "followers" ? loadFollowers : loadFollowing;

        void load({username, pageNumber: 1, pageSize: PAGE_SIZE})
            .unwrap()
            .then((result) => {
                setUsers(result.data?.items ?? []);
                setHasNext(result.data?.metadata.hasNext ?? false);
            });
    }, [open, type, username, loadFollowers, loadFollowing]);

    const loadNextPage = () => {
        if (isLoading || !hasNext) return;

        const nextPage = page + 1;
        const load = type === "followers" ? loadFollowers : loadFollowing;
        void load({username, pageNumber: nextPage, pageSize: PAGE_SIZE})
            .unwrap()
            .then((result) => {
                setUsers((current) => [...current, ...(result.data?.items ?? [])]);
                setPage(nextPage);
                setHasNext(result.data?.metadata.hasNext ?? false);
            });
    };

    const title = type === "followers" ? t("profile.followers") : t("profile.following");
    const emptyMessage = type === "followers" ? t("profile.emptyFollowers") : t("profile.emptyFollowing");

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>{title}</DialogTitle>
                    <DialogDescription>@{username}</DialogDescription>
                </DialogHeader>
                <div className="max-h-80 overflow-y-auto">
                    {state.isError ? (
                        <p className="py-4 text-sm text-destructive">{t("profile.loadError")}</p>
                    ) : isLoading && users.length === 0 ? (
                        <p className="py-4 text-sm text-muted-foreground">{t("profile.loadingVideos")}</p>
                    ) : users.length === 0 ? (
                        <p className="py-4 text-sm text-muted-foreground">{emptyMessage}</p>
                    ) : (
                        <div className="space-y-1">
                            {users.map((user) => {
                                const avatar = getAvatarUrl(user.avatar);
                                return (
                                    <div key={user.id} className="flex items-center gap-3 rounded-md px-2 py-2">
                                        <div className="h-9 w-9 shrink-0 overflow-hidden rounded-full bg-neutral-700">
                                            {avatar ? (
                                                <img src={avatar} alt={user.username} className="h-full w-full object-cover"/>
                                            ) : (
                                                <div className="flex h-full w-full items-center justify-center text-sm font-medium">
                                                    {user.username[0]?.toUpperCase() ?? "?"}
                                                </div>
                                            )}
                                        </div>
                                        <span className="text-sm font-medium">@{user.username}</span>
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </div>
                {hasNext && (
                    <Button variant="outline" onClick={loadNextPage} disabled={isLoading}>
                        {isLoading ? t("profile.loadingVideos") : t("profile.loadMore")}
                    </Button>
                )}
            </DialogContent>
        </Dialog>
    );
};

export default FollowListDialog;
