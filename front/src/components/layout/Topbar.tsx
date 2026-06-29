import {Button} from "@/components/ui/button.tsx";
import {Moon, Sun} from "lucide-react";
import {useTheme} from "next-themes";
import {useTranslation} from "react-i18next";
import {useAppDispatch} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";

const Topbar = () => {
    const { theme, setTheme } = useTheme();
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    return(
            <header className="h-16 shrink-0 px-4 flex items-center justify-end gap-2">
                <Button onClick={() => dispatch(openModal())} className="w-20">
                    {t("auth.signInTitle")}
                </Button>
                {theme == "dark" ? (
                    <Button onClick={() => {setTheme("white")}} variant="ghost" size="icon">
                        <Moon className="h-4 w-4" />
                    </Button>

                ) : (
                    <Button onClick={() => {setTheme("dark")}} variant="ghost" size="icon">
                        <Sun className="h-4 w-4" />
                    </Button>
                )}
            </header>
    )
}

export default Topbar;