import {X} from "lucide-react";
import {useTranslation} from "react-i18next";
import {
    Dialog,
    DialogClose,
    DialogContent,
    DialogTitle,
} from "@/components/ui/dialog.tsx";
import MessagesPage from "@/pages/MessagesPage.tsx";

interface MessagesDrawerProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

const MessagesDrawer = ({open, onOpenChange}: MessagesDrawerProps) => {
    const {t} = useTranslation();

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent
            showCloseButton={false}
            className="inset-y-0 left-0 top-0 z-50 h-full w-full max-w-[900px] translate-x-0 translate-y-0 gap-0 rounded-none border-0 border-r bg-background p-0 text-foreground shadow-2xl shadow-black/30 data-[state=open]:slide-in-from-left data-[state=closed]:slide-out-to-left sm:w-[900px] sm:max-w-[900px]"
        >
                <DialogTitle className="sr-only">{t("chat.inbox")}</DialogTitle>
                <div className="absolute right-3 top-3 z-10">
                    <DialogClose asChild>
                        <button
                            type="button"
                            aria-label={t("chat.closeMessages")}
                            className="flex h-9 w-9 items-center justify-center rounded-full text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
                        >
                            <X className="h-5 w-5"/>
                        </button>
                    </DialogClose>
                </div>
                <MessagesPage/>
        </DialogContent>
        </Dialog>
    );
};

export default MessagesDrawer;
