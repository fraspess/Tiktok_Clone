import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Button} from "@/components/ui/button.tsx";
import UserAvatar from "@/components/profile/UserAvatar.tsx";
import {useFollowUserMutation} from "@/store/apis/userApi.ts";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import type {UserProfileDto} from "@/types/User.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";

interface ProfileHeaderProps {
    profile: UserProfileDto;
}

const ProfileHeader = ({profile}: ProfileHeaderProps) => {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const isAuth = useAppSelector((state) => state.auth.isAuth);
    const [followUser, {isLoading}] = useFollowUserMutation();

    const handleFollowClick = async () => {
        if (!isAuth) {
            dispatch(openModal());
            return;
        }
        try {
            await followUser({userId: profile.id, username: profile.username}).unwrap();
        } catch (err) {
            const message = isFetchBaseQueryError(err)
                ? (err.data as ApiResponse<null>)?.message
                : undefined;
            toast.error(message || t("profile.followError"));
        }
    };

    return (
        <div className="flex flex-col items-center gap-4 px-4 pt-10 pb-6 text-center sm:flex-row sm:items-start sm:text-left">
            <UserAvatar avatar={profile.avatar} size="large"/>

            <div className="flex flex-1 flex-col items-center gap-3 sm:items-start">
                <div>
                    <h1 className="text-xl font-semibold">@{profile.username}</h1>
                </div>

                <div className="flex items-center gap-5 text-sm">
                    <div>
                        <span className="font-semibold">{profile.followingCount}</span>{" "}
                        <span className="text-muted-foreground">{t("profile.following")}</span>
                    </div>
                    <div>
                        <span className="font-semibold">{profile.followersCount}</span>{" "}
                        <span className="text-muted-foreground">{t("profile.followers")}</span>
                    </div>
                </div>

                {profile.description && (
                    <p className="max-w-md text-sm whitespace-pre-line text-foreground/90">
                        {profile.description}
                    </p>
                )}

                {!profile.isOwnProfile && (
                    <Button
                        onClick={handleFollowClick}
                        disabled={isLoading}
                        variant={profile.isFollowing ? "outline" : "default"}
                        className="w-32"
                    >
                        {profile.isFollowing ? t("profile.unfollow") : t("profile.follow")}
                    </Button>
                )}
            </div>
        </div>
    );
};

export default ProfileHeader;
