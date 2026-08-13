import {useEffect, useMemo, useState} from "react";
import {Loader2, Search, UserPlus} from "lucide-react";
import {useTranslation} from "react-i18next";
import {toast} from "sonner";
import {Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {Input} from "@/components/ui/input.tsx";
import UserAvatar from "@/components/chat/UserAvatar.tsx";
import {useCreateConversationMutation, useLazyGetChatUsersQuery, type ChatUserDto} from "@/store/apis/conversationApi.ts";
import type {ConversationDto} from "@/types/Conversation.ts";
import {enrichConversationWithUser} from "@/lib/conversationHelpers.ts";
import {saveUserProfile} from "@/lib/userProfileCache.ts";

interface NewConversationDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    currentUserId?: string;
    onConversationCreated: (conversation: ConversationDto) => void;
}

const NewConversationDialog = ({open, onOpenChange, currentUserId, onConversationCreated}: NewConversationDialogProps) => {
    const {t} = useTranslation();
    const [search, setSearch] = useState("");
    const [getUsers, {data, isFetching, isError}] = useLazyGetChatUsersQuery();
    const [createConversation, {isLoading: isCreating}] = useCreateConversationMutation();

    useEffect(() => {
        if (open) void getUsers({pageNumber: 1, pageSize: 100});
    }, [open, getUsers]);

    const users = useMemo(() => {
        const term = search.trim().toLowerCase();
        return (data?.data.items ?? []).filter((user) =>
            user.id !== currentUserId && (!term || user.username.toLowerCase().includes(term))
        );
    }, [data, currentUserId, search]);

    const startConversation = async (user: ChatUserDto) => {
        try {
            saveUserProfile(user.id, user.username, typeof user.avatar === "string" ? user.avatar : user.avatar?.small);
            const response = await createConversation({userIds: [user.id]}).unwrap();
            const enriched = enrichConversationWithUser(response.data, {
                id: user.id,
                username: user.username,
                avatar: typeof user.avatar === "string" ? user.avatar : user.avatar?.small ?? null,
            });
            onConversationCreated(enriched);
            onOpenChange(false);
        } catch {
            toast.error(t("chat.createError"));
        }
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="max-h-[80vh] overflow-hidden bg-[#1c1c1c] text-white sm:max-w-lg">
                <DialogHeader>
                    <DialogTitle>{t("chat.newConversation")}</DialogTitle>
                    <DialogDescription className="text-white/60">{t("chat.newConversationDescription")}</DialogDescription>
                </DialogHeader>
                <div className="relative">
                    <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-white/45"/>
                    <Input value={search} onChange={(event) => setSearch(event.target.value)} placeholder={t("chat.searchUsers")} className="border-white/15 bg-white/5 pl-9 text-white placeholder:text-white/40"/>
                </div>
                <div className="max-h-[48vh] overflow-y-auto">
                    {isFetching ? <div className="flex justify-center py-8"><Loader2 className="h-5 w-5 animate-spin"/></div> : isError ? (
                        <p className="py-8 text-center text-sm text-white/60">{t("chat.usersLoadError")}</p>
                    ) : users.length === 0 ? (
                        <p className="py-8 text-center text-sm text-white/60">{t("chat.noUsersFound")}</p>
                    ) : users.map((user) => (
                        <button key={user.id} type="button" onClick={() => void startConversation(user)} disabled={isCreating} className="flex w-full items-center gap-3 rounded-lg px-2 py-3 text-left transition hover:bg-white/10 disabled:opacity-50">
                            <UserAvatar username={user.username} avatar={user.avatar as unknown as Parameters<typeof UserAvatar>[0]["avatar"]}/>
                            <span className="min-w-0 flex-1 truncate font-medium">@{user.username}</span>
                            {isCreating ? <Loader2 className="h-4 w-4 animate-spin"/> : <UserPlus className="h-4 w-4 text-white/60"/>}
                        </button>
                    ))}
                </div>
            </DialogContent>
        </Dialog>
    );
};

export default NewConversationDialog;
