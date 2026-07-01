import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {useEffect} from "react";
import {useRefreshTokenMutation} from "@/store/apis/authApi.ts";
import {logout, setAccessToken, setIsLoading} from "@/store/slices/authSlice.ts";
import {FullPageSpinner} from "@/pages/FullPageSpinner.tsx";


function AuthBootstrap({children}: { children: React.ReactNode }) {
    const dispatch = useAppDispatch();
    const status = useAppSelector(state => state.auth.isLoading);
    const [refreshToken] = useRefreshTokenMutation();

    useEffect(() => {
        dispatch(setIsLoading(true));
        sessionStorage.setItem('lastRefreshAttempt', String(Date.now()));
        refreshToken()
            .unwrap()
            .then((data) => dispatch(setAccessToken(data.accessToken)))
            .catch(() => dispatch(logout()));
    }, [])
    if (status) {
        return <FullPageSpinner/>
    }

    return <>{children}</>
}

export default AuthBootstrap;