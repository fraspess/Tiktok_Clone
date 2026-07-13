import {useEffect, useRef, useState, type RefObject, useMemo, type MouseEvent} from "react";
import Hls from "hls.js";
import {Pause, Play, Volume2, VolumeX} from "lucide-react";
import {useIntersectionObserver} from "@/hooks/useIntersectionObserver.ts";
import VideoActionsSidebar from "@/components/feed/VideoActionsSidebar.tsx";
import type {VideoDto} from "@/types/Video.ts";

interface VideoCardProps {
    video: VideoDto;
    containerRef: RefObject<HTMLDivElement | null>;
}

const VideoCard = ({video, containerRef}: VideoCardProps) => {
    const sectionRef = useRef<HTMLDivElement>(null);
    const videoRef = useRef<HTMLVideoElement>(null);
    const progressBarRef = useRef<HTMLDivElement>(null);
    const [isMuted, setIsMuted] = useState(true);
    const [isPlaying, setIsPlaying] = useState(true);
    const [progress, setProgress] = useState(0);
    const observerOptions = useMemo(
        () => ({ root: containerRef, threshold: 0.6 }),
        [containerRef]
    );
    const isVisible = useIntersectionObserver(sectionRef, observerOptions);


    const retryCountRef = useRef(0);

    useEffect(() => {
        const videoEl = videoRef.current;
        if (!videoEl) return;
        retryCountRef.current = 0;

        let hls: Hls | null = null;

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

            hls.loadSource(video.videoUrl);
            hls.attachMedia(videoEl);
        } else {
            videoEl.src = video.videoUrl;
        }

        videoEl.play().catch((err) => {
            console.warn("Video autoplay blocked or interrupted:", err);
        });

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

        const handleTimeUpdate = () => {
            if (videoEl.duration) {
                setProgress((videoEl.currentTime / videoEl.duration) * 100);
            }
        };

        videoEl.addEventListener("timeupdate", handleTimeUpdate);
        return () => videoEl.removeEventListener("timeupdate", handleTimeUpdate);
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
            className="relative flex h-full w-full snap-start snap-always items-center justify-center gap-3 bg-neutral-100 px-4 dark:bg-neutral-950"
        >
            <div className="relative h-[85%] max-h-[760px] aspect-[9/16] overflow-hidden rounded-2xl bg-black shadow-2xl">
                <video
                    ref={videoRef}
                    poster={video.thumbnailUrl || undefined}
                    className="h-full w-full object-cover"
                    loop
                    muted={isMuted}
                    playsInline
                    preload="metadata"
                    onClick={togglePlayPause}
                />

                <button
                    type="button"
                    onClick={togglePlayPause}
                    className="absolute left-3 top-3 rounded-full bg-black/40 p-2 text-white backdrop-blur-sm"
                >
                    {isPlaying ? <Pause size={18}/> : <Play size={18}/>}
                </button>

                <button
                    type="button"
                    onClick={() => setIsMuted((prev) => !prev)}
                    className="absolute right-3 top-3 rounded-full bg-black/40 p-2 text-white backdrop-blur-sm"
                >
                    {isMuted ? <VolumeX size={18}/> : <Volume2 size={18}/>}
                </button>

                <div className="absolute bottom-4 left-4 right-4 text-white">
                    <p className="font-semibold">@{video.author?.username ?? "unknown"}</p>
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
                            className="h-full bg-white transition-[width] duration-150 ease-linear"
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