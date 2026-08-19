import {useEffect, useMemo, useRef} from "react";
import {useLocation, useNavigate, useParams} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {ArrowLeft} from "lucide-react";
import VideoCard from "@/components/feed/VideoCard.tsx";
import {useInfiniteUserVideos} from "@/hooks/useInfiniteUserVideos.ts";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import {useGetUserProfileQuery} from "@/store/apis/userApi.ts";
import type {VideoDto} from "@/types/Video.ts";

interface LocationState {
    videos?: VideoDto[];
    userId?: string;
    nextPage?: number;
    hasNext?: boolean;
}

const ProfileVideoFeedPage = () => {
    const {username: rawUsername, videoId} = useParams<{ username: string; videoId: string }>();
    const username = rawUsername?.startsWith("@") ? rawUsername.slice(1) : rawUsername;
    const location = useLocation();
    const navigate = useNavigate();
    const {t} = useTranslation();

    const state = (location.state ?? {}) as LocationState;


    const {data: profileData} = useGetUserProfileQuery(username ?? "", {
        skip: !username || Boolean(state.userId),
    });
    const userId = state.userId ?? profileData?.data.id;

    const containerRef = useRef<HTMLDivElement>(null);
    const sentinelRef = useRef<HTMLDivElement>(null);
    const hasScrolledRef = useRef(false);

    const {videos, loadMore, hasNext, isFetching, error} = useInfiniteUserVideos(userId, 12, {
        seedVideos: state.videos,
        seedNextPage: state.nextPage,
        seedHasNext: state.hasNext,
    });

    useEffect(() => {
        if (userId && videos.length === 0) {
            loadMore();
        }

    }, [userId]);


    useEffect(() => {
        if (hasScrolledRef.current || !videoId) return;
        const node = document.getElementById(videoId);
        if (node) {
            node.scrollIntoView({block: "start"});
            hasScrolledRef.current = true;
        }
    }, [videos, videoId]);

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

    const handleBack = () => {
        navigate(username ? `/@${username}` : "/");
    };

    const backButton = (
        <button
            type="button"
            onClick={handleBack}
            className="absolute left-4 top-4 z-10 flex items-center gap-1.5 rounded-full bg-black/50 px-3 py-1.5 text-sm font-medium text-white backdrop-blur hover:bg-black/70"
        >
            <ArrowLeft size={16}/>
            {t("profile.backToProfile")}
        </button>
    );

    if (videos.length === 0 && (isFetching || (!userId && username))) {
        return (
            <div className="relative flex h-full w-full items-center justify-center text-muted-foreground">
                {backButton}
                {t("feed.loading")}
            </div>
        );
    }

    if (videos.length === 0 && error) {
        return (
            <div className="relative flex h-full w-full items-center justify-center text-muted-foreground">
                {backButton}
                {error}
            </div>
        );
    }

    if (videos.length === 0) {
        return (
            <div className="relative flex h-full w-full items-center justify-center text-muted-foreground">
                {backButton}
                {t("profile.emptyVideos")}
            </div>
        );
    }

    return (
        <div className="relative h-full w-full">
            {backButton}
            <div
                ref={containerRef}
                className="h-full w-full snap-y snap-mandatory overflow-y-scroll scroll-smooth [-ms-overflow-style:none] [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
            >
                {videos.map((video) => (
                    <VideoCard key={video.id} video={video} containerRef={containerRef}/>
                ))}
                {hasNext && <div ref={sentinelRef} className="h-1 w-full"/>}
            </div>
        </div>
    );
};

export default ProfileVideoFeedPage;
