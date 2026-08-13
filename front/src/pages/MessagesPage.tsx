import {MessageCircle, Plus} from "lucide-react";
import {useCallback, useEffect, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import {Button} from "@/components/ui/button.tsx";
import ConversationList from "@/components/chat/ConversationList.tsx";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";
import {useLazyGetMessagesQuery} from "@/store/apis/conversationApi.ts";
import type {ConversationDto} from "@/types/Conversation.ts";
import type {MessageDto} from "@/types/Message.ts";
import ConversationWindow from "@/components/chat/ConversationWindow.tsx";
import {useChatConnection} from "@/hooks/useChatConnection.ts";
import {useGetCurrentUserQuery} from "@/store/apis/authApi.ts";
import {getCachedMessages, saveCachedMessages} from "@/lib/chatMessagesCache.ts";
import {saveUserProfile} from "@/lib/userProfileCache.ts";
import NewConversationDialog from "@/components/chat/NewConversationDialog.tsx";

function mergeMessages(serverHistory: MessageDto[], currentMessages: MessageDto[], currentUserId?: string): MessageDto[] {
    const serverIds = new Set(serverHistory.map((m) => m.id));

    // Cache user profiles for all sender usernames in history
    for (const m of serverHistory) {
        if (m.senderId && m.senderUsername) {
            saveUserProfile(m.senderId, m.senderUsername);
        }
    }

    // Normalize server messages with correct isOwn calculation
    const normalizedServer: MessageDto[] = serverHistory.map((m) => ({
        ...m,
        isOwn: currentUserId ? m.senderId === currentUserId : m.isOwn,
    }));

    // Retain only local optimistic messages that have NOT been returned by server history yet
    const pendingOptimistic = currentMessages.filter((localMsg) => {
        // If server already returned exact message ID, drop local copy
        if (serverIds.has(localMsg.id)) return false;

        // If this is an optimistic message sent by current user
        if (localMsg.isOwn) {
            // Check if server history contains a message with matching content sent by current user
            const existsInServer = normalizedServer.some(
                (sMsg) => sMsg.isOwn && sMsg.content.trim() === localMsg.content.trim()
            );
            if (existsInServer) {
                return false; // Server has matched this message! Drop local optimistic duplicate!
            }
        }
        return true;
    });

    const combined = [...normalizedServer, ...pendingOptimistic];

    // Sort chronologically by createdAt (ascending)
    combined.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());

    // Deduplicate by message ID just in case
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
    const isAuth = useAppSelector((state) => state.auth.isAuth);
    const accessToken = useAppSelector((state) => state.auth.accessToken);
    const [selectedConversation, setSelectedConversation] = useState<ConversationDto | null>(null);
    const [messages, setMessages] = useState<MessageDto[]>([]);
    const [messagesError, setMessagesError] = useState<string | null>(null);
    const [isMessagesLoading, setIsMessagesLoading] = useState(false);
    const [isNewConversationOpen, setIsNewConversationOpen] = useState(false);
    const [getMessages] = useLazyGetMessagesQuery();
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
        if (!silent) {
            setIsMessagesLoading(true);
        }
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
            if (!silent) {
                setIsMessagesLoading(false);
            }
        }
    }, [getMessages, currentUser?.id, t]);

    const {isConnected, sendMessage} = useChatConnection({
        accessToken,
        onMessagesReceived: (data) => {
            const activeConv = selectedConvRef.current;
            if (!activeConv) return;
            if (data) {
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
            }
            void loadMessages(activeConv, true);
        },
    });

    // Fast polling every 1.5 seconds & tab focus handler for active conversation
    useEffect(() => {
        if (!selectedConversation) return;

        // Immediate fetch on selection or focus
        void loadMessages(selectedConversation, true);

        const interval = setInterval(() => {
            if (selectedConvRef.current) {
                void loadMessages(selectedConvRef.current, true);
            }
        }, 1500);

        const handleFocus = () => {
            if (selectedConvRef.current) {
                void loadMessages(selectedConvRef.current, true);
            }
        };

        window.addEventListener("focus", handleFocus);

        return () => {
            clearInterval(interval);
            window.removeEventListener("focus", handleFocus);
        };
    }, [selectedConversation, loadMessages]);

    const handleSelectConversation = (conversation: ConversationDto) => {
        setSelectedConversation(conversation);
        setMessages([]);
        setMessagesError(null);
        void loadMessages(conversation);
    };

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
            const updatedMessages = [...current, optimisticMessage];
            saveCachedMessages(selectedConversation.id, updatedMessages);
            return updatedMessages;
        });

        try {
            await sendMessage(selectedConversation.id, content);
        } catch (err) {
            console.warn("Failed to send message via SignalR", err);
        } finally {
            setTimeout(() => {
                if (selectedConvRef.current) {
                    void loadMessages(selectedConvRef.current, true);
                }
            }, 400);
        }
    };

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
            <div className="flex w-full max-w-[420px] shrink-0 flex-col border-r border-white/10">
            <header className="flex shrink-0 items-center justify-between border-b border-white/10 px-5 py-5">
                <h1 className="text-[24px] font-bold tracking-[-0.03em]">{t("chat.inbox")}</h1>
                <Button size="icon-sm" variant="ghost" onClick={() => setIsNewConversationOpen(true)} aria-label={t("chat.newConversation")}>
                    <Plus className="h-5 w-5"/>
                </Button>
            </header>
            <ConversationList selectedConversationId={selectedConversation?.id ?? null} onSelect={handleSelectConversation} currentUser={currentUser}/>
            </div>
            {selectedConversation ? (
                <ConversationWindow
                    conversation={selectedConversation}
                    messages={messages}
                    isLoading={isMessagesLoading}
                    error={messagesError}
                    isConnected={isConnected}
                    onSend={handleSend}
                    currentUser={currentUser}
                />
            ) : (
                <div className="flex flex-1 items-center justify-center px-6 text-center text-sm text-white/60">
                    {t("chat.selectConversation")}
                </div>
            )}
            <NewConversationDialog
                open={isNewConversationOpen}
                onOpenChange={setIsNewConversationOpen}
                currentUserId={currentUser?.id}
                onConversationCreated={handleSelectConversation}
            />
        </div>
    );
};

export default MessagesPage;
