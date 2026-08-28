/* import {useState} from "react";
import {useTranslation} from "react-i18next";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "@/components/ui/tabs.tsx";
import ProfileVideoGrid from "@/components/profile/ProfileVideoGrid.tsx";
import ProfileFavoriteVideoGrid from "@/components/profile/ProfileFavoriteVideoGrid.tsx";

interface ProfileVideoTabsProps {
    userId: string;
    username: string;
    isOwnProfile: boolean;
}

const ProfileVideoTabs = ({userId, username, isOwnProfile}: ProfileVideoTabsProps) => {
    const {t} = useTranslation();
    const [tab, setTab] = useState("uploaded");

    return (
        <Tabs value={tab} onValueChange={setTab} className="flex h-full min-h-0 w-full flex-col">
            <TabsList className="px-4">
                <TabsTrigger value="uploaded">{t("profile.tabs.uploaded")}</TabsTrigger>
                {isOwnProfile && <TabsTrigger value="saved">{t("profile.tabs.saved")}</TabsTrigger>}
            </TabsList>

            <TabsContent value="uploaded" className="min-h-0 flex-1">
                <ProfileVideoGrid userId={userId} username={username}/>
            </TabsContent>

            {isOwnProfile && (
                <TabsContent value="saved" className="min-h-0 flex-1">
                    <ProfileFavoriteVideoGrid enabled={tab === "saved"}/>
                </TabsContent>
            )}
        </Tabs>
    );
};

export default ProfileVideoTabs; */