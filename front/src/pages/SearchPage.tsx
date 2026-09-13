import {useEffect, useMemo, useRef, useState} from "react";
import {useNavigate} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Loader2, Play, Search as SearchIcon} from "lucide-react";
import {useDebounce} from "@/hooks/useDebounce.ts";
import {useInfiniteSearch} from "@/hooks/useInfiniteSearch.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import {Input} from "@/components/ui/input.tsx";
import {formatCount} from "@/lib/utils.ts";
import type {VideoAuthor} from "@/types/Video.ts";

const SearchPage = () => {
    const {t} = useTranslation();
    const navigate = useNavigate();
    const [inputValue, setInputValue] = useState("");
    const debouncedQuery = useDebounce(inputValue, 1000);

    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);

    const {videos, loadMore, hasNext, isFetching, error} = useInfiniteSearch(debouncedQuery, 12);

    useEffect(() => {
        if (debouncedQuery.trim()) {
            loadMore();
        }
    }, [debouncedQuery]);

    const sentinelOptions = useMemo(
        () => ({root: containerRef, rootMargin: "600px"}),
        [containerRef]
    );
    const isSentinelVisible = useIntersectionObserver(sentinelRef, sentinelOptions);

    useEffect(() => {
        if (isSentinelVisible && hasNext && !isFetching) {
            loadMore();
        }
    }, [isSentinelVisible, hasNext, isFetching, loadMore]);

    const lowerQuery = debouncedQuery.trim().toLowerCase();

    const matchedAuthors = useMemo(() => {
        if (!lowerQuery) return [];
        const map = new Map<string, VideoAuthor>();
        videos.forEach((video) => {
            if (video.author && video.author.username.toLowerCase().includes(lowerQuery)) {
                map.set(video.author.id, video.author);
            }
        });
        return Array.from(map.values());
    }, [videos, lowerQuery]);

    const matchedHashtags = useMemo(() => {
        if (!lowerQuery) return [];
        const set = new Set<string>();
        videos.forEach((video) => {
            video.hashTags.forEach((tag) => {
                if (tag.toLowerCase().includes(lowerQuery)) {
                    set.add(tag);
                }
            });
        });
        return Array.from(set);
    }, [videos, lowerQuery]);

    const showEmptyState = !lowerQuery;
    const showNoResults =
        lowerQuery && !isFetching && videos.length === 0 && !error;

    return (
        <div className="flex h-full w-full flex-col overflow-hidden">
            <div className="border-b px-4 py-3">
                <div className="relative mx-auto max-w-xl">
                    <SearchIcon
                        size={16}
                        className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
                    />
                    <Input
                        value={inputValue}
                        onChange={(e) => setInputValue(e.target.value)}
                        placeholder={t("search.placeholder")}
                        className="pl-9"
                        autoFocus
                    />
                </div>
            </div>

            <div ref={containerRef} className="min-h-0 flex-1 overflow-y-auto px-4 py-4">
                {showEmptyState && (
                    <div className="flex h-full items-center justify-center text-muted-foreground">
                        {t("search.typeToSearch")}
                    </div>
                )}

                {!showEmptyState && (
                    <>
                        {matchedAuthors.length > 0 && (
                            <div className="mb-6">
                                <h3 className="mb-2 text-sm font-semibold text-muted-foreground">
                                    {t("search.users")}
                                </h3>
                                <div className="flex flex-col gap-2">
                                    {matchedAuthors.map((author) => (
                                        <button
                                            key={author.id}
                                            type="button"
                                            onClick={() => navigate(`/@${author.username}`)}
                                            className="flex items-center gap-3 rounded-lg p-2 text-left hover:bg-neutral-100 dark:hover:bg-neutral-900"
                                        >
                                            {author.avatar ? (
                                                <img
                                                    src={author.avatar.small}
                                                    alt={author.username}
                                                    className="h-10 w-10 rounded-full object-cover"
                                                />
                                            ) : (
                                                <div className="h-10 w-10 rounded-full bg-neutral-700"/>
                                            )}
                                            <span className="font-medium">@{author.username}</span>
                                        </button>
                                    ))}
                                </div>
                            </div>
                        )}

                        {matchedHashtags.length > 0 && (
                            <div className="mb-6">
                                <h3 className="mb-2 text-sm font-semibold text-muted-foreground">
                                    {t("search.hashtags")}
                                </h3>
                                <div className="flex flex-wrap gap-2">
                                    {matchedHashtags.map((tag) => (
                                        <button
                                            key={tag}
                                            type="button"
                                            onClick={() => setInputValue(tag)}
                                            className="rounded-full border px-3 py-1 text-sm hover:bg-neutral-100 dark:hover:bg-neutral-900"
                                        >
                                            #{tag}
                                        </button>
                                    ))}
                                </div>
                            </div>
                        )}

                        {videos.length > 0 && (
                            <div>
                                <h3 className="mb-2 text-sm font-semibold text-muted-foreground">
                                    {t("search.videos")}
                                </h3>
                                <div className="grid grid-cols-2 gap-2 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6 xl:grid-cols-7 2xl:grid-cols-8">
                                    {videos.map((video) => (
                                        <div
                                            key={video.id}
                                            role="button"
                                            tabIndex={0}
                                            onClick={() =>
                                                video.author &&
                                                navigate(`/@${video.author.username}/video/${video.id}`, {
                                                    state: {userId: video.author.id},
                                                })
                                            }
                                            onKeyDown={(e) => {
                                                if ((e.key === "Enter" || e.key === " ") && video.author) {
                                                    e.preventDefault();
                                                    navigate(`/@${video.author.username}/video/${video.id}`, {
                                                        state: {userId: video.author.id},
                                                    });
                                                }
                                            }}
                                            className="group relative aspect-[3/4] cursor-pointer overflow-hidden rounded-md bg-neutral-800"
                                        >
                                            {video.thumbnailUrl ? (
                                                <img
                                                    src={video.thumbnailUrl}
                                                    alt={video.description || video.id}
                                                    className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
                                                />
                                            ) : (
                                                <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                                                    <Play size={28}/>
                                                </div>
                                            )}
                                            <div className="absolute inset-x-0 bottom-0 flex items-center gap-1 bg-gradient-to-t from-black/70 to-transparent px-2 py-1.5 text-xs font-medium text-white">
                                                <Play size={12} className="fill-white"/>
                                                {formatCount(video.viewCount)}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                                {hasNext && <div ref={sentinelRef} className="h-1 w-full"/>}
                            </div>
                        )}

                        {isFetching && videos.length === 0 && (
                            <div className="flex h-40 items-center justify-center">
                                <Loader2 className="h-6 w-6 animate-spin text-muted-foreground"/>
                            </div>
                        )}

                        {showNoResults && (
                            <div className="flex h-40 items-center justify-center text-muted-foreground">
                                {t("search.noResults")}
                            </div>
                        )}

                        {error && videos.length === 0 && (
                            <div className="flex h-40 items-center justify-center text-muted-foreground">
                                {error}
                            </div>
                        )}
                    </>
                )}
            </div>
        </div>
    );
};

export default SearchPage;