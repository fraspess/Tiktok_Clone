import {Outlet} from "react-router-dom";
import Sidebar from "@/components/layout/Sidebar.tsx";
import Topbar from "@/components/layout/Topbar.tsx";
import {useState} from "react";
import AuthModal from "@/components/modals/AuthModal";
import MessagesDrawer from "@/components/chat/MessagesDrawer.tsx";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openDrawer, closeDrawer} from "@/store/slices/messagesDrawerSlice.ts";


interface MainLayoutProps {
    children?: React.ReactNode;
}

const MainLayout = ({children}: MainLayoutProps) => {
    const [isCollapsed, setIsCollapsed] = useState(false);
    const dispatch = useAppDispatch();
    const isMessagesOpen = useAppSelector((s) => s.messagesDrawer.isOpened);

    return (
        <div className="flex h-screen">
            <div>
                <Sidebar
                    collapsed={isCollapsed}
                    onToggle={() => setIsCollapsed(!isCollapsed)}
                    isMessagesOpen={isMessagesOpen}
                    onMessagesClick={() => dispatch(openDrawer())}
                />

            </div>
            <div className="flex flex-col flex-1">
                <main className="flex-1 overflow-hidden pt-16">
                    {children ?? <Outlet/>}
                </main>
            </div>
            <Topbar/>
            <AuthModal/>
            <MessagesDrawer open={isMessagesOpen} onOpenChange={(open) => open ? dispatch(openDrawer()) : dispatch(closeDrawer())}/>
        </div>
    )
}
export default MainLayout
