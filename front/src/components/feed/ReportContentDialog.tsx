import {useState} from "react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Button} from "@/components/ui/button.tsx";
import {Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {useGetReportReasonsQuery} from "@/store/apis/adminApi.ts";
import {useReportContentMutation} from "@/store/apis/videoApi.ts";
import type {ReportType} from "@/types/Admin.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";

export default function ReportContentDialog({contentId, contentType, open, onOpenChange}: {
    contentId: string; contentType: ReportType; open: boolean; onOpenChange: (open: boolean) => void;
}) {
    const {t} = useTranslation();
    const {currentData, isFetching, isError, refetch} = useGetReportReasonsQuery(contentType, {skip: !open});
    const [submit, {isLoading}] = useReportContentMutation();
    const [reason, setReason] = useState("");
    const [details, setDetails] = useState("");
    const [error, setError] = useState("");
    const reasons = currentData?.data ?? [];
    const selected = reasons.find(item => String(item.id) === reason);
    const changeOpen = (next: boolean) => {
        if (isLoading) return;
        setReason(""); setDetails(""); setError(""); onOpenChange(next);
    };
    const send = async () => {
        if (!selected) { setError(t("report.selectReasonError")); return; }
        if (selected.name === "Other" && !details.trim()) { setError(t("report.customReasonRequiredError")); return; }
        try {
            await submit({contentId, contentType, reason: selected.id, customReason: details.trim() || undefined}).unwrap();
            toast.success(t("report.success"));
            setReason(""); setDetails(""); setError(""); onOpenChange(false);
        } catch (err) {
            const code = isFetchBaseQueryError(err) && err.data && typeof err.data === "object" && "code" in err.data ? err.data.code : null;
            setError(t(code === "AlreadyExists" || code === "ALREADY_EXISTS" || code === "DUPLICATE" ? "report.duplicateContent" : "report.error"));
        }
    };
    return <Dialog open={open} onOpenChange={changeOpen}><DialogContent>
        <DialogHeader><DialogTitle>{t(`report.titles.${contentType}`)}</DialogTitle><DialogDescription>{t("report.chooseReason")}</DialogDescription></DialogHeader>
        {isFetching ? <p role="status">{t("auth.loading")}</p> : isError ?
            <div role="alert">{t("report.reasonsError")} <Button onClick={() => void refetch()}>{t("chat.privacy.retry")}</Button></div> :
            <fieldset className="max-h-64 space-y-2 overflow-y-auto" disabled={isLoading}>
                <legend className="sr-only">{t("report.chooseReason")}</legend>
                {reasons.map(item => <label key={item.id} className="flex items-center gap-3 text-sm">
                    <input type="radio" name={`report-${contentType}-${contentId}`} value={item.id} checked={reason === String(item.id)}
                        onChange={() => {setReason(String(item.id)); setError("");}}/>
                    {t(`report.reasonNames.${item.name}`, {defaultValue: item.description || item.name || t("admin.unknownReason")})}
                </label>)}
            </fieldset>}
        <label className="space-y-1 text-sm">{t(selected?.name === "Other" ? "report.requiredDetails" : "report.customReasonLabel")}
            <textarea value={details} onChange={e => setDetails(e.target.value)} maxLength={255} disabled={isLoading}
                className="w-full rounded border p-2" placeholder={t("report.customReasonPlaceholder")}/>
        </label>
        {error && <p role="alert" className="text-destructive">{error}</p>}
        <DialogFooter><Button variant="outline" disabled={isLoading} onClick={() => changeOpen(false)}>{t("report.cancel")}</Button>
            <Button disabled={isLoading || isFetching || isError || !selected} onClick={() => void send()}>{t("report.submit")}</Button></DialogFooter>
    </DialogContent></Dialog>;
}
