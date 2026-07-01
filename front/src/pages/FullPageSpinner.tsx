import {Loader2} from "lucide-react";
import MainLayout from "@/components/layout/MainLayout.tsx";

export function FullPageSpinner() {
    return (
        <MainLayout>
            <div className="flex h-full w-full items-center justify-center">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground"/>
            </div>
        </MainLayout>
    );
}