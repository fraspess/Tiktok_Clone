import {Outlet, useLocation} from "react-router-dom";
import {AnimatePresence} from "framer-motion";
import Sidebar from "@/components/layout/Sidebar.tsx";
import Topbar from "@/components/layout/Topbar.tsx";
import PageTransition from "@/components/layout/PageTransition.tsx";
import {useState} from "react";
import AuthModal from "@/components/modals/AuthModal";

interface MainLayoutProps {
    children?: React.ReactNode;
}

const MainLayout = ({children}: MainLayoutProps) => {
    const [isCollapsed, setIsCollapsed] = useState(false);
    const location = useLocation();

    return (
        <div className="flex h-screen">
            <Sidebar
                collapsed={isCollapsed}
                onToggle={() => setIsCollapsed(!isCollapsed)}
            />
            <div className="flex flex-col flex-1">
                <Topbar/>
                <main className="flex-1 overflow-hidden">
                    <AnimatePresence mode="wait" initial={false}>
                        <PageTransition key={location.pathname}>
                            {children ?? <Outlet/>}
                        </PageTransition>
                    </AnimatePresence>
                </main>
            </div>
            <AuthModal/>
        </div>
    )
}
export default MainLayout