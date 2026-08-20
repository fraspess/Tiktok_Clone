import {type ChangeEvent, useEffect, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {Hash, Pause, Play, Volume2, VolumeX} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import VideoDropzone from "@/components/videoDropZone/VideoDropZone.tsx";
import {useConfirmUploadMutation, useInitUploadMutation} from "@/store/apis/videoApi.ts";
import {useNavigate} from "react-router-dom";

const MAX_DESCRIPTION_LENGTH = 4000;

const UploadVideoPage = () => {
    const {t} = useTranslation();
    const videoRef = useRef<HTMLVideoElement>(null);

    const [file, setFile] = useState<File | null>(null);
    const [description, setDescription] = useState("");

    const [isPlaying, setIsPlaying] = useState(false);
    const [isMuted, setIsMuted] = useState(false);
    const [progress, setProgress] = useState(0);
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const [_, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);

    const [initUpload] = useInitUploadMutation();
    const [confirmUpload] = useConfirmUploadMutation();
    const navigate = useNavigate();
    const errorText = useRef<HTMLParagraphElement | null>(null);
    const onConfirm = async () => {
        try {
            const response = await initUpload({contentType: file!.type}).unwrap();
            console.log(response);
            await fetch(response.url, {
                method: "PUT",
                headers: {
                    "Content-Type": file!.type,
                },
                body: file
            })
            await confirmUpload({token: response.uploadToken, description: description.trim()}).unwrap();
            navigate("/");
        } catch (err) {
            if (errorText.current) {
                errorText.current.textContent = t("uploads.error");
            }
            console.error(err);
        }

    }
    const onFileSelected = (selected: File) => {
        setFile(selected);
        setDescription("");
    };

    const handleChange = (e: ChangeEvent<HTMLTextAreaElement>) => {
        if (e.target.value.length <= MAX_DESCRIPTION_LENGTH) {
            setDescription(e.target.value);
        }
    };

    const insertToken = (token: string) => {
        setDescription(
            description.length > 0 && !description.endsWith(" ")
                ? `${description} ${token}`
                : `${description}${token}`
        );
    };

    useEffect(() => {
        const video = videoRef.current;
        if (!video || !file) return;

        const url = URL.createObjectURL(file);
        video.src = url;
        video.load();
        setIsPlaying(false);

        return () => {
            URL.revokeObjectURL(url);
        };
    }, [file]);

    useEffect(() => {
        const video = videoRef.current;
        if (!video) return;

        const handleTimeUpdate = () => {
            setCurrentTime(video.currentTime);
            setProgress(video.duration ? (video.currentTime / video.duration) * 100 : 0);
        };
        const handleLoadedMetadata = () => setDuration(video.duration);
        const handleEnded = () => setIsPlaying(false);

        video.addEventListener("timeupdate", handleTimeUpdate);
        video.addEventListener("loadedmetadata", handleLoadedMetadata);
        video.addEventListener("ended", handleEnded);

        return () => {
            video.removeEventListener("timeupdate", handleTimeUpdate);
            video.removeEventListener("loadedmetadata", handleLoadedMetadata);
            video.removeEventListener("ended", handleEnded);
        };
    }, [file]);

    const togglePlay = () => {
        const video = videoRef.current;
        if (!video) return;
        if (video.paused) {
            video.play();
            setIsPlaying(true);
        } else {
            video.pause();
            setIsPlaying(false);
        }
    };

    const toggleMute = () => {
        const video = videoRef.current;
        if (!video) return;
        video.muted = !video.muted;
        setIsMuted(video.muted);
    };

    const handleSeek = (e: ChangeEvent<HTMLInputElement>) => {
        const video = videoRef.current;
        if (!video || !duration) return;
        const newProgress = Number(e.target.value);
        video.currentTime = (newProgress / 100) * duration;
        setProgress(newProgress);
    };

    return (
        <div className="flex flex-col gap-6">
            {!file && (
                <div className="flex flex-col h-64 items-center justify-center">
                    <div className="w-full max-w-2xl h-64">
                        <VideoDropzone onFileSelect={onFileSelected}/>
                    </div>
                </div>
            )}

            {file && (
                <div className="w-1/2 mx-auto flex flex-col gap-2">
                    <div
                        className="mt-5 flex flex-col gap-2 rounded-lg bg-neutral-100 dark:bg-neutral-900 p-3 w-1/3 mx-auto">
                        <div className="relative w-full aspect-[9/16] overflow-hidden rounded-md bg-black">
                            <video
                                ref={videoRef}
                                className="absolute inset-0 h-full w-full object-cover"
                                onClick={togglePlay}
                            />
                        </div>

                        <div className="flex items-center gap-3">
                            <button
                                type="button"
                                onClick={togglePlay}
                                className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-neutral-200 dark:bg-neutral-800 hover:bg-neutral-300 dark:hover:bg-neutral-700 transition-colors"
                            >
                                {isPlaying ? <Pause className="h-4 w-4"/> : <Play className="h-4 w-4 ml-0.5"/>}
                            </button>

                            <input
                                type="range"
                                min={0}
                                max={100}
                                step={0.1}
                                value={progress}
                                onChange={handleSeek}
                                className="flex-1 h-1 accent-foreground cursor-pointer"
                            />

                            <button
                                type="button"
                                onClick={toggleMute}
                                className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full hover:bg-neutral-200 dark:hover:bg-neutral-800 transition-colors"
                            >
                                {isMuted ? <VolumeX className="h-4 w-4"/> : <Volume2 className="h-4 w-4"/>}
                            </button>
                        </div>
                    </div>

                    <label htmlFor="description" className="text-sm font-medium">
                        {t("details.description")}
                    </label>
                    <div className="flex flex-col rounded-lg bg-neutral-100 dark:bg-neutral-900 overflow-hidden">
                        <textarea
                            id="description"
                            value={description}
                            onChange={handleChange}
                            rows={5}
                            placeholder={t("details.descriptionPlaceholder")}
                            className="w-full resize-none bg-transparent px-4 pt-4 pb-2 text-sm outline-none placeholder:text-muted-foreground"
                        />
                        <div
                            className="flex items-center justify-between px-4 py-2 border-t border-neutral-200 dark:border-neutral-800">
                            <div className="flex items-center gap-4">
                                <button
                                    type="button"
                                    onClick={() => insertToken("#")}
                                    className="flex items-center gap-1 text-xs font-medium text-muted-foreground hover:text-foreground transition-colors"
                                >
                                    <Hash className="h-3.5 w-3.5"/>
                                    {t("details.hashtags")}
                                </button>
                            </div>
                            <span className="text-xs text-muted-foreground">
                                {description.length}/{MAX_DESCRIPTION_LENGTH}
                            </span>
                        </div>
                        <p ref={errorText} className="text-red-500 text-center"/>
                    </div>

                    <div className="flex justify-end mt-3">
                        <Button onClick={onConfirm} className="px-10" variant="default"> {t("uploads.upload")} </Button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default UploadVideoPage;