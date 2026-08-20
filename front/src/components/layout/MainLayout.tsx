import {Outlet} from "react-router-dom";
import Sidebar from "@/components/layout/Sidebar.tsx";
import Topbar from "@/components/layout/Topbar.tsx";
import {useState} from "react";
import AuthModal from "@/components/modals/AuthModal";
import MessagesDrawer from "@/components/chat/MessagesDrawer.tsx";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {closeMessages, openMessages} from "@/store/slices/messagesSlice.ts";


interface MainLayoutProps {
    children?: React.ReactNode;
}

const MainLayout = ({children}: MainLayoutProps) => {
    const [isCollapsed, setIsCollapsed] = useState(false);
    const dispatch = useAppDispatch();
    const isMessagesOpen = useAppSelector((state) => state.messages.isOpen);

    return (
        <div className="flex h-screen">
            <Sidebar
                collapsed={isCollapsed}
                onToggle={() => setIsCollapsed(!isCollapsed)}
                isMessagesOpen={isMessagesOpen}
                onMessagesClick={() => dispatch(openMessages())}
            />
            <div className="flex flex-col flex-1">
                <Topbar/>
                <main className="flex-1 overflow-hidden">
                    {children ?? <Outlet/>}
                </main>
            </div>
            <AuthModal/>
            <MessagesDrawer open={isMessagesOpen} onOpenChange={(open) => dispatch(open ? openMessages() : closeMessages())}/>
        </div>
    )
}
export default MainLayout
