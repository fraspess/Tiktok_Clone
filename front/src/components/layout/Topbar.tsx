import {Button} from "@/components/ui/button.tsx";
import {Clapperboard, LogOut, Moon, Search, ShieldCheck, Sun, User} from "lucide-react";
import {useTheme} from "next-themes";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Link} from "react-router-dom";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {authApi, useLogoutMutation} from "@/store/apis/authApi.ts";
import {videoApi} from "@/store/apis/videoApi.ts";
import {useGetMeQuery, userApi} from "@/store/apis/userApi.ts";
import {commentApi} from "@/store/apis/commentApi.ts";
import {logout as logoutAction} from "@/store/slices/authSlice.ts";
import {cn} from "@/lib/utils.ts";
import {hasAdminRole} from "@/lib/jwt.ts";

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
                    <Button asChild variant="ghost" size="icon" className="md:hidden" aria-label={t("nav.studio", "Мої відео")}>
                        <Link to="/studio"><Clapperboard className="h-4 w-4"/></Link>
                    </Button>
                    {isAdmin && (
                        <Button asChild variant="ghost" size="icon" className="md:hidden" aria-label={t("nav.admin")}>
                            <Link to="/admin"><ShieldCheck className="h-4 w-4"/></Link>
                        </Button>
                    )}
                    {username && (
                        <Button asChild variant="ghost" className="hidden gap-2 md:inline-flex">
                            <Link to={`/@${username}`}>
                                <User className="h-4 w-4"/>
                                {t("profile.myProfile")}
                            </Link>
                        </Button>
                    )}
                    <Button
                        onClick={handleLogout}
                        disabled={isLoggingOut}
                        variant="ghost"
                        className="gap-2"
                        aria-label={t("auth.logout")}
                    >
                        <LogOut className="h-4 w-4"/>
                        <span className="hidden md:inline">{t("auth.logout")}</span>
                    </Button>
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