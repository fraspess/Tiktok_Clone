import {type MouseEvent, type RefObject, useEffect, useMemo, useRef, useState} from "react";
import Hls from "hls.js";
import {Link} from "react-router-dom";
import {Pause, Play, Volume2, VolumeX} from "lucide-react";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import VideoActionsSidebar from "@/components/feed/VideoActionsSidebar.tsx";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {setMuted} from "@/store/slices/playerSlice.ts";
import type {VideoDto} from "@/types/Video.ts";

interface VideoCardProps {
    video: VideoDto;
    containerRef: RefObject<HTMLDivElement | null>;
}

const VideoCard = ({video, containerRef}: VideoCardProps) => {
    const sectionRef = useRef<HTMLDivElement>(null);
    const videoRef = useRef<HTMLVideoElement>(null);
    const progressBarRef = useRef<HTMLDivElement>(null);
    const dispatch = useAppDispatch();
    const isMuted = useAppSelector((state) => state.player.isMuted);
    const [isPlaying, setIsPlaying] = useState(true);
    const [progress, setProgress] = useState(0);
    const observerOptions = useMemo(
        () => ({root: containerRef, threshold: 0.6}),
        [containerRef]
    );
    const isVisible = useIntersectionObserver(sectionRef, observerOptions);


    const retryCountRef = useRef(0);

    useEffect(() => {
        const videoEl = videoRef.current;
        if (!videoEl) return;
        retryCountRef.current = 0;

        let hls: Hls | null = null;

        const attemptPlay = () => {
            videoEl.play().catch(() => {
                // Browser blocked autoplay with the current sound setting — the only thing
                // it always allows is muted autoplay, so fall back to that instead of
                // staying paused. Reverting the shared state (not a local one) keeps every
                // video in the feed consistent with what the browser actually allows.
                if (!videoEl.muted) {
                    videoEl.muted = true;
                    dispatch(setMuted(true));
                }
                videoEl.play().catch((err) => {
                    console.warn("Video autoplay blocked or interrupted:", err);
                });
            });
        };

        if (!isVisible) {
            videoEl.pause();
            videoEl.removeAttribute("src");
            videoEl.load();
            return;
        }

        const isHlsSource = video.videoUrl.includes(".m3u8");

        if (isHlsSource && Hls.isSupported()) {
            hls = new Hls();

            hls.on(Hls.Events.ERROR, (_event, data) => {
                if (!data.fatal) return;

                const canRetry = retryCountRef.current < 3;
                retryCountRef.current += 1;

                switch (data.type) {
                    case Hls.ErrorTypes.NETWORK_ERROR:
                        if (canRetry) hls?.startLoad();
                        else hls?.destroy();
                        break;
                    case Hls.ErrorTypes.MEDIA_ERROR:
                        if (canRetry) hls?.recoverMediaError();
                        else hls?.destroy();
                        break;
                    default:
                        hls?.destroy();
                        break;
                }
            });

            // Autoplay must wait until HLS.js has actually parsed the manifest — calling
            // play() right after attachMedia() races the async load and silently fails.
            hls.on(Hls.Events.MANIFEST_PARSED, () => {
                attemptPlay();
            });

            hls.loadSource(video.videoUrl);
            hls.attachMedia(videoEl);
        } else {
            videoEl.src = video.videoUrl;
            attemptPlay();
        }

        return () => {
            if (hls) hls.destroy();
        };
    }, [video.videoUrl, isVisible]);

    useEffect(() => {
        setProgress(0);
    }, [video.id]);

    useEffect(() => {
        const videoEl = videoRef.current;
        if (!videoEl) return;

        let rafId: number;

        const updateProgress = () => {
            if (videoEl.duration) {
                setProgress((videoEl.currentTime / videoEl.duration) * 100);
            }
            rafId = requestAnimationFrame(updateProgress);
        };

        rafId = requestAnimationFrame(updateProgress);

        return () => cancelAnimationFrame(rafId);
    }, []);

    useEffect(() => {
        const videoEl = videoRef.current;
        if (!videoEl) return;

        const handlePlay = () => setIsPlaying(true);
        const handlePause = () => setIsPlaying(false);

        videoEl.addEventListener("play", handlePlay);
        videoEl.addEventListener("pause", handlePause);
        return () => {
            videoEl.removeEventListener("play", handlePlay);
            videoEl.removeEventListener("pause", handlePause);
        };
    }, []);

    const togglePlayPause = () => {
        const videoEl = videoRef.current;
        if (!videoEl) return;

        if (videoEl.paused) {
            videoEl.play().catch((err) => {
                console.warn("Video play blocked or interrupted:", err);
            });
        } else {
            videoEl.pause();
        }
    };

    const handleSeek = (e: MouseEvent<HTMLDivElement>) => {
        const videoEl = videoRef.current;
        const bar = progressBarRef.current;
        if (!videoEl || !bar || !videoEl.duration) return;

        const rect = bar.getBoundingClientRect();
        const ratio = Math.min(Math.max((e.clientX - rect.left) / rect.width, 0), 1);
        videoEl.currentTime = ratio * videoEl.duration;
        setProgress(ratio * 100);
    };


    return (
        <section
            ref={sectionRef}
            id={video.id}
            className="relative flex h-full w-full snap-start snap-always items-center justify-center gap-3 bg-neutral-100 px-4 dark:bg-neutral-950"
        >
            <div
                className="relative aspect-[9/16] h-full max-h-[85vh] max-w-full overflow-hidden rounded-2xl bg-black shadow-2xl">
                <video
                    ref={videoRef}
                    poster={video.thumbnailUrl || undefined}
                    className="h-full w-full object-cover"
                    loop
                    muted={isMuted}
                    autoPlay
                    playsInline
                    preload="metadata"
                    onClick={togglePlayPause}
                />
                <button
                    type="button"
                    onClick={togglePlayPause}
                    className="absolute left-4 top-4 z-20 rounded-full bg-black/40 p-3 text-white backdrop-blur-md transition active:scale-90 hover:bg-black/60"
                >
                    {isPlaying ? <Pause size={26}/> : <Play size={26}/>}
                </button>

                <button
                    type="button"
                    onClick={() => {
                        const videoEl = videoRef.current;
                        const next = !isMuted;
                        if (videoEl) videoEl.muted = next;
                        dispatch(setMuted(next));
                    }}
                    className="absolute right-4 top-4 z-20 rounded-full bg-black/40 p-3 text-white backdrop-blur-md transition active:scale-90 hover:bg-black/60"
                >
                    {isMuted ? <VolumeX size={26}/> : <Volume2 size={26}/>}
                </button>

                <div className="absolute bottom-4 left-4 right-4 text-white">
                    {video.author?.username ? (
                        <Link
                            to={`/@${video.author.username}`}
                            onClick={(e) => e.stopPropagation()}
                            className="font-semibold hover:underline"
                        >
                            @{video.author.username}
                        </Link>
                    ) : (
                        <p className="font-semibold">@unknown</p>
                    )}
                    {video.description && (
                        <p className="mt-1 line-clamp-2 text-sm text-white/90">{video.description}</p>
                    )}
                </div>

                <div
                    ref={progressBarRef}
                    onClick={handleSeek}
                    className="absolute bottom-0 left-0 right-0 z-10 flex h-3 w-full cursor-pointer items-end px-0"
                >
                    <div className="h-1 w-full bg-white/30">
                        <div
                            className="h-full bg-white"
                            style={{width: `${progress}%`}}
                        />
                    </div>
                </div>
            </div>

            <VideoActionsSidebar video={video}/>
        </section>
    );
};

export default VideoCard;