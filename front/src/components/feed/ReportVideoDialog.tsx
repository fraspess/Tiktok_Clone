import {useState} from "react";
import {useTranslation} from "react-i18next";
import {RadioGroup} from "radix-ui";
import {toast} from "sonner";
import {Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle,} from "@/components/ui/dialog.tsx";
import {Button} from "@/components/ui/button.tsx";
import {cn} from "@/lib/utils.ts";
import {useReportVideoMutation} from "@/store/apis/videoApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";

interface ReportVideoDialogProps {
    videoId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

const OTHER_REASON_ID = 6;

const ReportVideoDialog = ({videoId, open, onOpenChange}: ReportVideoDialogProps) => {
    const {t} = useTranslation();
    const [reportVideo, {isLoading}] = useReportVideoMutation();
    const [selectedReason, setSelectedReason] = useState<string>("");
    const [customReason, setCustomReason] = useState("");
    const [formError, setFormError] = useState<string | null>(null);

    const REASONS = [
        {id: 1, labelKey: "report.reasons.spam"},
        {id: 2, labelKey: "report.reasons.inappropriateContent"},
        {id: 3, labelKey: "report.reasons.hateSpeech"},
        {id: 4, labelKey: "report.reasons.harassment"},
        {id: 5, labelKey: "report.reasons.misinformation"},
        {id: OTHER_REASON_ID, labelKey: "report.reasons.other"},
    ];

    const resetState = () => {
        setSelectedReason("");
        setCustomReason("");
        setFormError(null);
    };

    const handleOpenChange = (nextOpen: boolean) => {
        if (!nextOpen) resetState();
        onOpenChange(nextOpen);
    };

    const handleSubmit = async () => {
        if (!selectedReason) {
            setFormError(t("report.selectReasonError"));
            return;
        }
        setFormError(null);

        try {
            await reportVideo({
                contentId: videoId,
                reason: Number(selectedReason),
                customReason: customReason.trim() || undefined,
            }).unwrap();

            toast.success(t("report.success"));
            handleOpenChange(false);
        } catch (err) {
            const message =
                isFetchBaseQueryError(err) && typeof err.data === "object" && err.data && "message" in err.data
                    ? String((err.data as { message?: string }).message)
                    : t("report.error");
            toast.error(message || t("report.error"));
        }
    };

    return (
        <Dialog open={open} onOpenChange={handleOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>{t("report.title")}</DialogTitle>
                    <DialogDescription>{t("report.description")}</DialogDescription>
                </DialogHeader>

                <RadioGroup.Root
                    value={selectedReason}
                    onValueChange={setSelectedReason}
                    className="flex flex-col gap-2"
                >
                    {REASONS.map((reason) => (
                        <label
                            key={reason.id}
                            htmlFor={`report-reason-${reason.id}`}
                            className={cn(
                                "flex cursor-pointer items-center gap-3 rounded-md border border-border px-3 py-2 text-sm transition-colors hover:bg-muted",
                                selectedReason === String(reason.id) && "border-primary bg-muted"
                            )}
                        >
                            <RadioGroup.Item
                                id={`report-reason-${reason.id}`}
                                value={String(reason.id)}
                                className="flex h-4 w-4 shrink-0 items-center justify-center rounded-full border border-input"
                            >
                                <RadioGroup.Indicator className="h-2 w-2 rounded-full bg-primary"/>
                            </RadioGroup.Item>
                            {t(reason.labelKey)}
                        </label>
                    ))}
                </RadioGroup.Root>

                {selectedReason === String(OTHER_REASON_ID) && (
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="report-custom-reason" className="text-sm text-muted-foreground">
                            {t("report.customReasonLabel")}
                        </label>
                        <textarea
                            id="report-custom-reason"
                            value={customReason}
                            onChange={(e) => setCustomReason(e.target.value)}
                            placeholder={t("report.customReasonPlaceholder")}
                            rows={3}
                            className="w-full min-w-0 resize-none rounded-md border border-input bg-transparent px-2.5 py-1.5 text-sm shadow-xs outline-none placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 dark:bg-input/30"
                        />
                    </div>
                )}

                {formError && <p className="text-sm text-destructive">{formError}</p>}

                <DialogFooter>
                    <Button variant="outline" onClick={() => handleOpenChange(false)}>
                        {t("report.cancel")}
                    </Button>
                    <Button onClick={handleSubmit} disabled={isLoading}>
                        {t("report.submit")}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};

export default ReportVideoDialog;
