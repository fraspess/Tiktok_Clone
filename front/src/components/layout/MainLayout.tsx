import {Outlet, useLocation} from "react-router-dom";
import {AnimatePresence} from "framer-motion";
import Sidebar from "@/components/layout/Sidebar.tsx";
import Topbar from "@/components/layout/Topbar.tsx";
import PageTransition from "@/components/layout/PageTransition.tsx";
import {useState} from "react";
import AuthModal from "@/components/modals/AuthModal";
import MobileNav from "@/components/layout/MobileNav.tsx";

interface MainLayoutProps {
    children?: React.ReactNode;
}

// Сторінки з відео на весь екран: на мобільці Topbar там накладається поверх відео,
// на решті сторінок він стає звичайною смужкою над контентом.
const OVERLAY_TOPBAR_ROUTE = /^\/(following)?$|(^|\/)video\//;

const MainLayout = ({children}: MainLayoutProps) => {
    const [isCollapsed, setIsCollapsed] = useState(false);
    const location = useLocation();
    const overlayTopbar = OVERLAY_TOPBAR_ROUTE.test(location.pathname);

    return (
        <div className="flex h-dvh">
            <Sidebar
                collapsed={isCollapsed}
                onToggle={() => setIsCollapsed(!isCollapsed)}
            />
            <div className="flex min-w-0 flex-1 flex-col">
                <Topbar overlay={overlayTopbar}/>
                <main className="min-h-0 flex-1 overflow-y-auto">
                    <AnimatePresence mode="wait" initial={false}>
                        <PageTransition key={location.pathname}>
                            {children ?? <Outlet/>}
                        </PageTransition>
                    </AnimatePresence>
                </main>
                <MobileNav/>
            </div>
            <AuthModal/>
        </div>
    )
}
export default MainLayout