import {useState} from "react";
import {Popover} from "radix-ui";
import {Bell, CheckCheck, Loader2} from "lucide-react";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router-dom";
import {toast} from "sonner";
import {Button} from "@/components/ui/button";
import UserAvatar from "@/components/ui/UserAvatar";
import {useNotificationConnection} from "@/hooks/useNotificationConnection";
import {useGetNotificationsInfiniteQuery, useGetUnreadNotificationCountQuery,
    useMarkAllNotificationsReadMutation, useMarkNotificationReadMutation} from "@/store/apis/notificationApi";
import {useLazyGetConversationQuery} from "@/store/apis/conversationApi";
import {notificationType, type NotificationDto} from "@/types/Notification";
import {cn} from "@/lib/utils";

export default function NotificationBell() {
    const {t, i18n} = useTranslation();
    const navigate = useNavigate();
    const [open, setOpen] = useState(false);
    const [openingId, setOpeningId] = useState<string | null>(null);
    useNotificationConnection();
    const {data: unread} = useGetUnreadNotificationCountQuery(undefined, {
        pollingInterval: 30000, refetchOnFocus: true, refetchOnReconnect: true,
    });
    const {data, isLoading, isError, isFetching, hasNextPage, fetchNextPage, refetch} =
        useGetNotificationsInfiniteQuery(undefined, {
            skip: !open, pollingInterval: 30000, refetchOnFocus: true, refetchOnReconnect: true,
        });
    const [markRead] = useMarkNotificationReadMutation();
    const [markAllRead, {isLoading: markingAll}] = useMarkAllNotificationsReadMutation();
    const [getConversation] = useLazyGetConversationQuery();
    const count = unread?.data ?? 0;
    const items = [...new Map((data?.pages.flatMap(page => page.data.items) ?? []).map(n => [n.id, n])).values()];

    const openNotification = async (notification: NotificationDto) => {
        if (openingId) return;
        setOpeningId(notification.id);
        try {
            if (!notification.readAt) await markRead(notification.id).unwrap();
            const type = notificationType(notification);
            if (type === "NewDMMessage" && notification.conversationId) {
                const conversation = await getConversation(notification.conversationId).unwrap();
                navigate("/messages", {state: {conversation: conversation.data}});
            } else if (type === "NewFollower" && notification.actor?.username) {
                navigate(`/@${encodeURIComponent(notification.actor.username)}`);
            } else if (notification.videoShortId) {
                navigate(`/video/${encodeURIComponent(notification.videoShortId)}`);
            } else {
                toast.info(t("notifications.unavailable"));
                return;
            }
            setOpen(false);
        } catch {
            toast.error(t("notifications.actionError"));
        } finally {
            setOpeningId(null);
        }
    };

    return <Popover.Root open={open} onOpenChange={setOpen}>
        <Popover.Trigger asChild>
            <Button variant="ghost" size="icon" className="relative" aria-label={count > 0
                ? t("notifications.unreadLabel", {count}) : t("notifications.title")}>
                <Bell className="h-5 w-5"/>
                {count > 0 && <span aria-hidden="true" className="absolute -right-0.5 -top-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-destructive px-1 text-[10px] font-semibold text-white">
                    {count > 99 ? "99+" : count}
                </span>}
            </Button>
        </Popover.Trigger>
        <Popover.Portal>
            <Popover.Content align="end" side="bottom" sideOffset={8} collisionPadding={12}
                             aria-label={t("notifications.title")}
                             className="z-[60] flex h-[440px] max-h-[var(--radix-popover-content-available-height)] w-[360px] max-w-[calc(100vw-24px)] flex-col overflow-hidden rounded-xl border bg-popover text-popover-foreground shadow-xl">
                <div className="flex shrink-0 items-center justify-between gap-2 border-b px-4 py-3">
                    <h2 className="font-semibold">{t("notifications.title")}</h2>
                    <Button variant="ghost" size="sm" disabled={!count || markingAll} onClick={async () => {
                        try { await markAllRead().unwrap(); }
                        catch { toast.error(t("notifications.actionError")); }
                    }}>
                        <CheckCheck className="h-4 w-4"/>{t("notifications.markAllRead")}
                    </Button>
                </div>
                <div className="min-h-0 flex-1 overflow-y-auto overscroll-contain" data-testid="notification-scroll"
                     onScroll={event => {
                         const el = event.currentTarget;
                         if (el.scrollHeight - el.scrollTop - el.clientHeight < 80 && hasNextPage && !isFetching && !isError)
                             void fetchNextPage();
                     }}>
                    {isLoading && <div role="status" className="flex justify-center p-10"><Loader2 className="h-5 w-5 animate-spin"/><span className="sr-only">{t("notifications.loading")}</span></div>}
                    {isError && <div role="alert" className="space-y-2 p-6 text-center text-sm">
                        <p>{t("notifications.loadError")}</p><Button variant="outline" size="sm" onClick={() => void refetch()}>{t("notifications.retry")}</Button>
                    </div>}
                    {!isLoading && !isError && items.length === 0 && <div className="flex h-full flex-col items-center justify-center gap-3 p-6 text-center text-muted-foreground">
                        <Bell className="h-8 w-8"/><p className="text-sm">{t("notifications.empty")}</p>
                    </div>}
                    <ul>
                        {items.map(notification => <li key={notification.id}>
                            <button type="button" disabled={openingId !== null} onClick={() => void openNotification(notification)}
                                    className={cn("flex w-full items-start gap-3 border-b p-4 text-left transition-colors hover:bg-accent focus-visible:bg-accent focus-visible:outline-none disabled:opacity-60",
                                        !notification.readAt && "bg-primary/5")}>
                                <UserAvatar username={notification.actor?.username ?? "?"} avatar={notification.actor?.avatar} size="sm"/>
                                <span className="min-w-0 flex-1">
                                    <span className="block break-words text-sm"><span className="font-semibold">{notification.actor?.username ?? t("notifications.someone")}</span>{" "}
                                        {t(`notifications.events.${notificationType(notification)}`, {defaultValue: t("notifications.activity")})}</span>
                                    <time dateTime={notification.createdAt} className="mt-1 block text-xs text-muted-foreground">
                                        {new Date(notification.createdAt).toLocaleString(i18n.language, {month: "short", day: "numeric", hour: "2-digit", minute: "2-digit"})}
                                    </time>
                                </span>
                                {!notification.readAt && <span className="mt-2 h-2 w-2 shrink-0 rounded-full bg-primary"><span className="sr-only">{t("notifications.unread")}</span></span>}
                            </button>
                        </li>)}
                    </ul>
                    {hasNextPage && <div className="p-3 text-center"><Button variant="ghost" size="sm" disabled={isFetching} onClick={() => void fetchNextPage()}>
                        {isFetching ? <Loader2 className="h-4 w-4 animate-spin"/> : t("notifications.loadMore")}
                    </Button></div>}
                </div>
            </Popover.Content>
        </Popover.Portal>
    </Popover.Root>;
}
