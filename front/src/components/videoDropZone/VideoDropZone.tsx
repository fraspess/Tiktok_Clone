import {useTranslation} from "react-i18next";
import {type ChangeEvent, type DragEvent, useRef, useState} from "react";
import {FolderOpen, MonitorPlay, RectangleHorizontal, UploadCloud} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {cn} from "@/lib/utils.ts";

interface VideoDropzoneProps {
    onFileSelect: (file: File) => void;
    file?: File | null;
    accept?: string;
    maxSizeMb?: number;
    disabled?: boolean;
    className?: string;
    onError?: (message: string) => void;
}

const infoItems = [
    {icon: FolderOpen, titleKey: "uploads.formatTitle", descKey: "uploads.formatDesc"},
    {icon: MonitorPlay, titleKey: "uploads.resolutionTitle", descKey: "uploads.resolutionDesc"},
    {icon: RectangleHorizontal, titleKey: "uploads.aspectRatioTitle", descKey: "uploads.aspectRatioDesc"},
] as const;

const VideoDropzone = ({
                           onFileSelect,
                           file,
                           accept = "video/*",
                           maxSizeMb,
                           disabled = false,
                           className,
                           onError,
                       }: VideoDropzoneProps) => {
    const {t} = useTranslation();
    const [isDragging, setIsDragging] = useState(false);
    const inputRef = useRef<HTMLInputElement>(null);

    const validateAndSelect = (candidate: File) => {
        if (!candidate.type.startsWith("video/")) {
            onError?.(t("uploads.invalidType"));
            return;
        }
        if (maxSizeMb && candidate.size > maxSizeMb * 1024 * 1024) {
            onError?.(t("uploads.fileTooLarge", {maxSizeMb}));
            return;
        }
        onFileSelect(candidate);
    };

    const handleFiles = (files: FileList | null) => {
        if (!files || files.length === 0) return;
        validateAndSelect(files[0]);
    };

    const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        if (!disabled) setIsDragging(true);
    };

    const handleDragLeave = () => setIsDragging(false);

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);
        if (disabled) return;
        handleFiles(e.dataTransfer.files);
    };

    const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
        handleFiles(e.target.files);
        e.target.value = "";
    };

    const openPicker = () => {
        if (!disabled) inputRef.current?.click();
    };

    return (
        <div className={cn("flex flex-col rounded-lg overflow-hidden border", className)}>
            <div
                onClick={openPicker}
                onDragOver={handleDragOver}
                onDragLeave={handleDragLeave}
                onDrop={handleDrop}
                className={cn(
                    "flex flex-col items-center justify-center gap-3 py-10 px-6 transition-colors bg-neutral-100 dark:bg-neutral-900",
                    disabled
                        ? "opacity-50 cursor-not-allowed"
                        : "cursor-pointer",
                    isDragging && "bg-neutral-200 dark:bg-neutral-800"
                )}
            >
                <input
                    ref={inputRef}
                    type="file"
                    accept={accept}
                    disabled={disabled}
                    className="hidden"
                    onChange={handleInputChange}
                />

                <UploadCloud className="h-10 w-10 text-muted-foreground"/>

                <p className="text-sm font-medium text-center">
                    {file ? file.name : t("uploads.dragDropText")}
                </p>

                <Button
                    type="button"
                    variant="secondary"
                    disabled={disabled}
                    onClick={(e) => {
                        e.stopPropagation();
                        openPicker();
                    }}
                >
                    {t("uploads.selectVideo")}
                </Button>
            </div>

            {/* format / resolution info */}
            <div
                className="flex flex-col sm:flex-row justify-center divide-y sm:divide-y-0 sm:divide-x divide-border bg-background border-t">
                {infoItems.map(({icon: Icon, titleKey, descKey}) => (
                    <div key={titleKey} className="flex flex-col gap-1 px-6 py-4 sm:w-1/3">
                        <div className="flex items-center gap-2">
                            <Icon className="h-4 w-4 shrink-0"/>
                            <span className="text-sm font-medium">{t(titleKey)}</span>
                        </div>
                        <p className="text-xs text-muted-foreground leading-relaxed">
                            {t(descKey)}
                        </p>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default VideoDropzone;