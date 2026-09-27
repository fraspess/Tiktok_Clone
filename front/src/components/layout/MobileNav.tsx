import {NavLink} from "react-router-dom";
import {Home, MessageCircle, Plus, User, Users} from "lucide-react";
import {useTranslation} from "react-i18next";
import {cn} from "@/lib/utils.ts";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {useGetMeQuery} from "@/store/apis/userApi.ts";

const itemClass = ({isActive}: { isActive: boolean }) =>
    cn(
        "flex h-full min-w-0 flex-1 flex-col items-center justify-center gap-0.5 px-1 text-[10px] font-medium transition-colors",
        isActive ? "text-foreground" : "text-muted-foreground"
    );

/** Нижня навігація для мобільних (< md). На десктопі замість неї працює Sidebar. */
const MobileNav = () => {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const isAuth = useAppSelector((s) => s.auth.isAuth);
    const {data: me} = useGetMeQuery(undefined, {skip: !isAuth});
    const username = me?.data.username;

    return (
        <nav className="shrink-0 border-t bg-background pb-[env(safe-area-inset-bottom)] md:hidden">
            <div className="flex h-14 items-stretch">
                <NavLink to="/" end className={itemClass}>
                    <Home className="h-6 w-6"/>
                    <span className="max-w-full truncate">{t("nav.home")}</span>
                </NavLink>

                <NavLink to="/following" end className={itemClass}>
                    <Users className="h-6 w-6"/>
                    <span className="max-w-full truncate">{t("nav.following")}</span>
                </NavLink>

                <NavLink
                    to="/upload"
                    aria-label={t("uploads.title")}
                    className="flex h-full min-w-0 flex-1 items-center justify-center"
                >
                    <span className="flex h-8 w-11 items-center justify-center rounded-lg bg-primary text-primary-foreground">
                        <Plus className="h-5 w-5" strokeWidth={3}/>
                    </span>
                </NavLink>

                <NavLink to="/messages" end className={itemClass}>
                    <MessageCircle className="h-6 w-6"/>
                    <span className="max-w-full truncate">{t("nav.messages")}</span>
                </NavLink>

                {username ? (
                    <NavLink to={`/@${username}`} end className={itemClass}>
                        <User className="h-6 w-6"/>
                        <span className="max-w-full truncate">{t("profile.myProfile")}</span>
                    </NavLink>
                ) : (
                    <button
                        type="button"
                        onClick={() => dispatch(openModal())}
                        className="flex h-full min-w-0 flex-1 flex-col items-center justify-center gap-0.5 px-1 text-[10px] font-medium text-muted-foreground"
                    >
                        <User className="h-6 w-6"/>
                        <span className="max-w-full truncate">{t("profile.myProfile")}</span>
                    </button>
                )}
            </div>
        </nav>
    );
};

export default MobileNav;
