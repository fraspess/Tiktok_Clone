import {MessageCircle, Search} from "lucide-react";
import {useCallback, useEffect, useRef, useState} from "react";
import {useLocation} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {Button} from "@/components/ui/button.tsx";
import {Input} from "@/components/ui/input.tsx";
import {cn} from "@/lib/utils.ts";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {useLazyGetMessagesQuery, useLazySearchConversationsQuery} from "@/store/apis/conversationApi.ts";
import type {ConversationDto} from "@/types/Conversation.ts";
import type {MessageDto} from "@/types/Message.ts";
import {useChatConnection} from "@/hooks/useChatConnection.ts";
import {useGetCurrentUserQuery} from "@/store/apis/authApi.ts";
import {getCachedMessages, saveCachedMessages} from "@/lib/chatMessagesCache.ts";
import {saveUserProfile} from "@/lib/userProfileCache.ts";
import ConversationList from "@/components/chat/ConversationList.tsx";
import ConversationWindow from "@/components/chat/ConversationWindow.tsx";

function mergeMessages(serverHistory: MessageDto[], currentMessages: MessageDto[], currentUserId?: string): MessageDto[] {
    const serverIds = new Set(serverHistory.map((m) => m.id));

    for (const m of serverHistory) {
        if (m.senderId && m.senderUsername) {
            saveUserProfile(m.senderId, m.senderUsername);
        }
    }

    const normalizedServer: MessageDto[] = serverHistory.map((m) => ({
        ...m,
        isOwn: currentUserId ? m.senderId === currentUserId : m.isOwn,
    }));

    const pendingOptimistic = currentMessages.filter((localMsg) => {
        if (serverIds.has(localMsg.id)) return false;
        if (localMsg.isOwn) {
            const existsInServer = normalizedServer.some(
                (sMsg) => sMsg.isOwn && sMsg.content.trim() === localMsg.content.trim()
            );
            if (existsInServer) return false;
        }
        return true;
    });

    const combined = [...normalizedServer, ...pendingOptimistic];
    combined.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());

    const finalResult: MessageDto[] = [];
    const seenIds = new Set<string>();
    for (const msg of combined) {
        if (!seenIds.has(msg.id)) {
            seenIds.add(msg.id);
            finalResult.push(msg);
        }
    }
    return finalResult;
}

const MessagesPage = () => {
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const location = useLocation();
    const isAuth = useAppSelector((state) => state.auth.isAuth);
    const accessToken = useAppSelector((state) => state.auth.accessToken);
    const [selectedConversation, setSelectedConversation] = useState<ConversationDto | null>(null);
    const [messages, setMessages] = useState<MessageDto[]>([]);
    const [messagesError, setMessagesError] = useState<string | null>(null);
    const [isMessagesLoading, setIsMessagesLoading] = useState(false);
    const [isNewConversationOpen, setIsNewConversationOpen] = useState(false);
    const [searchQuery, setSearchQuery] = useState("");
    const [newConversation, setNewConversation] = useState<ConversationDto | null>(null);
    const [getMessages] = useLazyGetMessagesQuery();
    const [searchConversations] = useLazySearchConversationsQuery();
    const {data: currentUserResponse} = useGetCurrentUserQuery(undefined, {skip: !isAuth});
    const currentUser = currentUserResponse?.data;

    useEffect(() => {
        if (currentUser?.id && currentUser?.username) {
            saveUserProfile(currentUser.id, currentUser.username);
        }
    }, [currentUser]);

    const selectedConvRef = useRef<ConversationDto | null>(null);
    useEffect(() => {
        selectedConvRef.current = selectedConversation;
    }, [selectedConversation]);

    const loadMessages = useCallback(async (conversation: ConversationDto, silent = false) => {
        if (!silent) setIsMessagesLoading(true);
        setMessagesError(null);
        try {
            const response = await getMessages({conversationId: conversation.id, pageNumber: 1, pageSize: 50}).unwrap();
            const history = [...response.data.items].reverse();
            setMessages((current) => {
                const merged = mergeMessages(history, current, currentUser?.id);
                saveCachedMessages(conversation.id, merged);
                return merged;
            });
        } catch {
            const cached = getCachedMessages(conversation.id);
            setMessages(cached);
            if (!silent) {
                setMessagesError(cached.length > 0 ? null : t("chat.loadMessagesError"));
            }
        } finally {
            if (!silent) setIsMessagesLoading(false);
        }
    }, [getMessages, currentUser?.id, t]);

    const {isConnected, sendMessage} = useChatConnection({
        accessToken,
        onMessagesReceived: (data) => {
            const activeConv = selectedConvRef.current;
            if (!activeConv || !data) return;

            const incomingList = Array.isArray(data) ? data : [data];
            const relevant = incomingList.filter(
                (m) => !m.conversationId || m.conversationId === activeConv.id
            );
            if (relevant.length > 0) {
                setMessages((current) => {
                    const merged = mergeMessages(relevant, current, currentUser?.id);
                    saveCachedMessages(activeConv.id, merged);
                    return merged;
                });
            }
        },
    });

    const handleSelectConversation = useCallback((conversation: ConversationDto) => {
        setSelectedConversation(conversation);
        setMessages([]);
        setMessagesError(null);
        setSearchQuery("");
        setIsNewConversationOpen(false);
        void loadMessages(conversation);
    }, [loadMessages]);

    const handleSend = async (content: string) => {
        if (!selectedConversation) return;
        const tempId = `temp-${crypto.randomUUID()}`;
        const optimisticMessage: MessageDto = {
            id: tempId,
            conversationId: selectedConversation.id,
            senderId: currentUser?.id ?? "",
            senderUsername: currentUser?.username ?? "",
            senderAvatarUrl: "",
            content,
            createdAt: new Date().toISOString(),
            isOwn: true,
        };
        setMessages((current) => {
            const updated = [...current, optimisticMessage];
            saveCachedMessages(selectedConversation.id, updated);
            return updated;
        });

        try {
            await sendMessage(selectedConversation.id, content);
        } catch (err) {
            console.warn("Failed to send message via SignalR", err);
        }
    };

    const handleSearch = useCallback(async (query: string) => {
        setSearchQuery(query);
        if (!query.trim()) return;
        try {
            await searchConversations({query: query.trim(), pageNumber: 1, pageSize: 20}).unwrap();
        } catch {
        }
    }, [searchConversations]);

    useEffect(() => {
        const conv = (location.state as { conversation?: ConversationDto } | null)?.conversation;
        if (conv) {
            setNewConversation(conv);
            handleSelectConversation(conv);
        }
    }, []);

    if (!isAuth) {
        return (
            <div className="flex h-full flex-col items-center justify-center gap-4 px-6 text-center">
                <div className="flex h-16 w-16 items-center justify-center rounded-full bg-neutral-100 dark:bg-neutral-800">
                    <MessageCircle className="h-8 w-8 text-muted-foreground"/>
                </div>
                <div className="space-y-1">
                    <p className="text-lg font-semibold">{t("chat.inbox")}</p>
                    <p className="text-sm text-muted-foreground">{t("chat.signInPrompt")}</p>
                </div>
                <Button onClick={() => dispatch(openModal())} className="min-w-32">
                    {t("auth.signInTitle")}
                </Button>
            </div>
        );
    }

    return (
        <div className="flex h-full min-h-0 w-full bg-[#121212] text-white">
            <div className={cn(
                "w-full shrink-0 flex-col border-r border-white/10 md:flex md:max-w-[420px]",
                selectedConversation ? "hidden" : "flex"
            )}>
                <header className="flex shrink-0 items-center justify-between border-b border-white/10 px-5 py-5">
                    <h1 className="text-[24px] font-bold tracking-[-0.03em]">{t("chat.inbox")}</h1>
                    <Button size="icon-sm" variant="ghost" onClick={() => setIsNewConversationOpen(true)}
                            aria-label={t("chat.newConversation")}>
                        <Search className="h-5 w-5"/>
                    </Button>
                </header>

                {isNewConversationOpen && (
                    <div className="border-b border-white/10 px-4 py-3">
                        <Input
                            autoFocus
                            value={searchQuery}
                            onChange={(e) => void handleSearch(e.target.value)}
                            placeholder={t("chat.searchUsers")}
                            className="border-white/10 bg-white/5 text-white placeholder:text-white/40"
                        />
                    </div>
                )}

                <ConversationList
                    selectedConversationId={selectedConversation?.id ?? null}
                    onSelect={handleSelectConversation}
                    currentUser={currentUser}
                    searchQuery={isNewConversationOpen ? searchQuery : ""}
                    newConversation={newConversation}
                />
            </div>
            {selectedConversation ? (
                <ConversationWindow
                    conversation={selectedConversation}
                    messages={messages}
                    isLoading={isMessagesLoading}
                    error={messagesError}
                    isConnected={isConnected}
                    onSend={handleSend}
                    onBack={() => setSelectedConversation(null)}
                    currentUser={currentUser}
                />
            ) : (
                <div className="hidden flex-1 items-center justify-center px-6 text-center text-sm text-white/60 md:flex">
                    {t("chat.selectConversation")}
                </div>
            )}
        </div>
    );
};

export default MessagesPage;