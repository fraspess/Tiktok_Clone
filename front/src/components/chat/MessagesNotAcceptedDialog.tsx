import {useTranslation} from "react-i18next";
import {Button} from "@/components/ui/button.tsx";
import {Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";

export default function MessagesNotAcceptedDialog({open, onOpenChange}: {
    open: boolean;
    onOpenChange: (open: boolean) => void;
}) {
    const {t} = useTranslation();
    return <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent>
            <DialogHeader>
                <DialogTitle>{t("chat.messagesNotAccepted.title")}</DialogTitle>
                <DialogDescription>{t("chat.messagesNotAccepted.description")}</DialogDescription>
            </DialogHeader>
            <DialogFooter>
                <DialogClose asChild><Button>{t("chat.messagesNotAccepted.close")}</Button></DialogClose>
            </DialogFooter>
        </DialogContent>
    </Dialog>;
}
