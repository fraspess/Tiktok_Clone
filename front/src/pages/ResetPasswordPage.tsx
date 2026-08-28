import {useForm} from "react-hook-form";
import {Field, FieldGroup} from "@/components/ui/field.tsx";
import {Label} from "@/components/ui/label.tsx";
import {Input} from "@/components/ui/input.tsx";
import {useTranslation} from "react-i18next";
import {useState} from "react";
import {CheckCircle2, CircleAlert, Eye, EyeOff} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {useResetPasswordMutation} from "@/store/apis/authApi.ts";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import type {ApiResponse} from "@/types/ApiResponse.ts";
import {useSearchParams} from "react-router-dom";

interface ResetPasswordFormData {
    newPassword: string;
    confirmPassword: string;
}

const ResetPasswordPage = () => {
    const {t} = useTranslation();
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token");
    const email = searchParams.get("email");

    const [resetPassword, {isLoading}] = useResetPasswordMutation();
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [error, setFormError] = useState<string | null>(null);
    const [isSuccess, setIsSuccess] = useState(false);

    const {
        register, handleSubmit, watch, trigger, formState: {errors, isSubmitting}
    } = useForm<ResetPasswordFormData>({
        mode: "onChange",
        reValidateMode: "onChange"
    });

    const isValidParams = token && email;

    if (!isValidParams) {
        return (
            <div className="flex min-h-[calc(100vh-4rem)] items-center justify-center px-4">
                <div className="w-full max-w-md text-center">
                    <CircleAlert className="mx-auto h-16 w-16 text-red-500 mb-4"/>
                    <h1 className="text-2xl font-semibold mb-2">{t("auth.resetPassword.invalidLinkTitle")}</h1>
                    <p className="text-muted-foreground">{t("auth.resetPassword.invalidLinkDescription")}</p>
                </div>
            </div>
        )
    }

    const onSubmit = async (data: ResetPasswordFormData) => {
        try {
            setFormError(null);
            await resetPassword({email, token, newPassword: data.newPassword}).unwrap();
            setIsSuccess(true);
        } catch (error) {
            if (!isFetchBaseQueryError(error)) {
                setFormError(t("auth.fallbackError"));
                return;
            }
            const errResponse = error.data as ApiResponse<null>;
            const {code, fieldErrors} = errResponse;

            if (fieldErrors) {
                const allErrors = Object.values(fieldErrors).flat();
                if (allErrors.includes('INVALID_PASSWORD')) {
                    setFormError(t("auth.validation.passwordNotStrong"));
                } else {
                    setFormError(t("auth.fallbackError"));
                }
                return;
            }

            switch (code) {
                case 'INVALID_TOKEN':
                    setFormError(t("auth.resetPassword.invalidToken"));
                    break;
                case 'INVALID_PASSWORD':
                    setFormError(t("auth.validation.passwordNotStrong"));
                    break;
                case 'TOO_MANY_REQUESTS':
                    setFormError(t("auth.tooManyRequests"));
                    break;
                default:
                    setFormError(t("auth.fallbackError"));
            }
        }
    }

    if (isSuccess) {
        return (
            <div className="flex min-h-[calc(100vh-4rem)] items-center justify-center px-4">
                <div className="w-full max-w-md text-center">
                    <CheckCircle2 className="mx-auto h-16 w-16 text-green-500 mb-4"/>
                    <h1 className="text-2xl font-semibold mb-2">{t("auth.resetPassword.successTitle")}</h1>
                    <p className="text-muted-foreground mb-6">{t("auth.resetPassword.successDescription")}</p>
                    <a href="/" className="text-primary underline underline-offset-4 hover:text-primary/80">
                        {t("auth.resetPassword.goToHome")}
                    </a>
                </div>
            </div>
        )
    }

    return (
        <div className="flex min-h-[calc(100vh-4rem)] items-center justify-center px-4">
            <div className="w-full max-w-md">
                <h1 className="text-2xl font-semibold mb-2">{t("auth.resetPassword.title")}</h1>
                <p className="text-muted-foreground mb-6">{t("auth.resetPassword.description")}</p>

                {error && (
                    <div
                        className="mb-4 flex items-center gap-2 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-600">
                        <CircleAlert size={16} className="shrink-0"/>
                        <span>{error}</span>
                    </div>
                )}

                <form onSubmit={handleSubmit(onSubmit)} noValidate>
                    <FieldGroup>
                        <Field>
                            <Label>{t("auth.passwordLabel")}</Label>
                            <div className="relative">
                                <Input
                                    id="newPassword"
                                    type={showPassword ? "text" : "password"}
                                    className="pr-10"
                                    {...register("newPassword", {
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
                            {errors.newPassword && (
                                <p className="text-sm text-red-500">{errors.newPassword.message}</p>
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
                                            value: 8,
                                            message: t("auth.validation.passwordMin")
                                        },
                                        pattern: {
                                            value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$/,
                                            message: t("auth.validation.passwordNotStrong")
                                        },
                                        validate: (value) =>
                                            value === watch("newPassword") || t("auth.validation.passwordMismatch")
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

                    <Button type="submit" disabled={isSubmitting || isLoading} className="w-full mt-6">
                        {t("auth.resetPassword.submit")}
                    </Button>
                </form>
            </div>
        </div>
    )
}

export default ResetPasswordPage;
