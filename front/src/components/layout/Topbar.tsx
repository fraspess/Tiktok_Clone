import {Button} from "@/components/ui/button.tsx";
import {LogOut, Moon, Sun, User} from "lucide-react";
import {useTheme} from "next-themes";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Link} from "react-router-dom";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {authApi, useLogoutMutation} from "@/store/apis/authApi.ts";
import {videoApi} from "@/store/apis/videoApi.ts";
import {userApi, useGetMeQuery} from "@/store/apis/userApi.ts";
import {commentApi} from "@/store/apis/commentApi.ts";
import {logout as logoutAction} from "@/store/slices/authSlice.ts";

const Topbar = () => {
    const {theme, setTheme} = useTheme();
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const isAuth = useAppSelector(state => state.auth.isAuth);
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
        }
    };

    return (
        <header className="h-16 shrink-0 px-4 flex items-center justify-end gap-2">
            {isAuth ? (
                <>
                    {username && (
                        <Button asChild variant="ghost" className="gap-2">
                            <Link to={`/${username}`}>
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
                    >
                        <LogOut className="h-4 w-4"/>
                        {t("auth.logout")}
                    </Button>
                </>
            ) : (
                <Button onClick={() => dispatch(openModal())} className="w-20">
                    {t("auth.signInTitle")}
                </Button>
            )}
            {theme == "dark" ? (
                <Button onClick={() => {
                    setTheme("white")
                }} variant="ghost" size="icon">
                    <Moon className="h-4 w-4"/>
                </Button>

            ) : (
                <Button onClick={() => {
                    setTheme("dark")
                }} variant="ghost" size="icon">
                    <Sun className="h-4 w-4"/>
                </Button>
            )}
        </header>
    )
}

export default Topbar;