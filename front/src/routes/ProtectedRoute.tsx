import {Navigate, Outlet} from "react-router-dom";
import {useAppSelector} from "@/store/hooks.ts";

interface ProtectedRouteProps {
    redirectTo?: string;
}

const ProtectedRoute = ({redirectTo = "/"}: ProtectedRouteProps) => {
    const isAuthenticated = useAppSelector((state) => state.auth.isAuth);

    if (!isAuthenticated) {
        return <Navigate to={redirectTo} replace/>;
    }

    return <Outlet/>;
};

export default ProtectedRoute;