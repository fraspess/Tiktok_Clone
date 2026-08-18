import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {useEffect} from "react";
import {useRefreshTokenMutation} from "@/store/apis/authApi.ts";
import {logout, setAccessToken, setIsLoading} from "@/store/slices/authSlice.ts";
import {FullPageSpinner} from "@/pages/FullPageSpinner.tsx";
import type {ApiResponse} from "@/types/ApiResponse.ts";


function AuthBootstrap({children}: { children: React.ReactNode }) {
    const dispatch = useAppDispatch();
    const status = useAppSelector(state => state.auth.isLoading);
    const [refreshToken] = useRefreshTokenMutation();

    useEffect(() => {
        let cancelled = false;
        dispatch(setIsLoading(true));

        // eslint-disable-next-line @typescript-eslint/ban-ts-comment
        // @ts-expect-error
        refreshToken()
            .unwrap()
            .then((data) => {
                if (cancelled) return;
                const response = data as ApiResponse<{ accessToken: string }>;
                dispatch(setAccessToken(response.data.accessToken));
            })
            .catch(() => {
                if (!cancelled) dispatch(logout());
            });

        // React Strict Mode remounts once; ignore the abandoned first run.
        return () => {
            cancelled = true;
        };
    }, [dispatch, refreshToken]);

    if (status) {
        return <FullPageSpinner/>
    }

    return <>{children}</>
}

export default AuthBootstrap;