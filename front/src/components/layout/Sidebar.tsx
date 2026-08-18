import {NavLink} from "react-router-dom";
import {cn} from "@/lib/utils.ts";
import {Button} from "@/components/ui/button.tsx";
import {Tooltip, TooltipContent, TooltipTrigger} from "@/components/ui/tooltip.tsx";
import {useTranslation} from "react-i18next";
import {Book, Home, MessageCircle} from "lucide-react";

interface SidebarProps {
    collapsed: boolean;
    onToggle: () => void;
    isMessagesOpen: boolean;
    onMessagesClick: () => void;
}

const navLinkClass = ({isActive}: { isActive: boolean }) =>
    cn(
        "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
        isActive
            ? "bg-neutral-100 text-foreground dark:bg-neutral-800"
            : "text-muted-foreground hover:bg-neutral-50 hover:text-foreground dark:hover:bg-neutral-900"
    );

const Sidebar = ({collapsed, onToggle, isMessagesOpen, onMessagesClick}: SidebarProps) => {
    const {t} = useTranslation();

    const navItems = [
        {to: "/", label: t("nav.home"), icon: Home, end: true},
    ];

    return (
        <aside className={cn(
            "sticky top-0 m-4 ml-2 flex h-screen flex-col border-r pr-3 transition-all duration-300",
            collapsed ? "w-12" : "w-64"
        )}>
            <div className={cn(
                "flex items-center p-2",
                collapsed ? "justify-center" : "justify-between"
            )}>
                {!collapsed && <h1 className="text-lg font-bold">TikTok Clone</h1>}
                <Tooltip>
                    <TooltipTrigger asChild>
                        <Button variant="ghost" size="icon" onClick={onToggle}>
                            <Book/>
                        </Button>
                    </TooltipTrigger>
                    <TooltipContent side="right">
                        <p>{t("collapseSidebar")}</p>
                    </TooltipContent>
                </Tooltip>
            </div>

            <nav className="mt-2 flex flex-col gap-1 px-1">
                {navItems.map(({to, label, icon: Icon, end}) => (
                    collapsed ? (
                        <Tooltip key={to}>
                            <TooltipTrigger asChild>
                <NavLink to={to} end={end} className={navLinkClass}>
                                    <Icon className="mx-auto h-5 w-5 shrink-0"/>
                                </NavLink>
                            </TooltipTrigger>
                            <TooltipContent side="right">
                                <p>{label}</p>
                            </TooltipContent>
                        </Tooltip>
                    ) : (
                        <NavLink key={to} to={to} end={end} className={navLinkClass}>
                            <Icon className="h-5 w-5 shrink-0"/>
                            <span>{label}</span>
                </NavLink>
            )
        ))}
                {collapsed ? (
                    <Tooltip>
                        <TooltipTrigger asChild>
                            <button type="button" onClick={onMessagesClick} className={cn(navLinkClass({isActive: isMessagesOpen}), "w-full")}>
                                <MessageCircle className="mx-auto h-5 w-5 shrink-0"/>
                            </button>
                        </TooltipTrigger>
                        <TooltipContent side="right"><p>{t("nav.messages")}</p></TooltipContent>
                    </Tooltip>
                ) : (
                    <button type="button" onClick={onMessagesClick} className={cn(navLinkClass({isActive: isMessagesOpen}), "w-full")}>
                        <MessageCircle className="h-5 w-5 shrink-0"/>
                        <span>{t("nav.messages")}</span>
                    </button>
                )}
            </nav>
        </aside>
    );
};

export default Sidebar;
