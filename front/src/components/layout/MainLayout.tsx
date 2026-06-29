import {Outlet} from "react-router-dom";
import Sidebar from "@/components/layout/Sidebar.tsx";
import AuthModal from "@/components/modals/AuthModal.tsx";

const MainLayout = () => {
    return(
        <div className="flex">
            <nav>
                <Sidebar />
            </nav>
            <main className="flex-1">
                <Outlet />
            </main>

            <AuthModal></AuthModal>
        </div>
    )
}

export default MainLayout