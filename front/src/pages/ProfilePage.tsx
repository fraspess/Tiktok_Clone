import {useCallback, useState} from "react";
import {Link, useParams} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {ArrowLeft, Bookmark, Grid3X3, Heart, Loader2} from "lucide-react";
import {useGetUserProfileQuery} from "@/store/apis/userApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import ProfileHeader from "@/components/profile/ProfileHeader.tsx";
import ProfileVideoGrid from "@/components/profile/ProfileVideoGrid.tsx";
import CachedVideoGrid from "@/components/profile/CachedVideoGrid.tsx";
import {cn} from "@/lib/utils.ts";
import type {VideoDto} from "@/types/Video.ts";

type ProfileTab = "videos" | "liked" | "saved";

// import ProfileVideoTabs from "@/components/profile/ProfileVideoTabs.tsx"; те що вище закоментувати
const ProfilePage = () => {
    const {username: rawUsername} = useParams<{ username: string }>();
    const username = rawUsername?.startsWith("@") ? rawUsername.slice(1) : rawUsername;
    const {t} = useTranslation();
    const {data, isLoading, isFetching, isError, error} = useGetUserProfileQuery(username ?? "", {
        skip: !username,
    });
    const [activeTab, setActiveTab] = useState<ProfileTab>("videos");
    const profile = data?.data;

    const isLiked = useCallback(
        (video: VideoDto) => video.isLiked,
        []
    );
    const isSaved = useCallback(
        (video: VideoDto) => video.isFavorited,
        []
    );

    const backToFeedButton = (
        <Link
            to="/"
            className="inline-flex items-center gap-1.5 px-4 pt-4 text-sm font-medium text-muted-foreground hover:text-foreground"
        >
            <ArrowLeft size={16}/>
            {t("profile.backToFeed")}
        </Link>
    );

    if (!username || (isLoading && !data)) {
        return (
            <div className="flex h-full w-full flex-col overflow-hidden">
                {backToFeedButton}
                <div className="flex flex-1 items-center justify-center">
                    <Loader2 className="h-8 w-8 animate-spin text-muted-foreground"/>
                </div>
            </div>
        );
    }

    if (isError) {
        const isNotFound = isFetchBaseQueryError(error) && error.status === 404;
        return (
            <div className="flex h-full w-full flex-col overflow-hidden">
                {backToFeedButton}
                <div className="flex flex-1 items-center justify-center text-muted-foreground">
                    {isNotFound ? t("profile.notFound") : t("profile.loadError")}
                </div>
            </div>
        );
    }

    if (!profile) {
        return (
            <div className="flex h-full w-full flex-col overflow-hidden">
                {backToFeedButton}
                <div className="flex flex-1 items-center justify-center text-muted-foreground">
                    {isFetching ? <Loader2 className="h-8 w-8 animate-spin"/> : t("profile.notFound")}
                </div>
            </div>
        );
    }

    return (
        <div className="flex h-full w-full flex-col overflow-hidden">
            {backToFeedButton}
            <ProfileHeader profile={profile}/>

            <div className="flex items-center justify-center border-b ">
                <button
                    type="button"
                    onClick={() => setActiveTab("videos")}
                    className={cn(
                        "flex items-center gap-1.5 border-b-2 px-4 py-2.5 text-sm font-medium transition-colors",
                        activeTab === "videos"
                            ? "border-foreground text-foreground"
                            : "border-transparent text-muted-foreground hover:text-foreground"
                    )}
                >
                    <Grid3X3 size={16}/>
                    {t("profile.tabs.videos")}
                </button>
                <button
                    type="button"
                    onClick={() => setActiveTab("liked")}
                    className={cn(
                        "flex items-center gap-1.5 border-b-2 px-4 py-2.5 text-sm font-medium transition-colors",
                        activeTab === "liked"
                            ? "border-foreground text-foreground"
                            : "border-transparent text-muted-foreground hover:text-foreground"
                    )}
                >
                    <Heart size={16}/>
                    {t("profile.tabs.liked")}
                </button>
                {profile.isOwnProfile && (
                    <button
                        type="button"
                        onClick={() => setActiveTab("saved")}
                        className={cn(
                            "flex items-center gap-1.5 border-b-2 px-4 py-2.5 text-sm font-medium transition-colors",
                            activeTab === "saved"
                                ? "border-foreground text-foreground"
                                : "border-transparent text-muted-foreground hover:text-foreground"
                        )}
                    >
                        <Bookmark size={16}/>
                        {t("profile.tabs.saved")}
                    </button>
                )}
            </div>

            <div className="min-h-0 flex-1 pt-4">
                {activeTab === "videos" && (
                    <ProfileVideoGrid userId={profile.id} username={profile.username}/>
                )}
                {activeTab === "liked" && (
                    <CachedVideoGrid filter={isLiked} username={profile.username}/>
                )}
                {activeTab === "saved" && profile.isOwnProfile && (
                    <CachedVideoGrid filter={isSaved} username={profile.username}/>
                )}
                <ProfileVideoGrid userId={profile.id} username={profile.username}/>
                {/*<ProfileVideoTabs userId={profile.id} username={profile.username} isOwnProfile={profile.isOwnProfile}/>*/}
            </div>
        </div>
    );
};

export default ProfilePage;
