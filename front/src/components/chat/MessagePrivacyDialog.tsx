import {useState} from "react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Button} from "@/components/ui/button.tsx";
import {Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {useChangeMessagePrivacyMutation, useGetMessagePrivacyQuery} from "@/store/apis/userApi.ts";
import {messagePrivacyOptions, type MessagePrivacy} from "@/lib/messagePrivacy.ts";

function PrivacySettings({onSaved}: {onSaved: () => void}) {
    const {t} = useTranslation();
    const {data, isFetching, isError, refetch} = useGetMessagePrivacyQuery(undefined, {refetchOnMountOrArgChange: true});
    const [changePrivacy, {isLoading: isSaving}] = useChangeMessagePrivacyMutation();
    const [selection, setSelection] = useState<MessagePrivacy | null>(null);
    const [saveError, setSaveError] = useState(false);
    const selected = selection ?? data?.data;

    const save = async () => {
        if (selected == null || isSaving) return;
        setSaveError(false);
        try {
            await changePrivacy(selected).unwrap();
            toast.success(t("chat.privacy.saved"));
            onSaved();
        } catch {
            setSaveError(true);
        }
    };

    if (isFetching) return <p role="status">{t("chat.privacy.loading")}</p>;
    if (isError || data?.data == null) return (
        <div className="space-y-3">
            <p role="alert">{t("chat.privacy.loadError")}</p>
            <Button variant="outline" onClick={() => void refetch()}>{t("chat.privacy.retry")}</Button>
        </div>
    );

    return <>
        <fieldset disabled={isSaving} className="space-y-2">
            <legend className="sr-only">{t("chat.privacy.title")}</legend>
            {messagePrivacyOptions.map(({value, key}) => (
                <label key={value} className="flex cursor-pointer items-start gap-3 rounded-lg border border-border p-3 has-[:checked]:border-primary has-[:checked]:bg-primary/5">
                    <input type="radio" name="messagePrivacy" value={value} checked={selected === value}
                           onChange={() => setSelection(value)} className="mt-1 accent-primary"/>
                    <span>
                        <span className="block font-medium">{t(`chat.privacy.${key}`)}</span>
                        <span className="block text-xs text-muted-foreground">{t(`chat.privacy.${key}Description`)}</span>
                    </span>
                </label>
            ))}
        </fieldset>
        {saveError && <p role="alert" className="text-sm text-destructive">{t("chat.privacy.saveError")}</p>}
        <DialogFooter>
            <Button onClick={() => void save()} disabled={isSaving || selected === data.data}>
                {t(isSaving ? "chat.privacy.saving" : "chat.privacy.save")}
            </Button>
        </DialogFooter>
    </>;
}

export default function MessagePrivacyDialog({open, onOpenChange}: {
    open: boolean;
    onOpenChange: (open: boolean) => void;
}) {
    const {t} = useTranslation();
    return <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="max-h-[90dvh] overflow-y-auto">
            <DialogHeader>
                <DialogTitle>{t("chat.privacy.title")}</DialogTitle>
                <DialogDescription>{t("chat.privacy.description")}</DialogDescription>
            </DialogHeader>
            {open && <PrivacySettings onSaved={() => onOpenChange(false)}/>}
        </DialogContent>
    </Dialog>;
}
