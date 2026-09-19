import {useEffect, useRef, useState} from "react";
import {Link} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Button} from "@/components/ui/button.tsx";
import {useGetMyVideosQuery} from "@/store/apis/videoApi.ts";
import {useAppSelector} from "@/store/hooks.ts";
import {formatCount} from "@/lib/utils.ts";

const PAGE_SIZE = 20;

// однакові колонки для шапки й рядків: відео | дата | перегляди | лайки | коментарі | прогрес
const ROW =
    "grid grid-cols-[minmax(0,1fr)_170px] md:grid-cols-[minmax(0,1fr)_110px_90px_80px_100px_170px] items-center gap-4 px-4";

// картинка з фолбеком на сірий плейсхолдер, якщо src нема або він биту
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

const MyVideosPage = () => {
    const {t} = useTranslation();
    const [pageNumber, setPageNumber] = useState(1);

    const {data, isLoading, isError, refetch} = useGetMyVideosQuery(
        {pageNumber, pageSize: PAGE_SIZE},
        {refetchOnMountOrArgChange: true}
    );

    // відео, що завантажуються або обробляються прямо зараз
    const pending = useAppSelector((s) => s.uploads.items);
    const finishedCount = useAppSelector((s) => s.uploads.finishedCount);
    const initialFinished = useRef(finishedCount);

    // коли обробка завершилась, підтягуємо свіжий список
    useEffect(() => {
        if (finishedCount !== initialFinished.current) {
            initialFinished.current = finishedCount;
            refetch();
        }
    }, [finishedCount, refetch]);

    // TODO: якщо PagedResult має інше поле зі списком (не items), виправ тут
    const videos = data?.data.items ?? [];
    const hasNext = videos.length === PAGE_SIZE;

    return (
        <div className="mx-auto flex w-full max-w-6xl flex-col gap-4 px-6 pt-20 pb-10">
            <div className="overflow-hidden rounded-lg border border-neutral-200 dark:border-neutral-800">
                <div className={`${ROW} border-b border-neutral-200 py-3 text-xs text-muted-foreground dark:border-neutral-800`}>
                    <span>{t("studio.video", "Відео")}</span>
                    <span className="hidden md:block">{t("studio.date", "Дата")}</span>
                    <span className="hidden text-right md:block">{t("studio.views", "Перегляди")}</span>
                    <span className="hidden text-right md:block">{t("studio.likes", "Лайки")}</span>
                    <span className="hidden text-right md:block">{t("studio.comments", "Коментарі")}</span>
                    <span/>
                </div>

                {/* відео, що завантажуються зараз */}
                {pageNumber === 1 && pending.map((u) => {
                    const isUploading = u.status === "uploading";
                    // єдиний плавний прогрес: 0-50% завантаження, 50-100% обробка
                    const percent = isUploading
                        ? Math.round(u.progress / 2)
                        : 50 + Math.round(u.processingProgress / 2);
                    return (
                        <div key={u.id} className={`${ROW} border-b border-neutral-200 py-3 dark:border-neutral-800`}>
                            <div className="flex min-w-0 items-center gap-3">
                                {/* сірий плейсхолдер, поки відео повністю не оброблено */}
                                <div className="h-16 w-28 shrink-0 rounded-md bg-neutral-300 dark:bg-neutral-700"/>
                                <span className="text-sm text-muted-foreground">
                                    {u.status === "error"
                                        ? t("studio.error", "Помилка")
                                        : isUploading
                                            ? t("studio.uploading", "Завантаження…")
                                            : t("studio.processing", "Обробка…")}
                                </span>
                            </div>
                            <span className="hidden text-muted-foreground md:block">—</span>
                            <span className="hidden text-right text-muted-foreground md:block">—</span>
                            <span className="hidden text-right text-muted-foreground md:block">—</span>
                            <span className="hidden text-right text-muted-foreground md:block">—</span>
                            <div className="flex flex-col gap-1">
                                {u.status === "error" ? (
                                    <span className="text-xs text-red-500">{u.errorMessage ?? t("studio.error", "Помилка")}</span>
                                ) : (
                                    <>
                                        <span className="text-right text-xs text-muted-foreground">{percent}%</span>
                                        <div className="h-1.5 overflow-hidden rounded bg-neutral-200 dark:bg-neutral-800">
                                            <div
                                                className="h-full bg-foreground transition-[width]"
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
                            {/* TODO: підстав своє поле прев'ю (thumbnailUrl / preview / poster ...) */}
                            <Thumbnail src={video.thumbnailUrl}/>
                            <span className="line-clamp-2 break-words text-sm">
                                {video.description || t("studio.noDescription", "Без опису")}
                            </span>
                        </Link>
                        <span className="hidden text-sm text-muted-foreground md:block">
                            {/* TODO: підстав поле дати */}
                            {new Date(video.createdAt).toLocaleDateString()}
                        </span>
                        <span className="hidden text-right text-sm md:block">{formatCount(video.viewCount ?? 0)}</span>
                        <span className="hidden text-right text-sm md:block">{formatCount(video.likeCount)}</span>
                        <span className="hidden text-right text-sm md:block">{formatCount(video.commentsCount)}</span>
                        <span/>
                    </div>
                ))}
            </div>

            {(pageNumber > 1 || hasNext) && (
                <div className="flex items-center justify-center gap-3">
                    <Button variant="outline" disabled={pageNumber === 1} onClick={() => setPageNumber((p) => p - 1)}>
                        {t("studio.prev", "Назад")}
                    </Button>
                    <span className="text-sm text-muted-foreground">{pageNumber}</span>
                    <Button variant="outline" disabled={!hasNext} onClick={() => setPageNumber((p) => p + 1)}>
                        {t("studio.next", "Далі")}
                    </Button>
                </div>
            )}
        </div>
    );
};

export default MyVideosPage;