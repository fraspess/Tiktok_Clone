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
            <DialogContent className="sm:max-w-sm">
                <DialogHeader>
                    <DialogTitle>{isSignIn ? t("auth.signInTitle") : t("auth.signUpTitle")}</DialogTitle>
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

                    <DialogFooter className="mt-4 w-full flex flex-col gap-2 sm:flex-col">
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

            </DialogContent>
        </Dialog>
    );
}


export default AuthModal