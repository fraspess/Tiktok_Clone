import {type ChangeEvent, useEffect, useMemo, useState} from "react";
import {useNavigate} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {Button} from "@/components/ui/button.tsx";
import {Input} from "@/components/ui/input.tsx";
import {Label} from "@/components/ui/label.tsx";
import {useChangeUsernameMutation, useUpdateUserMutation} from "@/store/apis/userApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import type {UserProfile} from "@/types/User.ts";

interface ProfileEditDialogProps {
    profile: UserProfile;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

const BIO_MAX_LENGTH = 160;

function extractErrorMessage(err: unknown): string | null {
    if (isFetchBaseQueryError(err) && typeof err.data === "object" && err.data && "message" in err.data) {
        const message = (err.data as { message?: string }).message;
        return message || null;
    }
    return null;
}

const ProfileEditDialog = ({profile, open, onOpenChange}: ProfileEditDialogProps) => {
    const {t} = useTranslation();
    const navigate = useNavigate();
    const [updateUser, {isLoading: isUpdatingProfile}] = useUpdateUserMutation();
    const [changeUsername, {isLoading: isChangingUsername}] = useChangeUsernameMutation();

    const [username, setUsername] = useState(profile.username);
    const [bio, setBio] = useState(profile.description ?? "");
    const [avatarFile, setAvatarFile] = useState<File | null>(null);
    const [formError, setFormError] = useState<string | null>(null);

    const isSubmitting = isUpdatingProfile || isChangingUsername;

    // Reset the form to the current profile every time the dialog is (re)opened.
    useEffect(() => {
        if (open) {
            setUsername(profile.username);
            setBio(profile.description ?? "");
            setAvatarFile(null);
            setFormError(null);
        }
    }, [open, profile.username, profile.description]);

    const avatarPreviewUrl = useMemo(() => {
        if (avatarFile) return URL.createObjectURL(avatarFile);
        return profile.avatar?.large ?? null;
    }, [avatarFile, profile.avatar]);

    useEffect(() => {
        return () => {
            if (avatarFile && avatarPreviewUrl) URL.revokeObjectURL(avatarPreviewUrl);
        };
    }, [avatarFile, avatarPreviewUrl]);

    const handleAvatarChange = (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) setAvatarFile(file);
    };

    const handleOpenChange = (nextOpen: boolean) => {
        if (!isSubmitting) onOpenChange(nextOpen);
    };

    const handleSubmit = async () => {
        const trimmedUsername = username.trim();
        const trimmedBio = bio.trim();

        if (!trimmedUsername) {
            setFormError(t("profile.edit.usernameRequired"));
            return;
        }
        if (trimmedBio.length > BIO_MAX_LENGTH) {
            setFormError(t("profile.edit.bioTooLong"));
            return;
        }
        setFormError(null);

        const usernameChanged = trimmedUsername !== profile.username;
        const bioChanged = trimmedBio !== (profile.description ?? "");
        const avatarChanged = avatarFile !== null;

        if (!usernameChanged && !bioChanged && !avatarChanged) {
            onOpenChange(false);
            return;
        }

        try {
            if (usernameChanged) {
                await changeUsername({
                    currentUsername: profile.username,
                    newUsername: trimmedUsername,
                }).unwrap();
            }

            if (bioChanged || avatarChanged) {
                const formData = new FormData();
                formData.append("Bio", trimmedBio);
                if (avatarFile) formData.append("Avatar", avatarFile);

                await updateUser({username: profile.username, formData}).unwrap();
            }

            toast.success(t("profile.edit.success"));
            onOpenChange(false);

            if (usernameChanged) {
                navigate(`/@${trimmedUsername}`, {replace: true});
            }
        } catch (err) {
            toast.error(extractErrorMessage(err) ?? t("profile.edit.error"));
        }
    };

    return (
        <Dialog open={open} onOpenChange={handleOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>{t("profile.edit.title")}</DialogTitle>
                    <DialogDescription>{t("profile.edit.description")}</DialogDescription>
                </DialogHeader>

                <div className="flex flex-col items-center gap-3">
                    <div className="h-24 w-24 overflow-hidden rounded-full bg-neutral-700">
                        {avatarPreviewUrl ? (
                            <img src={avatarPreviewUrl} alt={username} className="h-full w-full object-cover"/>
                        ) : (
                            <div className="flex h-full w-full items-center justify-center text-2xl font-semibold text-white">
                                {username[0]?.toUpperCase() ?? "?"}
                            </div>
                        )}
                    </div>
                    <label className="cursor-pointer text-sm font-medium text-primary hover:underline">
                        {t("profile.edit.changeAvatar")}
                        <input type="file" accept="image/*" className="hidden" onChange={handleAvatarChange}/>
                    </label>
                </div>

                <div className="flex flex-col gap-1.5">
                    <Label htmlFor="profile-edit-username">{t("profile.edit.usernameLabel")}</Label>
                    <Input
                        id="profile-edit-username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        maxLength={30}
                    />
                </div>

                <div className="flex flex-col gap-1.5">
                    <Label htmlFor="profile-edit-bio">{t("profile.edit.bioLabel")}</Label>
                    <textarea
                        id="profile-edit-bio"
                        value={bio}
                        onChange={(e) => setBio(e.target.value)}
                        rows={3}
                        maxLength={BIO_MAX_LENGTH}
                        className="w-full min-w-0 resize-none rounded-md border border-input bg-transparent px-2.5 py-1.5 text-sm shadow-xs outline-none placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 dark:bg-input/30"
                    />
                    <span className="self-end text-xs text-muted-foreground">
                        {bio.length}/{BIO_MAX_LENGTH}
                    </span>
                </div>

                {formError && <p className="text-sm text-destructive">{formError}</p>}

                <DialogFooter>
                    <Button variant="outline" onClick={() => handleOpenChange(false)} disabled={isSubmitting}>
                        {t("profile.edit.cancel")}
                    </Button>
                    <Button onClick={handleSubmit} disabled={isSubmitting}>
                        {t("profile.edit.save")}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};

export default ProfileEditDialog;
