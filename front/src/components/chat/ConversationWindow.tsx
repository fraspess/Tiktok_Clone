import type {FormEvent} from "react";
import {useEffect, useRef, useState} from "react";
import {Loader2, Send} from "lucide-react";
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
    error: string | null;
    isConnected: boolean;
    onSend: (content: string) => Promise<void>;
    currentUser?: { id: string; username: string };
}

const getConversationName = (conversation: ConversationDto, fallback: string, currentUser?: {
    id: string;
    username: string
}) =>
    conversation.participants
        .filter((participant) => participant.id !== currentUser?.id)
        .map((participant) => participant.username)
        .filter(Boolean)
        .map((username) => `@${username}`)
        .join(", ") || fallback;

const ConversationWindow = ({
                                conversation,
                                messages,
                                isLoading,
                                error,
                                isConnected,
                                onSend,
                                currentUser
                            }: ConversationWindowProps) => {
    const {t} = useTranslation();
    const [content, setContent] = useState("");
    const [isSending, setIsSending] = useState(false);
    const bottomRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        bottomRef.current?.scrollIntoView({block: "end"});
    }, [messages]);

    const submit = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const message = content.trim();
        if (!message || isSending) return;

        setIsSending(true);
        try {
            await onSend(message);
            setContent("");
        } finally {
            setIsSending(false);
        }
    };

    return (
        <section className="flex min-w-0 flex-1 flex-col bg-[#121212]">
            <header className="border-b border-white/10 px-5 py-4">
                <h2 className="truncate text-base font-semibold text-white">
                    {getConversationName(conversation, t("chat.unnamedConversation"), currentUser)}
                </h2>
            </header>
            <div className="flex-1 overflow-y-auto px-5 py-4">
                {isLoading ? (
                    <div className="flex h-full items-center justify-center text-white/60">
                        <Loader2 className="mr-2 h-5 w-5 animate-spin"/>{t("chat.loadingMessages")}
                    </div>
                ) : error ? (
                    <div
                        className="flex h-full items-center justify-center text-center text-sm text-white/60">{error}</div>
                ) : messages.length === 0 ? (
                    <div
                        className="flex h-full items-center justify-center text-center text-sm text-white/60">{t("chat.noMessages")}</div>
                ) : (
                    <div className="space-y-3">
                        {messages.map((message) => (
                            <div key={message.id}
                                 className={cn("flex", message.isOwn ? "justify-end" : "justify-start")}>
                                <div className={cn(
                                    "max-w-[80%] rounded-2xl px-3 py-2 text-sm",
                                    message.isOwn ? "rounded-br-md bg-[#fe2c55] text-white" : "rounded-bl-md bg-white/10 text-white"
                                )}>
                                    {!message.isOwn &&
                                        <p className="mb-1 text-xs font-medium text-white/60">{message.senderUsername}</p>}
                                    <p className="whitespace-pre-wrap break-words">{message.content}</p>
                                </div>
                            </div>
                        ))}
                        <div ref={bottomRef}/>
                    </div>
                )}
            </div>
            <form onSubmit={submit} className="flex gap-2 border-t border-white/10 p-4">
                <Input
                    value={content}
                    onChange={(event) => setContent(event.target.value)}
                    placeholder={t("chat.messagePlaceholder")}
                    disabled={!isConnected || isSending}
                    className="border-white/10 bg-white/5 text-white placeholder:text-white/40"
                />
                <Button type="submit" size="icon" disabled={!content.trim() || !isConnected || isSending}
                        aria-label={t("chat.send")}>
                    {isSending ? <Loader2 className="h-4 w-4 animate-spin"/> : <Send className="h-4 w-4"/>}
                </Button>
            </form>
            {!isConnected && <p className="px-4 pb-3 text-xs text-white/45">{t("chat.connecting")}</p>}
        </section>
    );
};

export default ConversationWindow;
