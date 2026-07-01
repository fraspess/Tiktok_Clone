import {Outlet} from "react-router-dom";
import Sidebar from "@/components/layout/Sidebar.tsx";
import Topbar from "@/components/layout/Topbar.tsx";
import {useState} from "react";
import AuthModal from "@/components/modals/AuthModal";


interface MainLayoutProps {
    children?: React.ReactNode;
}

const MainLayout = ({children}: MainLayoutProps) => {
    const [isCollapsed, setIsCollapsed] = useState(false);

    return (
        <div className="flex h-screen">
            <Sidebar collapsed={isCollapsed} onToggle={() => setIsCollapsed(!isCollapsed)}/>
            <div className="flex flex-col flex-1">
                <Topbar/>
                <main className="flex-1 overflow-auto">
                    {children ?? <Outlet/>}
                </main>
            </div>
            <AuthModal/>
        </div>
    )
}
export default MainLayout