import {MessageCircle, Search, Settings} from "lucide-react";
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
import MessagePrivacyDialog from "@/components/chat/MessagePrivacyDialog.tsx";
import MessagesNotAcceptedDialog from "@/components/chat/MessagesNotAcceptedDialog.tsx";
import {isMessagePrivacyError} from "@/lib/messagePrivacy.ts";
import {toast} from "sonner";

function mergeMessages(serverHistory: MessageDto[], currentMessages: MessageDto[], currentUserId?: string): MessageDto[] {
    const serverIds = new Set(serverHistory.map((m) => m.id));
    const currentById = new Map(currentMessages.map(m => [m.id, m]));

    for (const m of serverHistory) {
        if (m.senderId && m.senderUsername) {
            saveUserProfile(m.senderId, m.senderUsername);
        }
    }

    const normalizedServer: MessageDto[] = serverHistory.map((m) => ({
        ...m,
        isOwn: currentUserId ? m.senderId === currentUserId : m.isOwn,
        isRead: m.isRead || currentById.get(m.id)?.isRead,
        isDelivered: m.isDelivered || currentById.get(m.id)?.isDelivered,
    }));

    const pendingOptimistic = currentMessages.filter((localMsg) => {
        if (serverIds.has(localMsg.id)) return false;
        if (localMsg.id.startsWith("temp-") && localMsg.isOwn) {
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
    const [historyPage, setHistoryPage] = useState(1);
    const [hasOlder, setHasOlder] = useState(false);
    const [isLoadingOlder, setIsLoadingOlder] = useState(false);
    const olderRequest = useRef(false);
    const receiptPending = useRef(new Set<string>());
    const deliveryPending = useRef(new Set<string>());
    const [deliveryQueue, setDeliveryQueue] = useState<MessageDto[]>([]);
    const [isMessagesLoading, setIsMessagesLoading] = useState(false);
    const [isNewConversationOpen, setIsNewConversationOpen] = useState(false);
    const [isPrivacyOpen, setIsPrivacyOpen] = useState(false);
    const [isMessagesNotAcceptedOpen, setIsMessagesNotAcceptedOpen] = useState(false);
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
            if (selectedConvRef.current?.id !== conversation.id) return;
            setHistoryPage(1);
            setHasOlder(response.data.metadata.hasNext);
            const history = [...response.data.items].reverse();
            setMessages((current) => {
                const merged = mergeMessages(history, current, currentUser?.id);
                saveCachedMessages(conversation.id, merged);
                return merged;
            });
        } catch {
            if (selectedConvRef.current?.id !== conversation.id) return;
            const cached = getCachedMessages(conversation.id);
            setMessages(cached);
            if (!silent) {
                setMessagesError(cached.length > 0 ? null : t("chat.loadMessagesError"));
            }
        } finally {
            if (!silent && selectedConvRef.current?.id === conversation.id) setIsMessagesLoading(false);
        }
    }, [getMessages, currentUser?.id, t]);

    const loadOlder = async () => {
        const conversation = selectedConvRef.current;
        if (!conversation || olderRequest.current || !hasOlder) return;
        olderRequest.current = true;
        setIsLoadingOlder(true);
        try {
            const response = await getMessages({
                conversationId: conversation.id,
                pageNumber: historyPage + 1,
                pageSize: 50
            }).unwrap();
            if (selectedConvRef.current?.id !== conversation.id) return;
            setMessages(current => {
                const merged = mergeMessages(response.data.items, current, currentUser?.id);
                saveCachedMessages(conversation.id, merged);
                return merged;
            });
            setHistoryPage(p => p + 1);
            setHasOlder(response.data.metadata.hasNext);
        } catch {
            if (selectedConvRef.current?.id === conversation.id) toast.error(t("chat.loadMessagesError"));
        } finally {
            olderRequest.current = false;
            setIsLoadingOlder(false);
        }
    };

    const {isConnected, sendMessage, markAsRead, markAsDelivered} = useChatConnection({
        accessToken,
        onReceipt: receipt => {
            const apply = (items: MessageDto[]) => items.map(m => m.id === receipt.messageId ?
                {...m, isDelivered: m.isDelivered || receipt.isDelivered, isRead: m.isRead || receipt.isRead} : m);
            saveCachedMessages(receipt.conversationId, apply(getCachedMessages(receipt.conversationId)));
            if (selectedConvRef.current?.id === receipt.conversationId) setMessages(apply);
        },
        onMessagesReceived: (data) => {
            const activeConv = selectedConvRef.current;
            if (!data) return;

            const incomingList = Array.isArray(data) ? data : [data];
            // Pending messages may arrive before the socket or current-user query is ready.
            setDeliveryQueue(current => [...new Map([...current, ...incomingList]
                .filter(message => !message.isDelivered)
                .map(message => [message.id, message])).values()]);
            if (!activeConv) return;
            const relevant = incomingList.filter(
                (m) => m.conversationId === activeConv.id
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

    useEffect(() => {
        if (!isConnected || !currentUser?.id) return;
        for (const message of deliveryQueue) {
            if (deliveryPending.current.has(message.id)) continue;
            deliveryPending.current.add(message.id);
            const acknowledgement = message.senderId === currentUser.id
                ? Promise.resolve(true) : markAsDelivered(message.id);
            void acknowledgement.then(acknowledged => {
                if (acknowledged) setDeliveryQueue(current => current.filter(m => m.id !== message.id));
            }).catch(() => undefined).finally(() => deliveryPending.current.delete(message.id));
        }
    }, [deliveryQueue, currentUser?.id, isConnected, markAsDelivered]);

    useEffect(() => {
        if (!isConnected || !currentUser?.id) return;
        const acknowledge = () => {
            if (document.visibilityState !== "visible" || !document.hasFocus()) return;
            const conversationId = selectedConvRef.current?.id;
            for (const message of messages) {
                if (message.senderId === currentUser.id || message.isRead || message.id.startsWith("temp-") || receiptPending.current.has(message.id)) continue;
                receiptPending.current.add(message.id);
                void markAsRead(message.id).then(acknowledged => {
                    if (acknowledged && selectedConvRef.current?.id === conversationId) {
                        setMessages(current => current.map(m => m.id === message.id ? {
                            ...m,
                            isRead: true,
                            isDelivered: true
                        } : m));
                    }
                }).catch(() => undefined).finally(() => receiptPending.current.delete(message.id));
            }
        };
        acknowledge();
        window.addEventListener("focus", acknowledge);
        document.addEventListener("visibilitychange", acknowledge);
        return () => {
            window.removeEventListener("focus", acknowledge);
            document.removeEventListener("visibilitychange", acknowledge);
        };
    }, [messages, currentUser?.id, isConnected, markAsRead]);

    const handleSelectConversation = useCallback((conversation: ConversationDto) => {
        selectedConvRef.current = conversation;
        setSelectedConversation(conversation);
        setMessages([]);
        setHistoryPage(1);
        setHasOlder(false);
        setMessagesError(null);
        setSearchQuery("");
        setIsNewConversationOpen(false);
        void loadMessages(conversation);
    }, [loadMessages]);

    const handleSend = async (content: string) => {
        if (!selectedConversation) return false;
        const conversationId = selectedConversation.id;
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
            await sendMessage(conversationId, content);
            return true;
        } catch (err) {
            // Failed optimistic messages must disappear from both the UI and its cache.
            saveCachedMessages(conversationId, getCachedMessages(conversationId).filter((m) => m.id !== tempId));
            if (selectedConvRef.current?.id === conversationId) {
                setMessages((current) => current.filter((m) => m.id !== tempId));
            }
            if (isMessagePrivacyError(err)) {
                setIsMessagesNotAcceptedOpen(true);
            } else {
                toast.error(t("chat.sendError"));
            }
            return false;
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
            // eslint-disable-next-line react-hooks/set-state-in-effect
            setNewConversation(conv);
            handleSelectConversation(conv);
        }
    }, []);

    if (!isAuth) {
        return (
            <div className="flex h-full flex-col items-center justify-center gap-4 px-6 text-center">
                <div
                    className="flex h-16 w-16 items-center justify-center rounded-full bg-neutral-100 dark:bg-neutral-800">
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
        <div className="flex h-full min-h-0 w-full bg-background text-foreground">
            <div className={cn(
                "w-full shrink-0 flex-col border-r bg-card md:flex md:max-w-[420px]",
                selectedConversation ? "hidden" : "flex"
            )}>
                <header className="flex shrink-0 items-center justify-between border-b px-5 py-5">
                    <h1 className="text-[24px] font-bold tracking-[-0.03em]">{t("chat.inbox")}</h1>
                    <div className="flex items-center gap-1 ">
                        <Button size="icon-sm" variant="ghost" onClick={() => setIsPrivacyOpen(true)}
                                aria-label={t("chat.privacy.trigger")}>
                            <Settings className="h-5 w-5"/>
                        </Button>
                        <Button size="icon-sm" variant="ghost" onClick={() => setIsNewConversationOpen(true)}
                                aria-label={t("chat.newConversation")}>
                            <Search className="h-5 w-5"/>
                        </Button>
                    </div>
                </header>

                {isNewConversationOpen && (
                    <div className="border-b px-4 py-3">
                        <Input
                            autoFocus
                            value={searchQuery}
                            onChange={(e) => void handleSearch(e.target.value)}
                            placeholder={t("chat.searchUsers")}
                            className="bg-background"
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
                    key={selectedConversation.id}
                    conversation={selectedConversation}
                    messages={messages.map(message => ({
                        ...message,
                        isOwn: currentUser ? message.senderId === currentUser.id : message.isOwn
                    }))}
                    isLoading={isMessagesLoading}
                    hasOlder={hasOlder}
                    isLoadingOlder={isLoadingOlder}
                    onLoadOlder={loadOlder}
                    error={messagesError}
                    isConnected={isConnected}
                    onSend={handleSend}
                    onBack={() => {
                        selectedConvRef.current = null;
                        setSelectedConversation(null);
                    }}
                    currentUser={currentUser}
                />
            ) : (
                <div
                    className="hidden flex-1 items-center justify-center px-6 text-center text-sm text-muted-foreground md:flex">
                    {t("chat.selectConversation")}
                </div>
            )}
            <MessagePrivacyDialog open={isPrivacyOpen} onOpenChange={setIsPrivacyOpen}/>
            <MessagesNotAcceptedDialog open={isMessagesNotAcceptedOpen} onOpenChange={setIsMessagesNotAcceptedOpen}/>
        </div>
    );
};

export default MessagesPage;
