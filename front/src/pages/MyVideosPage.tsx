import {useEffect, useMemo, useRef, useState} from "react";
import {Link} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Button} from "@/components/ui/button.tsx";
import {useGetMyVideosQuery} from "@/store/apis/videoApi.ts";
import {useAppSelector} from "@/store/hooks.ts";
import {formatCount} from "@/lib/utils.ts";

const PAGE_SIZE = 20;

const ROW =
    "grid grid-cols-[minmax(0,1fr)_minmax(96px,1fr)] sm:grid-cols-[minmax(0,3fr)_1fr_1fr_1fr_1fr_1.4fr] items-center gap-4 px-4";

const Thumbnail = ({src}: {src?: string | null}) => {
    const [broken, setBroken] = useState(false);

    if (!src || broken) {
        return <div className="h-16 w-28 shrink-0 rounded-md bg-neutral-300 dark:bg-neutral-700"/>;
    }

    return (
        <img
            src={src}
            alt=""
            onError={() => setBroken(true)}
            className="h-16 w-28 shrink-0 rounded-md bg-neutral-300 object-cover dark:bg-neutral-700"
        />
    );
};

const buildPageList = (current: number, total: number): (number | "...")[] => {
    const delta = 1;
    const pages: (number | "...")[] = [];
    const start = Math.max(2, current - delta);
    const end = Math.min(total - 1, current + delta);

    pages.push(1);
    if (start > 2) pages.push("...");
    for (let p = start; p <= end; p++) pages.push(p);
    if (end < total - 1) pages.push("...");
    if (total > 1) pages.push(total);

    return pages;
};

const MyVideosPage = () => {
    const {t} = useTranslation();
    const [pageNumber, setPageNumber] = useState(1);

    const {data, isLoading, isError, refetch} = useGetMyVideosQuery(
        {pageNumber, pageSize: PAGE_SIZE},
        {refetchOnMountOrArgChange: true}
    );

    const pending = useAppSelector((s) => s.uploads.items);
    const finishedCount = useAppSelector((s) => s.uploads.finishedCount);
    const initialFinished = useRef(finishedCount);

    useEffect(() => {
        if (finishedCount !== initialFinished.current) {
            initialFinished.current = finishedCount;
            refetch();
        }
    }, [finishedCount, refetch]);

    const pendingVideoIds = useMemo(
        () => new Set(pending.map((u) => u.videoId).filter((id): id is string => Boolean(id))),
        [pending]
    );

    const videos = useMemo(
        () => (data?.data.items ?? []).filter((video) => !pendingVideoIds.has(video.id)),
        [data, pendingVideoIds]
    );
    const totalPages = data?.data.totalPages as number | undefined;
    const hasNext = totalPages ? pageNumber < totalPages : videos.length === PAGE_SIZE;

    const pageList = useMemo(() => {
        if (!totalPages || totalPages <= 1) return null;
        return buildPageList(pageNumber, totalPages);
    }, [pageNumber, totalPages]);

    return (
        <div className="mx-auto flex w-full max-w-6xl flex-col gap-4 px-3 pt-4 pb-6 md:px-6 md:pt-20 md:pb-10">
            <div className="overflow-hidden rounded-lg border border-neutral-200 dark:border-neutral-800">
                <div className={`${ROW} border-b border-neutral-200 py-3 text-xs text-muted-foreground dark:border-neutral-800`}>
                    <span>{t("studio.video", "Відео")}</span>
                    <span className="hidden sm:block">{t("studio.date", "Дата")}</span>
                    <span className="hidden text-right sm:block">{t("studio.views", "Перегляди")}</span>
                    <span className="hidden text-right sm:block">{t("studio.likes", "Лайки")}</span>
                    <span className="hidden text-right sm:block">{t("studio.comments", "Коментарі")}</span>
                    <span/>
                </div>

                {/* відео, що завантажуються зараз */}
                {pageNumber === 1 && pending.map((u) => {
                    const isUploading = u.status === "uploading";
                    const percent = isUploading ? u.progress : u.processingProgress;
                    return (
                        <div key={u.id} className={`${ROW} border-b border-neutral-200 py-3 dark:border-neutral-800`}>
                            <div className="flex min-w-0 items-center gap-3">
                                <div className="h-16 w-28 shrink-0 rounded-md bg-neutral-300 dark:bg-neutral-700"/>
                                <span className="text-sm text-muted-foreground">
                                    {u.status === "error"
                                        ? t("studio.error", "Помилка")
                                        : isUploading
                                            ? t("studio.uploading", "Завантаження…")
                                            : t("studio.processing", "Обробка…")}
                                </span>
                            </div>
                            <span className="hidden text-muted-foreground sm:block">—</span>
                            <span className="hidden text-right text-muted-foreground sm:block">—</span>
                            <span className="hidden text-right text-muted-foreground sm:block">—</span>
                            <span className="hidden text-right text-muted-foreground sm:block">—</span>
                            <div className="flex w-full flex-col items-end gap-1 self-center -mt-5">
                                {u.status === "error" ? (
                                    <span className="text-xs text-red-500">{u.errorMessage ?? t("studio.error", "Помилка")}</span>
                                ) : (
                                    <>
                                        <span className="text-xs tabular-nums text-muted-foreground">{percent}%</span>
                                        <div className="h-1.5 w-full overflow-hidden rounded bg-neutral-200 dark:bg-neutral-800">
                                            <div
                                                className={`h-full bg-foreground transition-[width] ease-linear ${
                                                    isUploading ? "duration-150" : "duration-500"
                                                }`}
                                                style={{width: `${percent}%`}}
                                            />
                                        </div>
                                    </>
                                )}
                            </div>
                        </div>
                    );
                })}

                {isLoading && (
                    <p className="px-4 py-8 text-center text-muted-foreground">{t("studio.loading", "Завантаження…")}</p>
                )}
                {isError && (
                    <p className="px-4 py-8 text-center text-red-500">{t("studio.loadError", "Не вдалося завантажити відео")}</p>
                )}
                {!isLoading && !isError && videos.length === 0 && pending.length === 0 && (
                    <p className="px-4 py-8 text-center text-muted-foreground">{t("studio.empty", "У вас ще немає відео")}</p>
                )}

                {videos.map((video) => (
                    <div
                        key={video.id}
                        className={`${ROW} border-b border-neutral-200 py-3 last:border-b-0 hover:bg-neutral-50 dark:border-neutral-800 dark:hover:bg-neutral-900/60`}
                    >
                        <Link to={`/video/${video.id}`} className="flex min-w-0 items-center gap-3">
                            <Thumbnail src={video.thumbnailUrl}/>
                            <span className="line-clamp-2 break-words text-sm">
                                {video.description || t("studio.noDescription", "Без опису")}
                            </span>
                        </Link>
                        <span className="hidden text-sm text-muted-foreground sm:block">
                            {new Date(video.createdAt).toLocaleDateString()}
                        </span>
                        <span className="hidden text-right text-sm sm:block">{formatCount(video.viewCount ?? 0)}</span>
                        <span className="hidden text-right text-sm sm:block">{formatCount(video.likeCount)}</span>
                        <span className="hidden text-right text-sm sm:block">{formatCount(video.commentsCount)}</span>
                        <span/>
                    </div>
                ))}
            </div>

            {(pageNumber > 1 || hasNext) && (
                <div className="flex items-center justify-center gap-2">
                    <Button
                        variant="outline"
                        disabled={pageNumber === 1}
                        onClick={() => setPageNumber((p) => p - 1)}
                    >
                        {t("studio.prev", "Назад")}
                    </Button>

                    {pageList ? (
                        <div className="flex items-center gap-1">
                            {pageList.map((p, idx) =>
                                p === "..." ? (
                                    <span key={`dots-${idx}`} className="px-2 text-sm text-muted-foreground">
                                        …
                                    </span>
                                ) : (
                                    <Button
                                        key={p}
                                        variant={p === pageNumber ? "default" : "outline"}
                                        size="icon"
                                        className="h-9 w-9"
                                        onClick={() => setPageNumber(p)}
                                    >
                                        {p}
                                    </Button>
                                )
                            )}
                        </div>
                    ) : (
                        <span className="text-sm text-muted-foreground">{pageNumber}</span>
                    )}

                    <Button
                        variant="outline"
                        disabled={!hasNext}
                        onClick={() => setPageNumber((p) => p + 1)}
                    >
                        {t("studio.next", "Далі")}
                    </Button>
                </div>
            )}
        </div>
    );
};

export default MyVideosPage;