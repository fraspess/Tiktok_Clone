import {useEffect, useMemo, useRef} from "react";
import {Link} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Loader2} from "lucide-react";
import {Dialog, DialogContent, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {useInfiniteFollowList} from "@/hooks/useInfiniteFollowList.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";

interface FollowListDialogProps {
    username: string;
    type: "followers" | "following";
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

const FollowListDialog = ({username, type, open, onOpenChange}: FollowListDialogProps) => {
    const {t} = useTranslation();
    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);

    const {users, loadMore, hasNext, isFetching} = useInfiniteFollowList(open ? username : undefined, type, 20);

    useEffect(() => {
        if (open) loadMore();
    }, [open, username, type]);

    const sentinelOptions = useMemo(() => ({root: containerRef, rootMargin: "200px"}), []);
    const isSentinelVisible = useIntersectionObserver(sentinelRef, sentinelOptions);

    useEffect(() => {
        if (isSentinelVisible && hasNext && !isFetching) {
            loadMore();
        }
    }, [isSentinelVisible, hasNext, isFetching, loadMore]);

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>
                        {type === "followers" ? t("profile.followers") : t("profile.following")}
                    </DialogTitle>
                </DialogHeader>

                <div ref={containerRef} className="max-h-[400px] overflow-y-auto">
                    {users.length === 0 && isFetching ? (
                        <div className="flex items-center justify-center py-8 text-muted-foreground">
                            <Loader2 className="h-5 w-5 animate-spin"/>
                        </div>
                    ) : users.length === 0 ? (
                        <p className="py-8 text-center text-sm text-muted-foreground">
                            {type === "followers" ? t("profile.noFollowers") : t("profile.noFollowing")}
                        </p>
                    ) : (
                        <div className="flex flex-col gap-1">
                            {users.map((user) => (
                                <Link
                                    key={user.id}
                                    to={`/@${user.username}`}
                                    onClick={() => onOpenChange(false)}
                                    className="flex items-center gap-3 rounded-lg p-2 hover:bg-neutral-100 dark:hover:bg-neutral-900"
                                >
                                    <div className="h-10 w-10 shrink-0 overflow-hidden rounded-full bg-neutral-700">
                                        {user.avatar?.small ? (
                                            <img src={user.avatar.small} alt={user.username} className="h-full w-full object-cover"/>
                                        ) : (
                                            <div className="flex h-full w-full items-center justify-center text-sm font-semibold text-white">
                                                {user.username[0]?.toUpperCase() ?? "?"}
                                            </div>
                                        )}
                                    </div>
                                    <span className="font-medium">@{user.username}</span>
                                </Link>
                            ))}
                            {hasNext && (
                                <div ref={sentinelRef} className="flex justify-center py-3">
                                    {isFetching && <Loader2 className="h-4 w-4 animate-spin text-muted-foreground"/>}
                                </div>
                            )}
                        </div>
                    )}
                </div>
            </DialogContent>
        </Dialog>
    );
};

export default FollowListDialog;