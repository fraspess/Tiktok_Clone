import {useForm} from "react-hook-form";
import {Field, FieldGroup} from "@/components/ui/field.tsx";
import {Label} from "@/components/ui/label.tsx";
import {Input} from "@/components/ui/input.tsx";
import {useTranslation} from "react-i18next";
import {useState} from "react";
import {CheckCircle2, CircleAlert, Mail} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {DialogFooter} from "@/components/ui/dialog.tsx";
import {useForgotPasswordMutation, useGoogleAuthMutation} from "@/store/apis/authApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import {useGoogleLogin} from "@react-oauth/google";
import {useAppDispatch} from "@/store/hooks.ts";
import {setAccessToken} from "@/store/slices/authSlice.ts";

interface ForgotPasswordFormData {
    email: string;
}

interface ForgotPasswordFormProps {
    onSwitchToSignIn: () => void;
    onSuccess: () => void;
}

const ForgotPasswordForm = ({onSwitchToSignIn, onSuccess}: ForgotPasswordFormProps) => {
    const {t} = useTranslation();
    const [forgotPassword, {isLoading}] = useForgotPasswordMutation();
    const [googleAuth, {isLoading: isGoogleAuthLoading}] = useGoogleAuthMutation();
    const dispatch = useAppDispatch();
    const [error, setFormError] = useState<string | null>(null);
    const [isEmailSent, setIsEmailSent] = useState(false);
    const [isExternalAccount, setIsExternalAccount] = useState(false);

    const {
        register, handleSubmit, formState: {errors, isSubmitting}
    } = useForm<ForgotPasswordFormData>({
        mode: "onChange",
        reValidateMode: "onChange"
    });

    const googleLogin = useGoogleLogin({
        flow: "auth-code",
        onSuccess: async (codeResponse) => {
            try {
                const response = await googleAuth(codeResponse).unwrap() as ApiResponse<{ accessToken: string }>;
                dispatch(setAccessToken(response.data.accessToken));
                onSuccess();
            } catch {
                setFormError(t("auth.fallbackError"));
            }
        },
        onError: () => setFormError(t("auth.fallbackError")),
    });

    const onSubmit = async (data: ForgotPasswordFormData) => {
        try {
            setFormError(null);
            await forgotPassword(data).unwrap();
            setIsEmailSent(true);
        } catch (error) {
            if (!isFetchBaseQueryError(error)) {
                setFormError(t("auth.fallbackError"));
                return;
            }
            const errResponse = error.data as ApiResponse<null>;
            const {code} = errResponse;

            switch (code) {
                case 'INVALID_EMAIL':
                    setFormError(t("auth.validation.email"));
                    break;
                case 'CANT_RESET_PASSWORD_EXTERNAL':
                    setIsExternalAccount(true);
                    break;
                case 'TOO_MANY_REQUESTS':
                    setFormError(t("auth.tooManyRequests"));
                    break;
                default:
                    setFormError(t("auth.fallbackError"));
            }
        }
    }

    if (isEmailSent) {
        return (
            <div className="flex flex-col items-center gap-5 pt-2 text-center">
                <CheckCircle2 className="h-16 w-16 text-green-500"/>
                <div>
                    <p className="font-medium">{t("auth.forgotPassword.successTitle")}</p>
                    <p className="text-sm text-muted-foreground">
                        {t("auth.forgotPassword.successDescription")}
                    </p>
                </div>
                <Button className="w-full mt-2" onClick={onSwitchToSignIn}>
                    {t("auth.signInTitle")}
                </Button>
            </div>
        )
    }

    if (isExternalAccount) {
        return (
            <div className="flex flex-col items-center gap-4 pt-2 text-center">
                <CircleAlert className="h-16 w-16 text-amber-500"/>
                <div>
                    <p className="font-medium">{t("auth.forgotPassword.externalAccountTitle")}</p>
                    <p className="text-sm text-muted-foreground mt-1">
                        {t("auth.forgotPassword.externalAccountDescription")}
                    </p>
                </div>
                <Button
                    onClick={() => googleLogin()}
                    disabled={isGoogleAuthLoading}
                    className="w-full bg-white hover:bg-gray-50 text-gray-700 border border-gray-200"
                >
                    <svg viewBox="-3 0 262 262" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="xMidYMid"
                         fill="#000000" className="h-5 w-5 mr-2">
                        <g id="SVGRepo_bgCarrier" strokeWidth="0"></g>
                        <g id="SVGRepo_tracerCarrier" strokeLinecap="round" strokeLinejoin="round"></g>
                        <g id="SVGRepo_iconCarrier">
                            <path d="M255.878 133.451c0-10.734-.871-18.567-2.756-26.69H130.55v48.448h71.947c-1.45 12.04-9.283 30.172-26.69 42.356l-.244 1.622 38.755 30.023 2.685.268c24.659-22.774 38.875-56.282 38.875-96.027" fill="#4285F4"></path>
                            <path d="M130.55 261.1c35.248 0 64.839-11.605 86.453-31.622l-41.196-31.913c-11.024 7.688-25.82 13.055-45.257 13.055-34.523 0-63.824-22.773-74.269-54.25l-1.531.13-40.298 31.187-.527 1.465C35.393 231.798 79.49 261.1 130.55 261.1" fill="#34A853"></path>
                            <path d="M56.281 156.37c-2.756-8.123-4.351-16.827-4.351-25.82 0-8.994 1.595-17.697 4.206-25.82l-.073-1.73L15.26 71.312l-1.335.635C5.077 89.644 0 109.517 0 130.55s5.077 40.905 13.925 58.602l42.356-32.782" fill="#FBBC05"></path>
                            <path d="M130.55 50.479c24.514 0 41.05 10.589 50.479 19.438l36.844-35.974C195.245 12.91 165.798 0 130.55 0 79.49 0 35.393 29.301 13.925 71.947l42.211 32.783c10.59-31.477 39.891-54.251 74.414-54.251" fill="#EB4335"></path>
                        </g>
                    </svg>
                    {t("auth.googleAuth")}
                </Button>
                <Button type="button" variant="ghost" onClick={onSwitchToSignIn}>
                    <p className="underline">{t("auth.forgotPassword.backToSignIn")}</p>
                </Button>
            </div>
        )
    }

    return (
        <>
            <p className="text-sm text-muted-foreground">
                {t("auth.forgotPassword.description")}
            </p>
            {error && (
                <div
                    className="mt-4 flex items-center gap-2 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-600">
                    <CircleAlert size={16} className="shrink-0"/>
                    <span>{error}</span>
                </div>
            )}
            <form onSubmit={handleSubmit(onSubmit)} noValidate>
                <FieldGroup>
                    <Field>
                        <Label>{t("auth.emailLabel")}</Label>
                        <Input
                            id="email"
                            type="email"
                            placeholder={t("auth.forgotPassword.emailPlaceholder")}
                            {...register("email", {
                                required: t("auth.validation.required"),
                                pattern: {
                                    value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                    message: t("auth.validation.email")
                                }
                            })}
                        />
                        {errors.email && (
                            <p className="text-sm text-red-500">{errors.email.message}</p>
                        )}
                    </Field>
                </FieldGroup>

                <DialogFooter className="mt-6 w-full flex flex-col gap-2 sm:flex-col">
                    <Button type="submit" disabled={isSubmitting || isLoading}>
                        <Mail size={16}/>
                        {t("auth.forgotPassword.submit")}
                    </Button>

                    <Button type="button" variant="ghost" onClick={onSwitchToSignIn}>
                        <p className="underline">{t("auth.forgotPassword.backToSignIn")}</p>
                    </Button>
                </DialogFooter>
            </form>
        </>
    )
}

export default ForgotPasswordForm;
