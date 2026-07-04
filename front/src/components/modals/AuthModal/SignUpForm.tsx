import {useTranslation} from "react-i18next";
import {useForm} from "react-hook-form";
import {useState} from "react";
import {Field, FieldGroup} from "@/components/ui/field.tsx";
import {Label} from "@/components/ui/label.tsx";
import {Input} from "@/components/ui/input.tsx";
import {CircleAlert, Eye, EyeOff} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {DialogFooter} from "@/components/ui/dialog.tsx";
import {Separator} from "@/components/ui/separator.tsx";
import {useGoogleLogin} from "@react-oauth/google";
import {useGoogleAuthMutation, useRegisterMutation} from "@/store/apis/authApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import {setAccessToken} from "@/store/slices/authSlice.ts";
import {useDispatch} from "react-redux";
import {closeModal} from "@/store/slices/authModalSlice.ts";

interface SignUpFormData {
    email: string;
    username: string;
    password: string;
    confirmPassword: string;
}

interface SignUpProps {
    onSwitchToSignIn: () => void,
    onSuccess: (email: string) => void,
}

const SignUpForm = ({onSwitchToSignIn, onSuccess}: SignUpProps) => {
    const {t} = useTranslation();
    const [showPassword, setShowPassword] = useState(false);
    const [googleAuth, {isLoading: isGoogleAuthLoading}] = useGoogleAuthMutation();
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [registerRequest, {isLoading: isRegisterLoading}] = useRegisterMutation();
    const [bannerError, setBannerError] = useState<string | null>(null);
    const dispatch = useDispatch();

    const {
        register, setError, handleSubmit, watch, trigger, formState: {errors, isSubmitting},
    } = useForm<SignUpFormData>({
        mode: "onChange",
        reValidateMode: "onChange"
    });
    const googleLogin = useGoogleLogin({
        flow: "auth-code",
        onSuccess: async (codeResponse) => {
            try {
                const response = await googleAuth(codeResponse).unwrap() as ApiResponse<{ accessToken: string }>;
                dispatch(setAccessToken(response.data.accessToken));
                dispatch(closeModal())
            } catch (err) {
                if (!isFetchBaseQueryError(err)) {
                    setBannerError(t("auth.fallbackError"));
                    return;
                }
            }
        },
        onError: (error) => console.log(error),
    })

    const onSubmit = async (data: SignUpFormData) => {
        console.log(data);
        try {
            await registerRequest(data).unwrap();
            onSuccess(data.email);
        } catch (error) {
            if (!isFetchBaseQueryError(error)) {
                setBannerError(t("auth.fallbackError"));
                return;
            }
            const errResponse = error.data as ApiResponse<null>;
            const {code} = errResponse;

            switch (code) {
                case 'USERNAME_ALREADY_EXISTS':
                    setError("username", {
                        type: "custom",
                        message: t("auth.usernameAlreadyTaken"),
                    }, {shouldFocus: true});
                    break;
                case 'EMAIL_ALREADY_EXISTS':
                    setError("email", {
                        type: "custom",
                        message: t("auth.emailAlreadyTaken")
                    })
                    break;
                default:
                    setBannerError(t("auth.fallbackError"))
            }

        }
    }

    return (
        <>
            {bannerError && (
                <div
                    className="mb-4 flex items-center gap-2 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-600">
                    <CircleAlert size={16} className="shrink-0"/>
                    <span>{bannerError}</span>
                </div>
            )}
            <form onSubmit={handleSubmit(onSubmit)} noValidate>
                <FieldGroup>
                    <Field>
                        <Label>{t("auth.emailLabel")}</Label>
                        <Input
                            id="email"
                            type="email"
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

                    <Field>
                        <Label>{t("auth.usernameLabel")}</Label>
                        <Input
                            id="username"
                            type="text"
                            {...register("username", {
                                required: t("auth.validation.required"),
                                minLength: {
                                    value: 3,
                                    message: t("auth.validation.usernameMin")
                                }
                            })}
                        />
                        {errors.username && (
                            <p className="text-sm text-red-500">{errors.username.message}</p>
                        )}
                    </Field>

                    <Field>
                        <Label>{t("auth.passwordLabel")}</Label>
                        <div className="relative">
                            <Input
                                id="password"
                                type={showPassword ? "text" : "password"}
                                className="pr-10"
                                {...register("password", {
                                    required: t("auth.validation.required"),
                                    minLength: {
                                        value: 8,
                                        message: t("auth.validation.passwordMin")
                                    },
                                    pattern: {
                                        value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$/,
                                        message: t("auth.validation.passwordNotStrong")
                                    },
                                    onChange: () => {
                                        if (watch("confirmPassword")) {
                                            trigger("confirmPassword")
                                        }
                                    }
                                })}
                            />
                            <button
                                type="button"
                                onClick={() => setShowPassword((prev) => !prev)}
                                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                                tabIndex={-1}
                            >
                                {showPassword ? <EyeOff size={18}/> : <Eye size={18}/>}
                            </button>
                        </div>
                        {errors.password && (
                            <p className="text-sm text-red-500">{errors.password.message}</p>
                        )}
                    </Field>

                    <Field>
                        <Label>{t("auth.confirmPasswordLabel")}</Label>
                        <div className="relative">
                            <Input
                                id="confirmPassword"
                                type={showConfirmPassword ? "text" : "password"}
                                className="pr-10"
                                {...register("confirmPassword", {
                                    required: t("auth.validation.required"),
                                    minLength: {
                                        value: 6,
                                        message: t("auth.validation.passwordMin")
                                    },
                                    pattern: {
                                        value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$/,
                                        message: t("auth.validation.passwordNotStrong")
                                    },
                                    validate: (value) =>
                                        value === watch("password") || t("auth.validation.passwordMismatch")
                                })}
                            />
                            <button
                                type="button"
                                onClick={() => setShowConfirmPassword((prev) => !prev)}
                                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                                tabIndex={-1}
                            >
                                {showConfirmPassword ? <EyeOff size={18}/> : <Eye size={18}/>}
                            </button>
                        </div>
                        {errors.confirmPassword && (
                            <p className="text-sm text-red-500">{errors.confirmPassword.message}</p>
                        )}
                    </Field>
                </FieldGroup>

                <DialogFooter className="mt-6 w-full flex flex-col gap-2 sm:flex-col">
                    <Button type="submit" disabled={isSubmitting && isRegisterLoading}>
                        {t("auth.signUpTitle")}
                    </Button>

                    <Button type="button" variant="ghost" onClick={() => onSwitchToSignIn()}>
                        <p className="underline">{t("auth.hasAccountPrompt")}</p>
                    </Button>
                </DialogFooter>
            </form>

            <Separator/>
            <Button onClick={googleLogin} className="bg-white hover:bg-gray-50 text-gray-700 border border-gray-200"
                    disabled={isGoogleAuthLoading}>
                <svg viewBox="-3 0 262 262" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="xMidYMid"
                     fill="#000000">
                    <g id="SVGRepo_bgCarrier" stroke-width="0"></g>
                    <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g>
                    <g id="SVGRepo_iconCarrier">
                        <path
                            d="M255.878 133.451c0-10.734-.871-18.567-2.756-26.69H130.55v48.448h71.947c-1.45 12.04-9.283 30.172-26.69 42.356l-.244 1.622 38.755 30.023 2.685.268c24.659-22.774 38.875-56.282 38.875-96.027"
                            fill="#4285F4"></path>
                        <path
                            d="M130.55 261.1c35.248 0 64.839-11.605 86.453-31.622l-41.196-31.913c-11.024 7.688-25.82 13.055-45.257 13.055-34.523 0-63.824-22.773-74.269-54.25l-1.531.13-40.298 31.187-.527 1.465C35.393 231.798 79.49 261.1 130.55 261.1"
                            fill="#34A853"></path>
                        <path
                            d="M56.281 156.37c-2.756-8.123-4.351-16.827-4.351-25.82 0-8.994 1.595-17.697 4.206-25.82l-.073-1.73L15.26 71.312l-1.335.635C5.077 89.644 0 109.517 0 130.55s5.077 40.905 13.925 58.602l42.356-32.782"
                            fill="#FBBC05"></path>
                        <path
                            d="M130.55 50.479c24.514 0 41.05 10.589 50.479 19.438l36.844-35.974C195.245 12.91 165.798 0 130.55 0 79.49 0 35.393 29.301 13.925 71.947l42.211 32.783c10.59-31.477 39.891-54.251 74.414-54.251"
                            fill="#EB4335"></path>
                    </g>
                </svg>
                {t("auth.googleAuth")}
            </Button>
        </>
    )
}

export default SignUpForm;