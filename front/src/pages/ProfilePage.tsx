import {useParams} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Loader2} from "lucide-react";
import {useGetUserProfileQuery} from "@/store/apis/userApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import ProfileHeader from "@/components/profile/ProfileHeader.tsx";
import ProfileVideoGrid from "@/components/profile/ProfileVideoGrid.tsx";

const ProfilePage = () => {
    const {username} = useParams<{ username: string }>();
    const {t} = useTranslation();
    const {data, isLoading, isFetching, isError, error} = useGetUserProfileQuery(username ?? "", {
        skip: !username,
    });

    if (!username || (isLoading && !data)) {
        return (
            <div className="flex h-full w-full items-center justify-center">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground"/>
            </div>
        );
    }

    if (isError) {
        const isNotFound = isFetchBaseQueryError(error) && error.status === 404;
        return (
            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                {isNotFound ? t("profile.notFound") : t("profile.loadError")}
            </div>
        );
    }

    const profile = data?.data;
    if (!profile) {
        return (
            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                {isFetching ? <Loader2 className="h-8 w-8 animate-spin"/> : t("profile.notFound")}
            </div>
        );
    }

    return (
        <div className="flex h-full w-full flex-col overflow-hidden">
            <ProfileHeader profile={profile}/>
            <div className="min-h-0 flex-1">
                <ProfileVideoGrid userId={profile.id}/>
            </div>
        </div>
    );
};

export default ProfilePage;
