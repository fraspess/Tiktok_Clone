import {Navigate, Outlet} from "react-router-dom";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {openModal} from "@/store/slices/authModalSlice.ts";

interface ProtectedRouteProps {
    redirectTo?: string;
}

const ProtectedRoute = ({redirectTo = "/"}: ProtectedRouteProps) => {
    const isAuthenticated = useAppSelector((state) => state.auth.isAuth);
    const dispatch = useAppDispatch();
    if (!isAuthenticated) {
        dispatch(openModal());
        return <Navigate to={redirectTo} replace/>;

    }

    return <Outlet/>;
};

export default ProtectedRoute;