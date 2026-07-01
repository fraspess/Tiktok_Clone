import {cn} from "@/lib/utils.ts";
import {Button} from "@/components/ui/button.tsx";
import {Tooltip, TooltipContent, TooltipTrigger} from "@/components/ui/tooltip.tsx";
import {useTranslation} from "react-i18next";
import {Book} from "lucide-react";

interface SidebarProps{
    collapsed:boolean;
    onToggle: () => void;
}
const Sidebar = ({collapsed, onToggle} : SidebarProps) => {
    const {t} = useTranslation();
    return(
        <aside className={cn("" +
            "h-screen sticky top-0 border-r flex flex-col transition-all duration-300 m-4 ml-2 pr-3",
            collapsed ? "w-12" : "w-64"
        )}>
            <div
                className={cn("items-center p-2 flex",
                collapsed ? "justify-center" : "justify-between"
                )}>
                {!collapsed && (<h1>TikTok Clone</h1>)}
            <Tooltip>
                <TooltipTrigger asChild>
                    <Button variant="ghost" className="" onClick={onToggle}>
                        <Book></Book>
                    </Button>

                </TooltipTrigger>
                <TooltipContent side="right">
                    <p>{t("collapseSidebar")}</p>
                </TooltipContent>
            </Tooltip>

            </div>
        </aside>
    )
}

export default Sidebar