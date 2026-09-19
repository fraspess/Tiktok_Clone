import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {ShieldCheck, ShieldX} from "lucide-react";
import {
    adminApi,
    useBanUserMutation,
    useBanVideoMutation,
    useGetAdminReportsQuery,
    useGetAdminUsersQuery,
    useGetAdminVideosQuery,
    useGetReportReasonsQuery,
    useMarkReportAsResolvedMutation,
    useUnbanUserMutation,
    useUnbanVideoMutation,
} from "@/store/apis/adminApi.ts";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "@/components/ui/tabs.tsx";
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card.tsx";
import {Button} from "@/components/ui/button.tsx";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog.tsx";
import {RadioGroup} from "radix-ui";
import {cn} from "@/lib/utils.ts";
import {getAvatarUrl, getMediaUrl} from "@/lib/getAvatarUrl.ts";
import type {
    AdminReportDto,
    AvatarDto,
    EnumValueDto,
    ReportType,
    SimpleUserDto,
    SimpleVideoDto
} from "@/types/Admin.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";

const PAGE_SIZE = 10;
type BanFilter = "all" | "banned" | "active";

function useApiErrorMessage() {
    return (err: unknown, fallback: string) => {
        const message =
            isFetchBaseQueryError(err) && typeof err.data === "object" && err.data && "message" in err.data
                ? String((err.data as { message?: string }).message)
                : null;
        toast.error(message || fallback);
    };
}

function PaginationControls({
                                hasNext,
                                hasPrevious,
                                currentPage,
                                onPageChange,
                            }: {
    hasNext: boolean;
    hasPrevious: boolean;
    currentPage: number;
    onPageChange: (page: number) => void;
}) {
    const {t} = useTranslation();
    return (
        <div className="flex items-center justify-between px-6 pt-2">
            <Button variant="outline" size="sm" disabled={!hasPrevious} onClick={() => onPageChange(currentPage - 1)}>
                {t("admin.previous")}
            </Button>
            <span className="text-sm text-muted-foreground">{currentPage}</span>
            <Button variant="outline" size="sm" disabled={!hasNext} onClick={() => onPageChange(currentPage + 1)}>
                {t("admin.next")}
            </Button>
        </div>
    );
}

function StatusFilter({value, onChange}: { value: BanFilter; onChange: (value: BanFilter) => void }) {
    const {t} = useTranslation();
    const filters: BanFilter[] = ["all", "active", "banned"];

    return (
        <div className="flex gap-1 px-6 pt-3">
            {filters.map((filter) => (
                <Button key={filter} variant={value === filter ? "secondary" : "ghost"} size="sm"
                        onClick={() => onChange(filter)}>
                    {t(`admin.filters.${filter}`)}
                </Button>
            ))}
        </div>
    );
}

function UserRow({user, filter}: { user: SimpleUserDto; filter: BanFilter }) {
    const {t} = useTranslation();
    const reportError = useApiErrorMessage();
    const {data: detail} = adminApi.useGetAdminUserQuery(user.id);
    const {data: reasons} = useGetReportReasonsQuery("User");
    const [banUser, {isLoading: banning}] = useBanUserMutation();
    const [unbanUser, {isLoading: unbanning}] = useUnbanUserMutation();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [selectedReason, setSelectedReason] = useState<string>("");

    const isBanned = detail?.data?.isBanned ?? false;
    const avatar = getAvatarUrl(user.avatar);

    if (filter === "banned" && !isBanned) return null;
    if (filter === "active" && isBanned) return null;

    const handleBan = async () => {
        if (!selectedReason) {
            toast.error(t("admin.selectReason"));
            return;
        }
        try {
            await banUser({id: user.id, reason: Number(selectedReason)}).unwrap();
            toast.success(t("admin.userBanned"));
            setDialogOpen(false);
            setSelectedReason("");
        } catch (err) {
            reportError(err, t("admin.banError"));
        }
    };

    const handleUnban = async () => {
        try {
            await unbanUser(user.id).unwrap();
            toast.success(t("admin.userUnbanned"));
        } catch (err) {
            reportError(err, t("admin.unbanError"));
        }
    };

    return (
        <div className="flex items-center justify-between border-b border-neutral-800 px-6 py-3 last:border-b-0">
            <div className="flex items-center gap-3">
                <div className="relative h-9 w-9 shrink-0 overflow-hidden rounded-full bg-neutral-700">
                    <div className="flex h-full w-full items-center justify-center text-sm font-medium">
                        {user.username.charAt(0).toUpperCase()}
                    </div>
                    {avatar && (
                        <img
                            src={getMediaUrl(avatar)}
                            alt={user.username}
                            loading="lazy"
                            className="absolute inset-0 h-full w-full rounded-full object-cover"
                            onError={(e) => e.currentTarget.remove()}
                        />
                    )}
                </div>
                <div>
                    <p className="text-sm font-medium">@{user.username}</p>
                    {isBanned && <p className="text-xs text-destructive">{t("admin.banned")}</p>}
                </div>
            </div>
            <div className="flex items-center gap-2">
                {!isBanned ? (
                    <Button variant="destructive" size="sm" onClick={() => setDialogOpen(true)}>
                        <ShieldX/> {t("admin.ban")}
                    </Button>
                ) : (
                    <Button variant="outline" size="sm" onClick={handleUnban} disabled={unbanning}>
                        <ShieldCheck/> {t("admin.unban")}
                    </Button>
                )}
            </div>

            <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>{t("admin.banUserTitle")}</DialogTitle>
                        <DialogDescription>@{user.username}</DialogDescription>
                    </DialogHeader>
                    <RadioGroup.Root value={selectedReason} onValueChange={setSelectedReason}
                                     className="flex flex-col gap-2">
                        {(reasons?.data ?? []).map((reason) => (
                            <label
                                key={reason.id}
                                htmlFor={`user-ban-reason-${reason.id}`}
                                className={cn(
                                    "flex cursor-pointer items-center gap-3 rounded-md border border-border px-3 py-2 text-sm transition-colors hover:bg-muted",
                                    selectedReason === String(reason.id) && "border-primary bg-muted"
                                )}
                            >
                                <RadioGroup.Item
                                    id={`user-ban-reason-${reason.id}`}
                                    value={String(reason.id)}
                                    className="flex h-4 w-4 shrink-0 items-center justify-center rounded-full border border-input"
                                >
                                    <RadioGroup.Indicator className="h-2 w-2 rounded-full bg-primary"/>
                                </RadioGroup.Item>
                                {reason.description ?? reason.name}
                            </label>
                        ))}
                    </RadioGroup.Root>
                    <DialogFooter>
                        <Button variant="outline" onClick={() => setDialogOpen(false)}>
                            {t("report.cancel")}
                        </Button>
                        <Button variant="destructive" onClick={handleBan} disabled={banning || !selectedReason}>
                            {t("admin.ban")}
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </div>
    );
}

const UsersTab = () => {
    const {t} = useTranslation();
    const [page, setPage] = useState(1);
    const [filter, setFilter] = useState<BanFilter>("all");
    const {data, isLoading, isError} = useGetAdminUsersQuery({pageNumber: page, pageSize: PAGE_SIZE});

    if (isLoading) return <p className="px-6 py-4 text-sm text-muted-foreground">{t("admin.loading")}</p>;
    if (isError) return <p className="px-6 py-4 text-sm text-destructive">{t("admin.loadError")}</p>;

    const users = data?.data?.items ?? [];

    return (
        <div className="flex h-full flex-col">
            <StatusFilter value={filter} onChange={setFilter}/>
            <div className="flex-1 overflow-y-auto">
                {users.length === 0 ? (
                    <p className="px-6 py-4 text-sm text-muted-foreground">{t("admin.empty")}</p>
                ) : (
                    users.map((user) => <UserRow key={user.id} user={user} filter={filter}/>)
                )}
            </div>
            <PaginationControls
                hasNext={data?.data?.metadata.hasNext ?? false}
                hasPrevious={data?.data?.metadata.hasPrevious ?? false}
                currentPage={page}
                onPageChange={setPage}
            />
        </div>
    );
};

const VideoRow = ({video}: { video: SimpleVideoDto }) => {
    const {t} = useTranslation();
    const reportError = useApiErrorMessage();
    const {data: reasons} = useGetReportReasonsQuery("Video");
    const [banVideo, {isLoading: banning}] = useBanVideoMutation();
    const [unbanVideo, {isLoading: unbanning}] = useUnbanVideoMutation();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [selectedReason, setSelectedReason] = useState<string>("");

    const thumbnail = getMediaUrl(video.thumbnailUrl);
    const isBanned = video.isBanned;

    const handleBan = async () => {
        if (!selectedReason) {
            toast.error(t("admin.selectReason"));
            return;
        }
        try {
            await banVideo({id: video.id, reason: Number(selectedReason)}).unwrap();
            toast.success(t("admin.videoBanned"));
            setDialogOpen(false);
            setSelectedReason("");
        } catch (err) {
            reportError(err, t("admin.banError"));
        }
    };

    const handleUnban = async () => {
        try {
            await unbanVideo(video.id).unwrap();
            toast.success(t("admin.videoUnbanned"));
        } catch (err) {
            reportError(err, t("admin.unbanError"));
        }
    };

    return (
        <div className="flex items-center justify-between border-b border-neutral-800 px-6 py-3 last:border-b-0">
            <div className="flex min-w-0 items-center gap-3">
                <div className="relative h-14 w-10 shrink-0 overflow-hidden rounded-md bg-neutral-700">
                    {thumbnail && (
                        <img
                            src={thumbnail}
                            alt=""
                            loading="lazy"
                            className="absolute inset-0 h-full w-full object-cover"
                            onError={(e) => e.currentTarget.remove()}
                        />
                    )}
                </div>
                <div className="min-w-0">
                    <p className="truncate text-sm font-medium">{video.description || t(t("admin.noVideoDescription"))}</p>
                    <p className="truncate text-xs text-muted-foreground">
                        @{video.author?.username ?? "?"}
                        {video.hashTags.map((tag) => ` #${tag}`).join("")}
                    </p>
                    <p className="text-xs text-muted-foreground">{t("admin.views", {count: video.viewCount})}</p>
                    {isBanned && <p className="text-xs text-destructive">{t("admin.banned")}</p>}
                </div>
            </div>
            <div className="flex items-center gap-2">
                {!isBanned ? (
                    <Button variant="destructive" size="sm" onClick={() => setDialogOpen(true)}>
                        <ShieldX/> {t("admin.ban")}
                    </Button>
                ) : (
                    <Button variant="outline" size="sm" onClick={handleUnban} disabled={unbanning}>
                        <ShieldCheck/> {t("admin.unban")}
                    </Button>
                )}
            </div>

            <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>{t("admin.banVideoTitle")}</DialogTitle>
                        <DialogDescription>{video.description || video.id}</DialogDescription>
                    </DialogHeader>
                    <RadioGroup.Root value={selectedReason} onValueChange={setSelectedReason}
                                     className="flex flex-col gap-2">
                        {(reasons?.data ?? []).map((reason) => (
                            <label
                                key={reason.id}
                                htmlFor={`video-ban-reason-${reason.id}`}
                                className={cn(
                                    "flex cursor-pointer items-center gap-3 rounded-md border border-border px-3 py-2 text-sm transition-colors hover:bg-muted",
                                    selectedReason === String(reason.id) && "border-primary bg-muted"
                                )}
                            >
                                <RadioGroup.Item
                                    id={`video-ban-reason-${reason.id}`}
                                    value={String(reason.id)}
                                    className="flex h-4 w-4 shrink-0 items-center justify-center rounded-full border border-input"
                                >
                                    <RadioGroup.Indicator className="h-2 w-2 rounded-full bg-primary"/>
                                </RadioGroup.Item>
                                {reason.description ?? reason.name}
                            </label>
                        ))}
                    </RadioGroup.Root>
                    <DialogFooter>
                        <Button variant="outline" onClick={() => setDialogOpen(false)}>
                            {t("report.cancel")}
                        </Button>
                        <Button variant="destructive" onClick={handleBan} disabled={banning || !selectedReason}>
                            {t("admin.ban")}
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </div>
    );
};

const VideosTab = () => {
    const {t} = useTranslation();
    const [page, setPage] = useState(1);
    const [filter, setFilter] = useState<BanFilter>("all");
    const {data, isLoading, isError} = useGetAdminVideosQuery({pageNumber: page, pageSize: PAGE_SIZE});

    if (isLoading) return <p className="px-6 py-4 text-sm text-muted-foreground">{t("admin.loading")}</p>;
    if (isError) return <p className="px-6 py-4 text-sm text-destructive">{t("admin.loadError")}</p>;

    const videos = (data?.data?.items ?? []).filter((video) =>
        filter === "all" || (filter === "banned" ? video.isBanned : !video.isBanned)
    );

    return (
        <div className="flex h-full flex-col">
            <StatusFilter value={filter} onChange={setFilter}/>
            <div className="flex-1 overflow-y-auto">
                {videos.length === 0 ? (
                    <p className="px-6 py-4 text-sm text-muted-foreground">{t("admin.empty")}</p>
                ) : (
                    videos.map((video) => <VideoRow key={video.id} video={video}/>)
                )}
            </div>
            <PaginationControls
                hasNext={data?.data?.metadata.hasNext ?? false}
                hasPrevious={data?.data?.metadata.hasPrevious ?? false}
                currentPage={page}
                onPageChange={setPage}
            />
        </div>
    );
};

const REPORT_TYPES: ReportType[] = ["Video", "User", "Comment"];

function BanReportAction({
                             report,
                             reportType,
                             onBlocked,
                         }: {
    report: AdminReportDto;
    reportType: Exclude<ReportType, "Comment">;
    onBlocked: () => void;
}) {
    const {t} = useTranslation();
    const reportError = useApiErrorMessage();
    const {data: reasons} = useGetReportReasonsQuery(reportType);
    const [banUser, {isLoading: isBanningUser}] = useBanUserMutation();
    const [banVideo, {isLoading: isBanningVideo}] = useBanVideoMutation();
    const [open, setOpen] = useState(false);
    const [reason, setReason] = useState("");
    const isLoading = isBanningUser || isBanningVideo;

    useEffect(() => {
        if (!open || !report.reason || !reasons?.data) return;

        const reportReason = report.reason.trim().toLocaleLowerCase();
        const matchingReason = reasons.data.find((item) =>
            [item.description, item.name]
                .filter(Boolean)
                .some((value) => value?.trim().toLocaleLowerCase() === reportReason)
        );

        setReason(matchingReason ? String(matchingReason.id) : "");
    }, [open, report.reason, reasons?.data]);

    const handleBan = async () => {
        if (!reason || !report.reportedContent?.id) return;
        try {
            if (reportType === "User") {
                await banUser({id: report.reportedContent.id, reason: Number(reason)}).unwrap();
            } else {
                await banVideo({id: report.reportedContent.id, reason: Number(reason)}).unwrap();
            }
            toast.success(reportType === "User" ? t("admin.userBanned") : t("admin.videoBanned"));
            setOpen(false);
            setReason("");
            onBlocked();
        } catch (err) {
            reportError(err, t("admin.banError"));
        }
    };

    return (
        <>
            <Button variant="destructive" size="sm" onClick={() => setOpen(true)}
                    disabled={!report.reportedContent?.id}>
                <ShieldX/> {t("admin.ban")}
            </Button>
            <Dialog open={open} onOpenChange={setOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>{reportType === "User" ? t("admin.banUserTitle") : t("admin.banVideoTitle")}</DialogTitle>
                        <DialogDescription>{report.reportedContent?.title ?? report.reportedContent?.id}</DialogDescription>
                    </DialogHeader>
                    <RadioGroup.Root value={reason} onValueChange={setReason} className="flex flex-col gap-2">
                        {(reasons?.data ?? []).map((item: EnumValueDto) => (
                            <label key={item.id}
                                   className={cn("flex cursor-pointer items-center gap-3 rounded-md border border-border px-3 py-2 text-sm", reason === String(item.id) && "border-primary bg-muted")}>
                                <RadioGroup.Item value={String(item.id)}
                                                 className="flex h-4 w-4 shrink-0 items-center justify-center rounded-full border border-input">
                                    <RadioGroup.Indicator className="h-2 w-2 rounded-full bg-primary"/>
                                </RadioGroup.Item>
                                {item.description ?? item.name}
                            </label>
                        ))}
                    </RadioGroup.Root>
                    <DialogFooter>
                        <Button variant="outline" onClick={() => setOpen(false)}>{t("report.cancel")}</Button>
                        <Button variant="destructive" onClick={handleBan}
                                disabled={!reason || isLoading}>{t("admin.ban")}</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    );
}

const ReportsTab = () => {
    const {t} = useTranslation();
    const [reportType, setReportType] = useState<ReportType>("Video");
    const [page, setPage] = useState(1);
    const [blocked, setBlocked] = useState<Set<string>>(new Set<string>());
    const [processed, setProcessed] = useState<Set<string>>(new Set<string>());
    const [markAsResolved, {isLoading: isResolving}] = useMarkReportAsResolvedMutation();

    const handleResolve = async (reportId: string) => {
        try {
            await markAsResolved(reportId).unwrap();

            toast.success("Report resolved");
            setProcessed((prev) => new Set(prev).add(reportId));
        } catch (err) {
            console.error(err);
            reportError("Failed to resolve report");
        }
    };

    const {data, isLoading, isError} = useGetAdminReportsQuery({
        reportType,
        pageNumber: page,
        pageSize: PAGE_SIZE,
    }, {
        pollingInterval: 3000,
        refetchOnFocus: true,
        refetchOnMountOrArgChange: true,
    });

    const markBlocked = (id: string) => {
        setBlocked((prev) => {
            const next = new Set(prev).add(id);
            try {
                localStorage.setItem("admin-blocked-reports", JSON.stringify([...next]));
            } catch {
                // ignore storage failures
            }
            return next;
        });
    };

    if (isLoading) return <p className="px-6 py-4 text-sm text-muted-foreground">{t("admin.loading")}</p>;
    if (isError) return <p className="px-6 py-4 text-sm text-destructive">{t("admin.loadError")}</p>;

    const reports = data?.data?.items ?? [];

    return (
        <div className="flex h-full flex-col">
            <div className="flex gap-1 px-6 pt-3">
                {REPORT_TYPES.map((type) => (
                    <Button
                        key={type}
                        variant={reportType === type ? "secondary" : "ghost"}
                        size="sm"
                        onClick={() => {
                            setReportType(type);
                            setPage(1);
                        }}
                    >
                        {t(`admin.types.${type.toLowerCase()}`)}
                    </Button>
                ))}
            </div>
            <div className="flex-1 overflow-y-auto pt-2">
                {reports.length === 0 ? (
                    <p className="px-6 py-4 text-sm text-muted-foreground">{t("admin.empty")}</p>
                ) : (
                    reports.map((report) => {
                        const isDone = processed.has(report.id);
                        const isBlocked = blocked.has(report.id);
                        const reporter = report.reportedBy;
                        const content = report.reportedContent;
                        const reportedAvatar = report.reportedContent?.thumbnail
                            ? getAvatarUrl(report.reportedContent.thumbnail as AvatarDto | string | null)
                            : null;
                        return (
                            <div
                                key={report.id}
                                className={cn(
                                    "flex items-center justify-between gap-3 border-b border-neutral-800 px-6 py-3 last:border-b-0",
                                    isDone && "opacity-50"
                                )}
                            >
                                <div className="min-w-0">
                                    <p className="text-sm font-medium">
                                        {t("admin.reportedBy")}: @{reporter?.username ?? "?"}
                                    </p>
                                    {content?.title &&
                                        <p className="truncate text-xs text-muted-foreground">{content.title}</p>}
                                    {reportedAvatar && (
                                        <img src={getMediaUrl(reportedAvatar)} alt="" loading="lazy"
                                             className="mt-1 h-10 w-10 rounded-md object-cover"
                                             onError={(e) => e.currentTarget.remove()}/>
                                    )}
                                    <p className="text-xs text-muted-foreground">
                                        {t("admin.reason")}: {report.reason ?? "?"} · {new Date(report.createdAt).toLocaleString()}
                                    </p>
                                </div>
                                <div className="flex shrink-0 items-center gap-2">
                                    {isBlocked ? (
                                        <span
                                            className="text-xs font-medium text-destructive">{t("admin.blocked")}</span>
                                    ) : reportType !== "Comment" ? (
                                        <BanReportAction report={report} reportType={reportType}
                                                         onBlocked={() => markBlocked(report.id)}/>
                                    ) : null}
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        disabled={isResolving}
                                        onClick={() => handleResolve(report.id)}
                                    >
                                        {isResolving ? "Resolving..." : "Mark as Resolved"}
                                    </Button>
                                </div>
                            </div>
                        );
                    })
                )}
            </div>
            <PaginationControls
                hasNext={data?.data?.metadata.hasNext ?? false}
                hasPrevious={data?.data?.metadata.hasPrevious ?? false}
                currentPage={page}
                onPageChange={setPage}
            />
        </div>
    );
};

const AdminPage = () => {
    const {t} = useTranslation();

    return (
        <Card className="h-full">
            <CardHeader className="border-b border-neutral-800">
                <CardTitle>{t("admin.title")}</CardTitle>
            </CardHeader>
            <CardContent className="flex h-[calc(100%-4rem)] flex-col p-0">
                <Tabs defaultValue="reports" className="flex h-full flex-col">
                    <TabsList className="px-6">
                        <TabsTrigger value="users">{t("admin.users")}</TabsTrigger>
                        <TabsTrigger value="videos">{t("admin.videos")}</TabsTrigger>
                        <TabsTrigger value="reports">{t("admin.reports")}</TabsTrigger>
                    </TabsList>
                    <TabsContent value="users" className="flex-1 overflow-hidden">
                        <UsersTab/>
                    </TabsContent>
                    <TabsContent value="videos" className="flex-1 overflow-hidden">
                        <VideosTab/>
                    </TabsContent>
                    <TabsContent value="reports" className="flex-1 overflow-hidden">
                        <ReportsTab/>
                    </TabsContent>
                </Tabs>
            </CardContent>
        </Card>
    );
};

export default AdminPage;
