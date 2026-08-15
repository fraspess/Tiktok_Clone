import {Link, useParams} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {ArrowLeft, Loader2} from "lucide-react";
import {useGetUserProfileQuery} from "@/store/apis/userApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import ProfileHeader from "@/components/profile/ProfileHeader.tsx";
import ProfileVideoGrid from "@/components/profile/ProfileVideoGrid.tsx";

const ProfilePage = () => {
    const {username: rawUsername} = useParams<{ username: string }>();
    const username = rawUsername?.startsWith("@") ? rawUsername.slice(1) : rawUsername;
    const {t} = useTranslation();
    const {data, isLoading, isFetching, isError, error} = useGetUserProfileQuery(username ?? "", {
        skip: !username,
    });

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

    const profile = data?.data;
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
            <div className="min-h-0 flex-1">
                <ProfileVideoGrid userId={profile.id} username={profile.username}/>
            </div>
        </div>
    );
};

export default ProfilePage;
