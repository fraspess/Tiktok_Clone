import {Navigate, Outlet} from "react-router-dom";
import {useAppSelector} from "@/store/hooks.ts";
import {hasAdminRole} from "@/lib/jwt.ts";

interface AdminRouteProps {
    redirectTo?: string;
}

const AdminRoute = ({redirectTo = "/"}: AdminRouteProps) => {
    const accessToken = useAppSelector((state) => state.auth.accessToken);

    if (!hasAdminRole(accessToken)) {
        return <Navigate to={redirectTo} replace/>;
    }

    return <Outlet/>;
};

export default AdminRoute;