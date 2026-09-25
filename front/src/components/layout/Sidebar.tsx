import {NavLink} from "react-router-dom";
import {cn} from "@/lib/utils.ts";
import {Button} from "@/components/ui/button.tsx";
import {Tooltip, TooltipContent, TooltipTrigger} from "@/components/ui/tooltip.tsx";
import {useTranslation} from "react-i18next";
import {Book, Clapperboard, Home, MessageCircle, Plus, Search, ShieldCheck, Users} from "lucide-react";
import {useAppSelector} from "@/store/hooks.ts";
import {hasAdminRole} from "@/lib/jwt.ts";
import logo from "@/assets/mascot-full.png";

interface SidebarProps {
    collapsed: boolean;
    onToggle: () => void;
}

const navLinkClass = ({isActive}: { isActive: boolean }) =>
    cn(
        "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
        isActive
            ? "bg-accent text-accent-foreground"
            : "text-muted-foreground hover:bg-accent hover:text-accent-foreground"
    );

const Sidebar = ({collapsed, onToggle}: SidebarProps) => {
    const {t} = useTranslation();
    const accessToken = useAppSelector((s) => s.auth.accessToken);
    const isAuth = useAppSelector((s) => s.auth.isAuth);
    const isAdmin = hasAdminRole(accessToken);

    const navItems = [
        {to: "/", label: t("nav.home"), icon: Home, end: true},
        {to: "/search", label: t("nav.search"), icon: Search, end: true},
        {to: "/following", label: t("nav.following"), icon: Users, end: true},
        {to: "/messages", label: t("nav.messages"), icon: MessageCircle, end: true},
        ...(isAdmin ? [{to: "/admin", label: t("nav.admin"), icon: ShieldCheck, end: true}] : []),
        {to: "/upload", label: t("uploads.title"), icon: Plus},
        ...(isAuth ? [{to: "/studio", label: t("nav.studio"), icon: Clapperboard}] : []),
    ];

    return (
        <aside className={cn(
            "sticky top-0 hidden h-dvh flex-col border-r bg-sidebar text-sidebar-foreground transition-all duration-300 md:max-w-none max-w-[70%] md:flex",
            collapsed ? "w-12" : "w-70"
        )}>
            <div className={cn(
                "flex p-2",
                collapsed ? "items-center justify-center" : "items-center justify-between"
            )}>
                {!collapsed && (
                    <NavLink
                        to="/"
                        aria-label="Much&More"
                        className="flex min-w-0 items-center"
                    >
                        <div className="flex items-center gap-1">
                            <span className="relative block h-8 w-8 shrink-0 overflow-hidden" aria-hidden="true">
                                <img
                                    src={logo}
                                    alt=""
                                    className="absolute left-1/2 top-1/2 h-8 max-w-none -translate-x-1/2 -translate-y-1/2"
                                />
                            </span>
                            <span className="truncate text-lg font-bold leading-8">Much&More</span>
                        </div>
                    </NavLink>
                )}
                <Tooltip>
                    <TooltipTrigger asChild>
                        <Button
                            variant="ghost"
                            size="icon-sm"
                            className="hover:bg-accent"
                            onClick={onToggle}
                            aria-label={t("collapseSidebar")}
                        >
                            <Book className="mx-auto h-5 w-5 shrink-0"/>
                        </Button>
                    </TooltipTrigger>
                    <TooltipContent>
                        <p>{t("collapseSidebar")}</p>
                    </TooltipContent>
                </Tooltip>
            </div>

            <nav className=" flex flex-col gap-1 px-1">
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
                            <span className={cn(
                                "whitespace-nowrap transition-all duration-300",
                                collapsed ? "w-0 opacity-0" : "w-auto opacity-100"
                            )}>{label}</span>
                        </NavLink>
                    )
                ))}
            </nav>
        </aside>
    );
};

export default Sidebar;
