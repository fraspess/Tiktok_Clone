import {Link} from "react-router-dom";
import type {FormEvent} from "react";
import {useCallback, useLayoutEffect, useRef, useState} from "react";
import {ArrowLeft, Loader2, Send} from "lucide-react";
import {useTranslation} from "react-i18next";
import {Button} from "@/components/ui/button.tsx";
import {Input} from "@/components/ui/input.tsx";
import {cn} from "@/lib/utils.ts";
import type {ConversationDto} from "@/types/Conversation.ts";
import type {MessageDto} from "@/types/Message.ts";

interface ConversationWindowProps {
    conversation: ConversationDto;
    messages: MessageDto[];
    isLoading: boolean;
    hasOlder?: boolean;
    isLoadingOlder?: boolean;
    onLoadOlder?: () => Promise<void>;
    error: string | null;
    isConnected: boolean;
    onSend: (content: string) => Promise<boolean>;
    onBack?: () => void;
    currentUser?: {id: string; username: string};
}

const getOtherParticipants = (
    conversation: ConversationDto,
    currentUser?: {id: string; username: string}
) => conversation.participants.filter((p) => p.id !== currentUser?.id && p.username);

const ConversationWindow = ({
                                conversation,
                                messages,
                                isLoading,
                                hasOlder,
                                isLoadingOlder,
                                onLoadOlder,
                                error,
                                isConnected,
                                onSend,
                                onBack,
                                currentUser,
                            }: ConversationWindowProps) => {
    const {t} = useTranslation();
    const [content, setContent] = useState("");
    const [isSending, setIsSending] = useState(false);
    const scrollRef = useRef<HTMLDivElement>(null);
    const scrollSnapshot = useRef<{height: number; top: number} | null>(null);
    const previousLast = useRef<string | undefined>(undefined);
    const loadOlder = async () => {
        const el = scrollRef.current;
        if (!el || !onLoadOlder) return;
        scrollSnapshot.current = {height: el.scrollHeight, top: el.scrollTop};
        await onLoadOlder();
    };
    const bottomRef = useRef<HTMLDivElement>(null);
    const inputRef = useRef<HTMLInputElement>(null);

    const otherParticipants = getOtherParticipants(conversation, currentUser);

    useLayoutEffect(() => {
        const el = scrollRef.current;
        const snapshot = scrollSnapshot.current;
        const last = messages.at(-1)?.id;
        if (el && snapshot) {
            el.scrollTop = snapshot.top + el.scrollHeight - snapshot.height;
            if (!isLoadingOlder) scrollSnapshot.current = null;
        } else if (last !== previousLast.current) {
            bottomRef.current?.scrollIntoView({block: "end"});
        }
        previousLast.current = last;
    }, [messages, isLoadingOlder]);

    const submit = useCallback(async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const message = content.trim();
        if (!message || isSending) return;

        setIsSending(true);
        try {
            if (await onSend(message)) {
                setContent("");
                requestAnimationFrame(() => inputRef.current?.focus());
            }
        } finally {
            setIsSending(false);
        }
    }, [content, isSending, onSend]);

    return (
        <section className="flex min-w-0 flex-1 flex-col bg-background">
            <header className="flex items-center gap-2 border-b px-3 py-3 md:px-5 md:py-4">
                {onBack && (
                    <Button type="button" variant="ghost" size="icon-sm" onClick={onBack}
                            className="shrink-0 md:hidden" aria-label={t("chat.back")}>
                        <ArrowLeft className="h-5 w-5"/>
                    </Button>
                )}
                <h2 className="truncate text-base font-semibold">
                    {otherParticipants.length > 0 ? (
                        otherParticipants.map((p, idx) => (
                            <span key={p.id}>
                                {idx > 0 && ", "}
                                <Link to={`/@${p.username}`} className="hover:underline">
                                    @{p.username}
                                </Link>
                            </span>
                        ))
                    ) : (
                        t("chat.unnamedConversation")
                    )}
                </h2>
            </header>

            <div ref={scrollRef} className="flex-1 overflow-y-auto px-3 py-4 md:px-5">
                {hasOlder && <Button variant="ghost" disabled={isLoadingOlder} onClick={() => void loadOlder()}>
                    {t(isLoadingOlder ? "chat.loadingMessages" : "chat.loadOlder")}
                </Button>}
                {isLoading ? (
                    <div className="flex h-full items-center justify-center text-muted-foreground">
                        <Loader2 className="mr-2 h-5 w-5 animate-spin"/>{t("chat.loadingMessages")}
                    </div>
                ) : error ? (
                    <div className="flex h-full items-center justify-center text-center text-sm text-muted-foreground">
                        {error}
                    </div>
                ) : messages.length === 0 ? (
                    <div className="flex h-full items-center justify-center text-center text-sm text-muted-foreground">
                        {t("chat.noMessages")}
                    </div>
                ) : (
                    <div className="space-y-3">
                        {messages.map((message) => (
                            <div key={message.id} className={cn("flex", message.isOwn ? "justify-end" : "justify-start")}>
                                <div className={cn(
                                    "max-w-[80%] rounded-2xl px-3 py-2 text-sm",
                                    message.isOwn ? "rounded-br-md bg-primary text-primary-foreground" : "rounded-bl-md bg-muted text-foreground"
                                )}>
                                    {!message.isOwn && (
                                        <Link
                                            to={`/@${message.senderUsername}`}
                                            className="mb-1 block text-xs font-medium text-muted-foreground hover:underline"
                                        >
                                            {message.senderUsername}
                                        </Link>
                                    )}
                                    <p className="whitespace-pre-wrap break-words">{message.content}</p>
                                    {message.isOwn && <p className="mt-1 text-right text-xs opacity-70">{t(message.id.startsWith("temp-") ? "chat.sending" : message.isRead ? "chat.read" : message.isDelivered ? "chat.delivered" : "chat.sent")}</p>}
                                </div>
                            </div>
                        ))}
                        <div ref={bottomRef}/>
                    </div>
                )}
            </div>

            <form onSubmit={submit} className="flex gap-2 border-t p-3 md:p-4">
                <Input
                    ref={inputRef}
                    value={content}
                    onChange={(e) => setContent(e.target.value)}
                    placeholder={t("chat.messagePlaceholder")}
                    maxLength={255}
                    disabled={!isConnected || isSending}
                    className="bg-background"
                />
                <Button type="submit" size="icon" disabled={!content.trim() || !isConnected || isSending}
                        aria-label={t("chat.send")}>
                    {isSending ? <Loader2 className="h-4 w-4 animate-spin"/> : <Send className="h-4 w-4"/>}
                </Button>
            </form>
            {!isConnected && <p className="px-4 pb-3 text-xs text-muted-foreground">{t("chat.connecting")}</p>}
        </section>
    );
};

export default ConversationWindow;
