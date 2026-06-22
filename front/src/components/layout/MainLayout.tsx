import {Outlet} from "react-router-dom";
import Sidebar from "@/components/layout/Sidebar.tsx";

const MainLayout = () => {
    return(
        <div className="flex">
            <nav>
                <Sidebar />
            </nav>
            <main className="flex-1">
                <Outlet />
            </main>
        </div>
    )
}

export default MainLayout