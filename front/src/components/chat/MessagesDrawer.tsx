import {X} from "lucide-react";
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

const MessagesDrawer = ({open, onOpenChange}: MessagesDrawerProps) => (
    <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent
            showCloseButton={false}
            className="inset-y-0 left-0 top-0 z-50 h-full w-full max-w-[900px] translate-x-0 translate-y-0 gap-0 rounded-none border-0 border-r border-white/10 bg-[#121212] p-0 text-white shadow-2xl shadow-black/60 data-[state=open]:slide-in-from-left data-[state=closed]:slide-out-to-left sm:w-[900px] sm:max-w-[900px]"
        >
                <DialogTitle className="sr-only">Повідомлення</DialogTitle>
                <div className="absolute right-3 top-3 z-10">
                    <DialogClose asChild>
                        <button
                            type="button"
                            aria-label="Закрити повідомлення"
                            className="flex h-9 w-9 items-center justify-center rounded-full text-white/70 transition-colors hover:bg-white/10 hover:text-white"
                        >
                            <X className="h-5 w-5"/>
                        </button>
                    </DialogClose>
                </div>
                <MessagesPage/>
        </DialogContent>
    </Dialog>
);

export default MessagesDrawer;
