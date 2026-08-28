import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Button} from "@/components/ui/button.tsx";
import {formatCount} from "@/lib/utils.ts";
import {useFollowUserMutation} from "@/store/apis/userApi.ts";
import {useCreateConversationMutation} from "@/store/apis/conversationApi.ts";
import {openMessagesWith} from "@/store/slices/messagesSlice.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import ProfileEditDialog from "@/components/profile/ProfileEditDialog.tsx";
import type {UserProfile} from "@/types/User.ts";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {openDrawer} from "@/store/slices/messagesDrawerSlice.ts";
import {Send} from "lucide-react";

interface ProfileHeaderProps {
    profile: UserProfile;
}

const ProfileHeader = ({profile}: ProfileHeaderProps) => {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();

    const isAuth = useAppSelector((s) => s.auth.isAuth);

    const [followUser, {isLoading}] = useFollowUserMutation();
    const [createConversation] = useCreateConversationMutation();

    const [isFollowing, setIsFollowing] = useState(profile.isFollowing);
    const [followersCount, setFollowersCount] = useState(profile.followersCount);
    const [isEditOpen, setIsEditOpen] = useState(false);

    useEffect(() => {
        setIsFollowing(profile.isFollowing);
        setFollowersCount(profile.followersCount);
    }, [profile.id, profile.isFollowing, profile.followersCount]);

    const handleFollow = async () => {
        if (isFollowing || isLoading) return;

        setIsFollowing(true);
        setFollowersCount((prev) => prev + 1);

        try {
            await followUser({
                followingId: profile.id,
                username: profile.username
            }).unwrap();
        } catch (err) {
            setIsFollowing(false);
            setFollowersCount((prev) => prev - 1);

            const message =
                isFetchBaseQueryError(err) &&
                typeof err.data === "object" &&
                err.data &&
                "message" in err.data
                    ? String((err.data as { message?: string }).message)
                    : t("profile.followError");

            toast.error(message || t("profile.followError"));
        }
    };

    const handleSendMessage = async () => {
        if (!isAuth) {
            dispatch(openModal());
            return;
        }

        try {
            await createConversation({
                userId: profile.id
            }).unwrap();

            dispatch(
                openMessagesWith({
                    username: profile.username,
                    userId: profile.id
                })
            );

            dispatch(openDrawer());
        } catch (err) {
            const message =
                isFetchBaseQueryError(err) &&
                typeof err.data === "object" &&
                err.data &&
                "message" in err.data
                    ? String((err.data as { message?: string }).message)
                    : t("profile.sendMessageError");

            toast.error(message || t("profile.sendMessageError"));
        }
    };

    return (
        <div
            className="flex flex-col items-center gap-4 px-4 py-8 text-center sm:flex-row sm:items-start sm:text-left"
        >
            <div className="h-28 w-28 shrink-0 overflow-hidden rounded-full bg-neutral-700">
                {profile.avatar?.large ? (
                    <img
                        src={profile.avatar.large}
                        alt={profile.username}
                        className="h-full w-full object-cover"
                    />
                ) : (
                    <div
                        className="flex h-full w-full items-center justify-center text-3xl font-semibold text-white"
                    >
                        {profile.username[0]?.toUpperCase() ?? "?"}
                    </div>
                )}
            </div>

            <div className="flex flex-1 flex-col items-center gap-3 sm:items-start">
                <div className="flex flex-col items-center gap-2 sm:flex-row sm:items-center">
                    <h1 className="text-xl font-semibold">
                        @{profile.username}
                    </h1>

                    {profile.isOwnProfile ? (
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => setIsEditOpen(true)}
                        >
                            {t("profile.edit.trigger")}
                        </Button>
                    ) : (
                        <>
                            <Button
                                type="button"
                                onClick={handleFollow}
                                disabled={isFollowing || isLoading}
                                variant={isFollowing ? "outline" : "default"}
                            >
                                {isFollowing
                                    ? t("profile.followingStatus")
                                    : t("profile.follow")}
                            </Button>

                            <Button
                                type="button"
                                variant="outline"
                                onClick={handleSendMessage}
                            >
                                <Send className="mr-2 h-4 w-4"/>
                                {t("profile.message")}
                            </Button>
                        </>
                    )}
                </div>

                <div className="flex items-center gap-4 text-sm">
                    <span>
                        <span className="font-semibold">
                            {formatCount(followersCount)}
                        </span>{" "}
                        <span className="text-muted-foreground">
                            {t("profile.followers")}
                        </span>
                    </span>

                    <span>
                        <span className="font-semibold">
                            {formatCount(profile.followingCount)}
                        </span>{" "}
                        <span className="text-muted-foreground">
                            {t("profile.following")}
                        </span>
                    </span>
                </div>

                {profile.description && (
                    <p className="max-w-md text-sm text-muted-foreground">
                        {profile.description}
                    </p>
                )}
            </div>

            {profile.isOwnProfile && (
                <ProfileEditDialog
                    profile={profile}
                    open={isEditOpen}
                    onOpenChange={setIsEditOpen}
                />
            )}
        </div>
    );
};

export default ProfileHeader;