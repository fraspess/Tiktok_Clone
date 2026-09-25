import {useEffect, useRef, useState} from "react";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle
} from "@/components/ui/dialog.tsx";
import {Button} from "@/components/ui/button.tsx";
import {ChevronDown, Clapperboard, LogOut, Moon, Search, ShieldCheck, Sun, User} from "lucide-react";
import {useTheme} from "next-themes";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Link} from "react-router-dom";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {authApi, useLogoutAllMutation, useLogoutMutation} from "@/store/apis/authApi.ts";
import {videoApi} from "@/store/apis/videoApi.ts";
import {useGetMeQuery, userApi} from "@/store/apis/userApi.ts";
import {commentApi} from "@/store/apis/commentApi.ts";
import {logout as logoutAction} from "@/store/slices/authSlice.ts";
import {cn} from "@/lib/utils.ts";
import {hasAdminRole} from "@/lib/jwt.ts";
import UserAvatar from "@/components/chat/UserAvatar.tsx";

interface TopbarProps {
    /** true — на мобільці панель накладається поверх відео (стрічка), false — окрема смужка над контентом */
    overlay?: boolean;
}

const Topbar = ({overlay = false}: TopbarProps) => {
    const {theme, setTheme} = useTheme();
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const isAuth = useAppSelector(state => state.auth.isAuth);
    const accessToken = useAppSelector(state => state.auth.accessToken);
    const isAdmin = hasAdminRole(accessToken);
    const {data: me} = useGetMeQuery(undefined, {skip: !isAuth});
    const username = me?.data.username;
    const [logout, {isLoading: isLoggingOut}] = useLogoutMutation();

    const [logoutAll, {isLoading: isLoggingOutAll}] = useLogoutAllMutation();
    const [allOpen, setAllOpen] = useState(false);
    const [accountMenuOpen, setAccountMenuOpen] = useState(false);
    const accountMenuRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!accountMenuOpen) return;
        const closeMenu = (event: MouseEvent) => {
            if (!accountMenuRef.current?.contains(event.target as Node)) setAccountMenuOpen(false);
        };
        const closeOnEscape = (event: KeyboardEvent) => {
            if (event.key === "Escape") setAccountMenuOpen(false);
        };
        document.addEventListener("mousedown", closeMenu);
        document.addEventListener("keydown", closeOnEscape);
        return () => {
            document.removeEventListener("mousedown", closeMenu);
            document.removeEventListener("keydown", closeOnEscape);
        };
    }, [accountMenuOpen]);
    const handleLogoutAll = async () => {
        try {
            await logoutAll().unwrap();
            dispatch(logoutAction());
            window.location.href = "/";
        } catch {
            toast.error(t("auth.logoutAllError"));
        }
    };
    const handleLogout = async () => {
        try {
            await logout(undefined).unwrap();
        } catch {
            toast.error(t("auth.logoutError"));
        } finally {
            dispatch(logoutAction());
            dispatch(authApi.util.resetApiState());
            dispatch(videoApi.util.resetApiState());
            dispatch(userApi.util.resetApiState());
            dispatch(commentApi.util.resetApiState());
            window.location.href = "/";
        }
    };

    return (
        <div className={cn(
            "z-50 flex items-center gap-1 md:fixed md:top-4 md:right-4 md:gap-2 md:p-0",
            overlay
                ? "fixed right-2 top-[max(0.5rem,env(safe-area-inset-top))] rounded-full bg-background/60 p-0.5 backdrop-blur md:top-4 md:right-4 md:bg-transparent md:backdrop-blur-none"
                : "justify-end px-2 pb-1.5 pt-[calc(env(safe-area-inset-top,0px)+0.375rem)]"
        )}>
            <Button asChild variant="ghost" size="icon" className="md:hidden" aria-label={t("nav.search")}>
                <Link to="/search"><Search className="h-4 w-4"/></Link>
            </Button>
            {isAuth ? (
                <>
                    <Button asChild variant="ghost" size="icon" className="md:hidden" aria-label={t("nav.studio")}>
                        <Link to="/studio"><Clapperboard className="h-4 w-4"/></Link>
                    </Button>
                    {isAdmin && (
                        <Button asChild variant="ghost" size="icon" className="md:hidden" aria-label={t("nav.admin")}>
                            <Link to="/admin"><ShieldCheck className="h-4 w-4"/></Link>
                        </Button>
                    )}
                    <div ref={accountMenuRef} className="relative">
                        <Button
                            type="button"
                            variant="ghost"
                            className="h-10 gap-1 rounded-full p-0.5 pr-1.5"
                            onClick={() => setAccountMenuOpen(open => !open)}
                            aria-label={t("profile.myProfile")}
                            aria-expanded={accountMenuOpen}
                            aria-haspopup="menu"
                        >
                            <UserAvatar username={username ?? "?"} avatar={me?.data.avatar} size="sm"
                                        className="h-8 w-8"/>
                            <ChevronDown className="h-3.5 w-3.5"/>
                        </Button>
                        {accountMenuOpen && (
                            <div
                                role="menu"
                                className="absolute right-0 top-[calc(100%+0.5rem)] z-50 w-56 overflow-hidden rounded-lg border bg-popover p-1 text-popover-foreground shadow-lg"
                            >
                                {username && (
                                    <Link
                                        to={`/@${username}`}
                                        role="menuitem"
                                        onClick={() => setAccountMenuOpen(false)}
                                        className="flex items-center gap-2 rounded-md px-2.5 py-2 text-sm outline-none hover:bg-accent focus:bg-accent"
                                    >
                                        <User className="h-4 w-4"/>{t("profile.myProfile")}
                                    </Link>
                                )}
                                <button
                                    type="button"
                                    role="menuitem"
                                    onClick={() => {
                                        setAccountMenuOpen(false);
                                        void handleLogout();
                                    }}
                                    disabled={isLoggingOut}
                                    className="flex w-full items-center gap-2 rounded-md px-2.5 py-2 text-left text-sm outline-none hover:bg-accent focus:bg-accent disabled:pointer-events-none disabled:opacity-50"
                                >
                                    <LogOut className="h-4 w-4"/>{t("auth.logout")}
                                </button>
                                <button
                                    type="button"
                                    role="menuitem"
                                    onClick={() => {
                                        setAccountMenuOpen(false);
                                        setAllOpen(true);
                                    }}
                                    disabled={isLoggingOutAll}
                                    className="flex w-full items-center gap-2 rounded-md px-2.5 py-2 text-left text-sm text-destructive outline-none hover:bg-destructive/10 focus:bg-destructive/10 disabled:pointer-events-none disabled:opacity-50"
                                >
                                    <LogOut className="h-4 w-4"/><span>{t("auth.logoutAll")}</span>
                                </button>
                            </div>
                        )}
                    </div>
                    <Dialog open={allOpen} onOpenChange={open => {
                        if (!isLoggingOutAll) setAllOpen(open);
                    }}>
                        <DialogContent><DialogHeader><DialogTitle>{t("auth.logoutAll")}</DialogTitle><DialogDescription>{t("auth.logoutAllConfirm")}</DialogDescription></DialogHeader>
                            <DialogFooter><Button variant="outline" disabled={isLoggingOutAll}
                                                  onClick={() => setAllOpen(false)}>{t("report.cancel")}</Button>
                                <Button disabled={isLoggingOutAll}
                                        onClick={() => void handleLogoutAll()}>{t("auth.logoutAll")}</Button></DialogFooter>
                        </DialogContent>
                    </Dialog>
                </>
            ) : (
                <Button onClick={() => dispatch(openModal())} className="w-20">
                    {t("auth.signInTitle")}
                </Button>
            )}
            {theme == "dark" ? (
                <Button onClick={() => setTheme("white")} variant="ghost" size="icon">
                    <Moon className="h-4 w-4"/>
                </Button>
            ) : (
                <Button onClick={() => setTheme("dark")} variant="ghost" size="icon">
                    <Sun className="h-4 w-4"/>
                </Button>
            )}
        </div>
    )
}

export default Topbar;
