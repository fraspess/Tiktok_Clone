import {useState} from "react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Loader2, Search, ShieldCheck, ShieldX} from "lucide-react";
import {
    useBanUserMutation,
    useBanVideoMutation,
    useGetAdminReportsQuery,
    useGetAdminUsersQuery,
    useGetAdminVideosQuery,
    useGetAdminUserVideosQuery,
    useGetReportReasonsQuery,
    useMarkReportAsResolvedMutation,
    useUnbanUserMutation,
    useUnbanVideoMutation,
} from "@/store/apis/adminApi.ts";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "@/components/ui/tabs.tsx";
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card.tsx";
import {Button} from "@/components/ui/button.tsx";
import {Input} from "@/components/ui/input.tsx";
import {Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {RadioGroup} from "radix-ui";
import {cn} from "@/lib/utils.ts";
import {getAvatarUrl, getMediaUrl} from "@/lib/getAvatarUrl.ts";
import {useDebounce} from "@/hooks/useDebounce.ts";
import type {AdminReportDto, EnumValueDto, ReportType, SimpleUserDto, SimpleVideoDto} from "@/types/Admin.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";

const PAGE_SIZE = 10;
type BanFilter = "all" | "banned" | "active";

function useApiErrorMessage() {
    return (err: unknown, fallback: string) => {
        const message = isFetchBaseQueryError(err) && typeof err.data === "object" && err.data && "message" in err.data
            ? String((err.data as {message?: string}).message)
            : null;
        toast.error(message || fallback);
    };
}

function getUsername(username: string | null | undefined) {
    return username?.trim() || "?";
}

function PaginationControls({hasNext, hasPrevious, currentPage, totalPages, onPageChange}: {hasNext: boolean; hasPrevious: boolean; currentPage: number; totalPages?: number; onPageChange: (page: number) => void}) {
    const {t} = useTranslation();
    return <div className="flex items-center justify-between border-t border-border px-3 sm:px-6 py-3">
        <Button variant="outline" size="sm" disabled={!hasPrevious} onClick={() => onPageChange(currentPage - 1)}>{t("admin.previous")}</Button>
        <span className="text-sm text-muted-foreground">{t("admin.page", {current: currentPage, total: totalPages || currentPage})}</span>
        <Button variant="outline" size="sm" disabled={!hasNext} onClick={() => onPageChange(currentPage + 1)}>{t("admin.next")}</Button>
    </div>;
}

function StatusFilter({value, onChange}: {value: BanFilter; onChange: (value: BanFilter) => void}) {
    const {t} = useTranslation();
    return <div className="flex flex-wrap gap-1">{(["all", "active", "banned"] as BanFilter[]).map((filter) =>
        <Button key={filter} variant={value === filter ? "secondary" : "ghost"} size="sm" onClick={() => onChange(filter)}>{t(`admin.filters.${filter}`)}</Button>
    )}</div>;
}

function ModerationReasonDialog({open, onOpenChange, contentType, title, description, onConfirm, isLoading}: {open: boolean; onOpenChange: (open: boolean) => void; contentType: Exclude<ReportType, "Comment">; title: string; description: string; onConfirm: (reason: number) => Promise<void>; isLoading: boolean}) {
    const {t} = useTranslation();
    const {data: reasons} = useGetReportReasonsQuery(contentType, {skip: !open});
    const [selectedReason, setSelectedReason] = useState("");
    const confirm = async () => {
        if (!selectedReason) {
            toast.error(t("admin.selectReason"));
            return;
        }
        await onConfirm(Number(selectedReason));
    };
    const changeOpen = (nextOpen: boolean) => {
        if (!nextOpen) setSelectedReason("");
        onOpenChange(nextOpen);
    };
    return <Dialog open={open} onOpenChange={changeOpen}><DialogContent><DialogHeader><DialogTitle>{title}</DialogTitle><DialogDescription>{description}</DialogDescription></DialogHeader>
        <RadioGroup.Root value={selectedReason} onValueChange={setSelectedReason} className="flex flex-col gap-2">{(reasons?.data ?? []).map((reason: EnumValueDto) =>
            <label key={reason.id} className={cn("flex cursor-pointer items-center gap-3 rounded-md border border-border px-3 py-2 text-sm transition-colors hover:bg-muted", selectedReason === String(reason.id) && "border-primary bg-muted")}><RadioGroup.Item value={String(reason.id)} className="flex h-4 w-4 shrink-0 items-center justify-center rounded-full border border-input"><RadioGroup.Indicator className="h-2 w-2 rounded-full bg-primary"/></RadioGroup.Item>{reason.description ?? reason.name ?? t("admin.unknownReason")}</label>
        )}</RadioGroup.Root>
        <DialogFooter><Button variant="outline" onClick={() => onOpenChange(false)}>{t("report.cancel")}</Button><Button variant="destructive" onClick={confirm} disabled={isLoading || !selectedReason}>{isLoading && <Loader2 className="animate-spin"/>}{t("admin.ban")}</Button></DialogFooter>
    </DialogContent></Dialog>;
}

function UserRow({user}: {user: SimpleUserDto}) {
    const {t} = useTranslation();
    const showError = useApiErrorMessage();
    const [banUser, {isLoading: banning}] = useBanUserMutation();
    const [unbanUser, {isLoading: unbanning}] = useUnbanUserMutation();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [videosOpen, setVideosOpen] = useState(false);
    const username = getUsername(user.username);
    const avatar = getAvatarUrl(user.avatar);
    const ban = async (reason: number) => { try { await banUser({id: user.id, reason}).unwrap(); toast.success(t("admin.userBanned")); setDialogOpen(false); } catch (error) { showError(error, t("admin.banError")); } };
    const unban = async () => { try { await unbanUser(user.id).unwrap(); toast.success(t("admin.userUnbanned")); } catch (error) { showError(error, t("admin.unbanError")); } };
    return <div className="flex items-center justify-between gap-3 border-b border-border px-3 sm:px-6 py-3 last:border-b-0"><div className="flex min-w-0 items-center gap-3"><div className="relative flex h-9 w-9 shrink-0 items-center justify-center overflow-hidden rounded-full bg-muted text-sm font-medium">{username.charAt(0).toUpperCase()}{avatar && <img src={getMediaUrl(avatar)} alt={username} loading="lazy" className="absolute inset-0 h-full w-full object-cover" onError={(e) => e.currentTarget.remove()}/>}</div><div className="min-w-0"><p className="truncate text-sm font-medium">@{username}</p>{user.isBanned && <p className="text-xs text-destructive">{t("admin.banned")}</p>}</div></div>
        {user.isBanned ? <Button variant="outline" size="sm" onClick={unban} disabled={unbanning}>{unbanning && <Loader2 className="animate-spin"/>}<ShieldCheck/> {t("admin.unban")}</Button> : <Button variant="destructive" size="sm" onClick={() => setDialogOpen(true)}><ShieldX/> {t("admin.ban")}</Button>}
        <Button variant="outline" size="sm" onClick={() => setVideosOpen(true)}>{t("admin.userVideos")}</Button>
        <Dialog open={videosOpen} onOpenChange={setVideosOpen}><DialogContent className="max-h-[85dvh] overflow-y-auto sm:max-w-3xl">
            <DialogHeader><DialogTitle>{t("admin.userVideos")}</DialogTitle><DialogDescription>@{username}</DialogDescription></DialogHeader>
            {videosOpen && <UserVideos userId={user.id}/>}
        </DialogContent></Dialog>
        <ModerationReasonDialog open={dialogOpen} onOpenChange={setDialogOpen} contentType="User" title={t("admin.banUserTitle")} description={`@${username}`} onConfirm={ban} isLoading={banning}/>
    </div>;
}

function UserVideos({userId}: {userId: string}) {
    const {t} = useTranslation();
    const [page, setPage] = useState(1);
    const {currentData, isFetching, isError, refetch} = useGetAdminUserVideosQuery({id: userId, pageNumber: page, pageSize: PAGE_SIZE});
    const metadata = currentData?.data.metadata;
    return <div>
        {isFetching ? <p role="status">{t("admin.loading")}</p> : isError ?
            <div role="alert">{t("admin.loadError")} <Button onClick={() => void refetch()}>{t("chat.privacy.retry")}</Button></div> :
            currentData?.data.items.length ? currentData.data.items.map(video => <VideoRow key={video.id} video={video}/>) : <p>{t("admin.empty")}</p>}
        {!isFetching && <PaginationControls hasNext={metadata?.hasNext ?? false} hasPrevious={page > 1}
            currentPage={page} totalPages={metadata?.totalPages} onPageChange={setPage}/>}
    </div>;
}

function UsersTab() {
    const {t} = useTranslation();
    const [page, setPage] = useState(1);
    const [filter, setFilter] = useState<BanFilter>("all");
    const [search, setSearch] = useState("");
    const debouncedSearch = useDebounce(search, 350);
    const {data, isLoading, isFetching, isError} = useGetAdminUsersQuery({pageNumber: page, pageSize: PAGE_SIZE, search: debouncedSearch.trim() || undefined, isBanned: filter === "all" ? undefined : filter === "banned"});
    const users = data?.data?.items ?? [];
    const metadata = data?.data?.metadata;
    return <div className="flex h-full min-h-0 flex-col"><div className="space-y-3 border-b border-border px-3 sm:px-6 py-3"><div className="relative"><Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground"/><Input value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} placeholder={t("admin.searchUsers")} className="pl-9"/></div><StatusFilter value={filter} onChange={(value) => { setFilter(value); setPage(1); }}/></div><div className="min-h-0 flex-1 overflow-y-auto">
        {isLoading || isFetching ? <div className="flex justify-center py-8"><Loader2 className="animate-spin text-muted-foreground"/></div> : isError ? <p className="px-3 sm:px-6 py-4 text-sm text-destructive">{t("admin.loadError")}</p> : users.length === 0 ? <p className="px-3 sm:px-6 py-8 text-center text-sm text-muted-foreground">{search.trim() ? t("admin.noUsersFound") : t("admin.empty")}</p> : users.map((user) => <UserRow key={user.id} user={user}/>)}</div><PaginationControls hasNext={metadata?.hasNext ?? false} hasPrevious={metadata?.hasPrevious ?? false} currentPage={page} totalPages={metadata?.totalPages} onPageChange={setPage}/></div>;
}

function VideoRow({video}: {video: SimpleVideoDto}) {
    const {t} = useTranslation();
    const showError = useApiErrorMessage();
    const [banVideo, {isLoading: banning}] = useBanVideoMutation();
    const [unbanVideo, {isLoading: unbanning}] = useUnbanVideoMutation();
    const [dialogOpen, setDialogOpen] = useState(false);
    const description = video.description?.trim() || t("admin.noVideoDescription");
    const hashtags = Array.isArray(video.hashTags) ? video.hashTags.filter(Boolean) : [];
    const ban = async (reason: number) => { try { await banVideo({id: video.id, reason}).unwrap(); toast.success(t("admin.videoBanned")); setDialogOpen(false); } catch (error) { showError(error, t("admin.banError")); } };
    const unban = async () => { try { await unbanVideo(video.id).unwrap(); toast.success(t("admin.videoUnbanned")); } catch (error) { showError(error, t("admin.unbanError")); } };
    return <div className="flex items-center justify-between gap-3 border-b border-border px-3 sm:px-6 py-3 last:border-b-0"><div className="flex min-w-0 items-center gap-3"><div className="relative h-14 w-10 shrink-0 overflow-hidden rounded-md bg-muted">{getMediaUrl(video.thumbnailUrl) && <img src={getMediaUrl(video.thumbnailUrl)} alt="" loading="lazy" className="absolute inset-0 h-full w-full object-cover" onError={(e) => e.currentTarget.remove()}/>}</div><div className="min-w-0"><p className="truncate text-sm font-medium">{description}</p><p className="truncate text-xs text-muted-foreground">@{getUsername(video.author?.username)}{hashtags.map((tag) => ` #${tag}`).join("")}</p><p className="text-xs text-muted-foreground">{t("admin.views", {count: video.viewCount ?? 0})}</p>{video.isBanned && <p className="text-xs text-destructive">{t("admin.banned")}</p>}</div></div>
        {video.isBanned ? <Button variant="outline" size="sm" onClick={unban} disabled={unbanning}>{unbanning && <Loader2 className="animate-spin"/>}<ShieldCheck/> {t("admin.unban")}</Button> : <Button variant="destructive" size="sm" onClick={() => setDialogOpen(true)}><ShieldX/> {t("admin.ban")}</Button>}
        <ModerationReasonDialog open={dialogOpen} onOpenChange={setDialogOpen} contentType="Video" title={t("admin.banVideoTitle")} description={description} onConfirm={ban} isLoading={banning}/>
    </div>;
}

function VideosTab() {
    const {t} = useTranslation(); const [page, setPage] = useState(1); const [filter, setFilter] = useState<BanFilter>("all");
    const {data, isLoading, isError} = useGetAdminVideosQuery({pageNumber: page, pageSize: PAGE_SIZE, isBanned: filter === "all" ? undefined : filter === "banned"}); const metadata = data?.data?.metadata;
    const videos = data?.data?.items ?? [];
    return <div className="flex h-full min-h-0 flex-col"><div className="border-b border-border px-3 sm:px-6 py-3"><StatusFilter value={filter} onChange={(value) => { setFilter(value); setPage(1); }}/></div><div className="min-h-0 flex-1 overflow-y-auto">{isLoading ? <div className="flex justify-center py-8"><Loader2 className="animate-spin text-muted-foreground"/></div> : isError ? <p className="px-3 sm:px-6 py-4 text-sm text-destructive">{t("admin.loadError")}</p> : videos.length === 0 ? <p className="px-3 sm:px-6 py-8 text-center text-sm text-muted-foreground">{t("admin.empty")}</p> : videos.map((video) => <VideoRow key={video.id} video={video}/>)}</div><PaginationControls hasNext={metadata?.hasNext ?? false} hasPrevious={metadata?.hasPrevious ?? false} currentPage={page} totalPages={metadata?.totalPages} onPageChange={setPage}/></div>;
}

const REPORT_TYPES: ReportType[] = ["Video", "User", "Comment"];

function BanReportAction({report, reportType, onBanned}: {report: AdminReportDto; reportType: Exclude<ReportType, "Comment">; onBanned: () => void}) {
    const {t} = useTranslation(); const showError = useApiErrorMessage(); const [banUser, {isLoading: banningUser}] = useBanUserMutation(); const [banVideo, {isLoading: banningVideo}] = useBanVideoMutation(); const [open, setOpen] = useState(false); const [reason, setReason] = useState(""); const {data: reasons} = useGetReportReasonsQuery(reportType, {skip: !open});
    const ban = async () => { if (!reason || !report.reportedContent?.id) return; try { if (reportType === "User") await banUser({id: report.reportedContent.id, reason: Number(reason)}).unwrap(); else await banVideo({id: report.reportedContent.id, reason: Number(reason)}).unwrap(); toast.success(reportType === "User" ? t("admin.userBanned") : t("admin.videoBanned")); setOpen(false); onBanned(); } catch (error) { showError(error, t("admin.banError")); } };
    return <><Button variant="destructive" size="sm" onClick={() => { setReason(""); setOpen(true); }} disabled={!report.reportedContent?.id}><ShieldX/> {t("admin.ban")}</Button><Dialog open={open} onOpenChange={(nextOpen) => { if (!nextOpen) setReason(""); setOpen(nextOpen); }}><DialogContent><DialogHeader><DialogTitle>{reportType === "User" ? t("admin.banUserTitle") : t("admin.banVideoTitle")}</DialogTitle><DialogDescription>{report.reportedContent?.title ?? report.reportedContent?.id ?? t("admin.unavailableContent")}</DialogDescription></DialogHeader><RadioGroup.Root value={reason} onValueChange={setReason} className="flex flex-col gap-2">{(reasons?.data ?? []).map((item: EnumValueDto) => <label key={item.id} className={cn("flex cursor-pointer items-center gap-3 rounded-md border border-border px-3 py-2 text-sm", reason === String(item.id) && "border-primary bg-muted")}><RadioGroup.Item value={String(item.id)} className="flex h-4 w-4 shrink-0 items-center justify-center rounded-full border border-input"><RadioGroup.Indicator className="h-2 w-2 rounded-full bg-primary"/></RadioGroup.Item>{item.description ?? item.name ?? t("admin.unknownReason")}</label>)}</RadioGroup.Root><DialogFooter><Button variant="outline" onClick={() => setOpen(false)}>{t("report.cancel")}</Button><Button variant="destructive" onClick={ban} disabled={!reason || banningUser || banningVideo}>{(banningUser || banningVideo) && <Loader2 className="animate-spin"/>}{t("admin.ban")}</Button></DialogFooter></DialogContent></Dialog></>;
}

function ReportsTab() {
    const {t} = useTranslation(); const showError = useApiErrorMessage(); const [reportType, setReportType] = useState<ReportType>("Video"); const [page, setPage] = useState(1); const [resolved, setResolved] = useState<Set<string>>(new Set()); const [markAsResolved, {isLoading: isResolving}] = useMarkReportAsResolvedMutation();
    const {data, isLoading, isError} = useGetAdminReportsQuery({reportType, pageNumber: page, pageSize: PAGE_SIZE}, {refetchOnFocus: true, refetchOnMountOrArgChange: true}); const reports = data?.data?.items ?? []; const metadata = data?.data?.metadata;
    const resolve = async (id: string) => { try { await markAsResolved(id).unwrap(); setResolved((previous) => new Set(previous).add(id)); toast.success(t("admin.reportResolved")); } catch (error) { showError(error, t("admin.resolveError")); } };
    return <div className="flex h-full min-h-0 flex-col"><div className="border-b border-border px-3 sm:px-6 py-3"><div className="flex flex-wrap gap-1">{REPORT_TYPES.map((type) => <Button key={type} variant={reportType === type ? "secondary" : "ghost"} size="sm" onClick={() => { setReportType(type); setPage(1); setResolved(new Set()); }}>{t(`admin.types.${type.toLowerCase()}`)}</Button>)}</div></div><div className="min-h-0 flex-1 overflow-y-auto">{isLoading ? <div className="flex justify-center py-8"><Loader2 className="animate-spin text-muted-foreground"/></div> : isError ? <p className="px-3 sm:px-6 py-4 text-sm text-destructive">{t("admin.loadError")}</p> : reports.length === 0 ? <p className="px-3 sm:px-6 py-8 text-center text-sm text-muted-foreground">{t("admin.empty")}</p> : reports.map((report) => { const content = report.reportedContent; const thumbnail = getAvatarUrl(content?.thumbnail ?? null); const isResolved = resolved.has(report.id); const date = report.createdAt ? new Date(report.createdAt) : null; const dateLabel = date && !Number.isNaN(date.getTime()) ? date.toLocaleString() : t("admin.unknownDate"); return <div key={report.id} className={cn("flex items-center justify-between gap-3 border-b border-border px-3 sm:px-6 py-3 last:border-b-0", isResolved && "opacity-50")}><div className="min-w-0"><p className="text-sm font-medium">{t("admin.reportedBy")}: @{getUsername(report.reportedBy?.username)}</p>{content?.title && <p className="truncate text-xs text-muted-foreground">{content.title}</p>}{thumbnail && <img src={getMediaUrl(thumbnail)} alt="" loading="lazy" className="mt-1 h-10 w-10 rounded-md object-cover" onError={(e) => e.currentTarget.remove()}/>}<p className="text-xs text-muted-foreground">{t("admin.reason")}: {report.reason?.trim() || t("admin.unknownReason")} · {dateLabel}</p></div><div className="flex shrink-0 items-center gap-2">{!isResolved && reportType !== "Comment" && <BanReportAction report={report} reportType={reportType} onBanned={() => void resolve(report.id)}/>}<Button variant="outline" size="sm" disabled={isResolved || isResolving} onClick={() => void resolve(report.id)}>{isResolving && <Loader2 className="animate-spin"/>}{isResolved ? t("admin.processed") : t("admin.markProcessed")}</Button></div></div>; })}</div><PaginationControls hasNext={metadata?.hasNext ?? false} hasPrevious={metadata?.hasPrevious ?? false} currentPage={page} totalPages={metadata?.totalPages} onPageChange={setPage}/></div>;
}

function AdminPage() {
    const {t} = useTranslation();
    return <Card className="flex h-full min-h-0 flex-col"><CardHeader className="border-b border-border"><CardTitle>{t("admin.title")}</CardTitle></CardHeader><CardContent className="flex min-h-0 flex-1 flex-col p-0"><Tabs defaultValue="reports" className="flex min-h-0 flex-1 flex-col"><TabsList className="mx-3 sm:mx-6 mt-3 self-start"><TabsTrigger value="users">{t("admin.users")}</TabsTrigger><TabsTrigger value="videos">{t("admin.videos")}</TabsTrigger><TabsTrigger value="reports">{t("admin.reports")}</TabsTrigger></TabsList><TabsContent value="users" className="min-h-0 flex-1 overflow-hidden"><UsersTab/></TabsContent><TabsContent value="videos" className="min-h-0 flex-1 overflow-hidden"><VideosTab/></TabsContent><TabsContent value="reports" className="min-h-0 flex-1 overflow-hidden"><ReportsTab/></TabsContent></Tabs></CardContent></Card>;
}

export default AdminPage;
