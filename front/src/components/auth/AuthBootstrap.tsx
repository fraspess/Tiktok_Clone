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
        dispatch(setIsLoading(true));
        // eslint-disable-next-line @typescript-eslint/ban-ts-comment
        // @ts-expect-error
        refreshToken()
            .unwrap()
            .then((data) => {
                const response = data as ApiResponse<{ accessToken: string }>
                console.log(data)
                dispatch(setAccessToken(response.data.accessToken))
            })
            .catch(() => dispatch(logout()));
    }, [])
    if (status) {
        return <FullPageSpinner/>
    }

    return <>{children}</>
}

export default AuthBootstrap;