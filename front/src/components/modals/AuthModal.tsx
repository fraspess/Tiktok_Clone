import {
    Dialog,
    DialogContent,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import {useTranslation} from "react-i18next";
import {useState} from "react";
import {Field, FieldGroup} from "@/components/ui/field.tsx";
import {Label} from "@/components/ui/label.tsx";
import {Input} from "@/components/ui/input.tsx";
import {useForm} from "react-hook-form";
import {Button} from "@/components/ui/button.tsx";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {closeModal} from "@/store/slices/authModalSlice.ts";
import {Separator} from "@/components/ui/separator.tsx";


interface AuthnFormData{
    identifier: string
    email?: string;
    username?: string;
    password: string;
    confirmPassword: string;
}

const AuthModal = () => {
    const isOpened = useAppSelector(state => state.authModal.isOpened)
    const dispatch = useAppDispatch();
    const { t } = useTranslation()
    const [isSignIn, setIsSignIn] = useState<boolean>(true)

    const {
        register, handleSubmit, reset, formState: {errors, isSubmitting},
    } = useForm<AuthnFormData>();

    const onSubmit = (data : AuthnFormData) => {
        console.log(data)
    }

    const handleClose = () => {
        dispatch(closeModal())
        reset();
    }
    const toggleMode = () => {
        setIsSignIn(!isSignIn);
        reset();
    }

    return (
        <Dialog open={isOpened} onOpenChange={handleClose}>
            <DialogContent className="sm:max-w-sm max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle className="text-4xl">{isSignIn ? t("auth.signInTitle") : t("auth.signUpTitle")}</DialogTitle>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} noValidate>
                    <FieldGroup>
                        {isSignIn ? (
                            <>
                        <Field>
                            <Label>{t("auth.identifierLabel")}</Label>
                            <Input
                                id="identifier"
                                type="text"
                                {...register("identifier", {
                                    required: t("auth.validation.required"),
                                })}
                            />
                            {errors.identifier && (
                                <p className="text-sm text-red-500">{errors.identifier.message}</p>
                            )}
                        </Field>

                        <Field>
                            <Label>{t("auth.passwordLabel")}</Label>
                            <Input
                                id="password"
                                type="password"
                                {...register("password", {
                                    required: t("auth.validation.required"),
                                    minLength: {
                                        value: 6,
                                        message: t("auth.validation.passwordMin")
                                    },
                                })}
                            />
                            {errors.password && (
                                <p className="text-sm text-red-500">{errors.password.message}</p>
                                )}
                        </Field>
                            </>
                        ) : (
                            <>

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
                                    <Input
                                        id="password"
                                        type="password"
                                        {...register("password", {
                                            required: t("auth.validation.required"),
                                            minLength: {
                                                value: 6,
                                                message: t("auth.validation.passwordMin")
                                            },
                                        })}
                                    />
                                    {errors.password && (
                                        <p className="text-sm text-red-500">{errors.password.message}</p>
                                    )}
                                </Field>

                                <Field>
                                    <Label>{t("auth.confirmPasswordLabel")}</Label>
                                    <Input
                                        id="confirmPassword"
                                        type="password"
                                        {...register("confirmPassword", {
                                            required: t("auth.validation.required"),
                                            minLength: {
                                                value: 6,
                                                message: t("auth.validation.passwordMin")
                                            }
                                        })}
                                    />
                                    {errors.confirmPassword && (
                                        <p className="text-sm text-red-500">{errors.confirmPassword.message}</p>
                                    )}
                                </Field>
                            </>
                        )}
                    </FieldGroup>

                    <DialogFooter className="mt-6 w-full flex flex-col gap-2 sm:flex-col">
                        <Button type="submit" disabled={isSubmitting}>
                            {isSubmitting
                                ? t("auth.loading")
                                : isSignIn
                                    ? t("auth.signInTitle")
                                    : t("auth.signUpTitle")}
                        </Button>

                        <Button type="button" variant="ghost" onClick={toggleMode}>
                            <p className="underline">{isSignIn ? t("auth.noAccountPrompt") : t("auth.hasAccountPrompt")}</p>
                        </Button>
                    </DialogFooter>
                </form>


                <Separator />
                <Button className="bg-white hover:bg-gray-50 text-gray-700 border border-gray-200">
                    <svg viewBox="-3 0 262 262" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="xMidYMid" fill="#000000"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"><path d="M255.878 133.451c0-10.734-.871-18.567-2.756-26.69H130.55v48.448h71.947c-1.45 12.04-9.283 30.172-26.69 42.356l-.244 1.622 38.755 30.023 2.685.268c24.659-22.774 38.875-56.282 38.875-96.027" fill="#4285F4"></path><path d="M130.55 261.1c35.248 0 64.839-11.605 86.453-31.622l-41.196-31.913c-11.024 7.688-25.82 13.055-45.257 13.055-34.523 0-63.824-22.773-74.269-54.25l-1.531.13-40.298 31.187-.527 1.465C35.393 231.798 79.49 261.1 130.55 261.1" fill="#34A853"></path><path d="M56.281 156.37c-2.756-8.123-4.351-16.827-4.351-25.82 0-8.994 1.595-17.697 4.206-25.82l-.073-1.73L15.26 71.312l-1.335.635C5.077 89.644 0 109.517 0 130.55s5.077 40.905 13.925 58.602l42.356-32.782" fill="#FBBC05"></path><path d="M130.55 50.479c24.514 0 41.05 10.589 50.479 19.438l36.844-35.974C195.245 12.91 165.798 0 130.55 0 79.49 0 35.393 29.301 13.925 71.947l42.211 32.783c10.59-31.477 39.891-54.251 74.414-54.251" fill="#EB4335"></path></g></svg>
                    {t("auth.googleAuth")}
                </Button>
            </DialogContent>
        </Dialog>
    );
}


export default AuthModal