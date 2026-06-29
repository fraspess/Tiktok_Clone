import {Outlet} from "react-router-dom";
import Sidebar from "@/components/layout/Sidebar.tsx";
import AuthModal from "@/components/modals/AuthModal.tsx";
import Topbar from "@/components/layout/Topbar.tsx";
import {useState} from "react";

const MainLayout = () => {
    const [isCollapsed, setIsCollapsed] = useState(false);

    return(
        <div className="flex h-screen">
            <Sidebar collapsed={isCollapsed} onToggle={() => setIsCollapsed(!isCollapsed)} />
            <div className="flex flex-col flex-1">
                <Topbar />
                <main className="flex-1 overflow-auto">
                    <Outlet />
                </main>
            </div>
            <AuthModal />
        </div>
    )
}

export default MainLayout