import {CheckCircle2, CircleAlert, RefreshCwIcon} from "lucide-react"

import {Button} from "@/components/ui/button"
import {Field, FieldGroup, FieldLabel,} from "@/components/ui/field"
import {InputOTP, InputOTPGroup, InputOTPSeparator, InputOTPSlot,} from "@/components/ui/input-otp"
import {REGEXP_ONLY_DIGITS} from "input-otp";
import {useTranslation} from "react-i18next";
import {DialogDescription} from "@/components/ui/dialog.tsx";
import {useConfirmEmailMutation, useResendConfirmationCodeMutation} from "@/store/apis/authApi.ts";
import {cn} from "@/lib/utils.ts";
import {Input} from "@/components/ui/input.tsx";
import {Controller, useForm} from "react-hook-form";
import isFetchBaseQueryError from "@/store/isFetchBaseQueryError.ts";
import {useState} from "react";
import type {ApiResponse} from "@/types/ApiResponse.ts";

interface ConfirmEmailFormData {
    email: string;
    token: string;
}

interface ConfirmEmailFormProps {
    email: string;
    onSuccess: () => void;
}

export function ConfirmEmailForm({email, onSuccess}: ConfirmEmailFormProps) {
    const {t} = useTranslation();
    const [confirmEmail, {isLoading}] = useConfirmEmailMutation();
    const [resendConfirmationCode, {isLoading: isConfirmCodeLoading}] = useResendConfirmationCodeMutation();
    const [bannerError, setBannerError] = useState<string>();
    const [confirmCodeSentBanner, setConfirmCodeSentBanner] = useState<string>();
    const [isConfirmed, setIsConfirmed] = useState<boolean>(false);
    const {
        register, handleSubmit, control, setError, formState: {errors}
    } = useForm<ConfirmEmailFormData>({
        defaultValues: {email}
    });

    const onSubmit = async (data: ConfirmEmailFormData) => {
        if (isLoading) {
            return;
        }
        setBannerError("");
        setConfirmCodeSentBanner("");
        try {
            await confirmEmail(data).unwrap();
            setIsConfirmed(true);

        } catch (err) {
            if (!isFetchBaseQueryError(err)) {
                setBannerError(t("auth.fallbackError"));
                return;
            }
            const errorResponse = err.data as ApiResponse<null>;
            const {code} = errorResponse;

            switch (code) {
                case 'INVALID_TOKEN':
                    setError("token", {
                        type: "custom",
                        message: t("auth.email.invalidConfirmEmailToken")
                    });
                    break;
                case 'EMAIL_ALREADY_CONFIRMED': {
                    setBannerError(t("auth.email.alreadyConfirmed"));
                    break;
                }
                default:
                    setBannerError(t("auth.fallbackError"));
                    break;
            }
        }
    }

    const onResendCode = async () => {
        try {
            await resendConfirmationCode({email}).unwrap();
            setConfirmCodeSentBanner(t("auth.email.resendCodeSuccess"))
            setBannerError("");
        } catch (err) {
            if (!isFetchBaseQueryError(err)) {
                setBannerError(t("auth.fallbackError"));
                return;
            }

            const errResponse = err.data as ApiResponse<null>;
            const {code} = errResponse;

            switch (code) {
                case 'TOO_FAST':
                    setBannerError(t("auth.email.tooFast"));
                    break;
                default:
                    setBannerError(t("auth.fallbackError"));
                    break;
            }
        }
    }

    if (isConfirmed) {
        return (
            <div className="flex flex-col items-center gap-5 pt-2 text-center">
                <CheckCircle2 className="h-16 w-16 text-green-500"/>
                <div>
                    <p className="font-medium">{t("auth.email.confirmedTitle")}</p>
                    <p className="text-sm text-muted-foreground">
                        {t("auth.email.confirmedDescription")}
                    </p>
                </div>
                <Button className="w-full mt-2" onClick={onSuccess}>
                    {t("auth.signInTitle")}
                </Button>
            </div>
        )
    }

    const otpSlotClass = "h-12 w-10 text-lg sm:h-14 sm:w-12 sm:text-xl";
    return (
        <>
            <DialogDescription>
                {t("auth.email.description", {email: ""})}
            </DialogDescription>
            {confirmCodeSentBanner && (
                <div
                    className="mb-4 flex items-center gap-2 rounded-md border border-green-200 bg-green-50 px-3 py-2 text-sm text-green-600">
                    <CircleAlert size={16} className="shrink-0"/>
                    <span>{confirmCodeSentBanner}</span>
                </div>
            )}
            {bannerError && (
                <div
                    className="mb-4 flex items-center gap-2 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-600">
                    <CircleAlert size={16} className="shrink-0"/>
                    <span>{bannerError}</span>
                </div>
            )}
            <form onSubmit={handleSubmit(onSubmit)}>
                <Input type="hidden" id="email" {...register("email")}></Input>
                <FieldGroup>
                    <Field>
                        <div className="flex items-center justify-between w-full my-1 gap-2 flex-wrap sm:flex-nowrap ">
                            <FieldLabel htmlFor="otp-verification" className="min-w-0">
                                {t("auth.email.verificationCode")}
                            </FieldLabel>
                            <Button onClick={() => onResendCode()} variant="outline" type="button" size="xs"
                                    disabled={isConfirmCodeLoading}>
                                <RefreshCwIcon className={cn(isConfirmCodeLoading && "animate-spin")}/>
                                {t("auth.email.resendCode")}
                            </Button>
                        </div>

                        <div className="flex justify-center">
                            <Controller
                                name="token"
                                control={control}
                                rules={{
                                    required: true,
                                    minLength: 6,
                                    maxLength: 6,
                                    pattern: /^\d+$/,
                                }}
                                render={({field}) => (
                                    <InputOTP
                                        {...field}
                                        maxLength={6}
                                        id="otp-verification"
                                        pattern={REGEXP_ONLY_DIGITS}
                                    >
                                        <InputOTPGroup>
                                            <InputOTPSlot index={0} className={otpSlotClass}/>
                                            <InputOTPSlot index={1} className={otpSlotClass}/>
                                            <InputOTPSlot index={2} className={otpSlotClass}/>
                                        </InputOTPGroup>

                                        <InputOTPSeparator/>

                                        <InputOTPGroup>
                                            <InputOTPSlot index={3} className={otpSlotClass}/>
                                            <InputOTPSlot index={4} className={otpSlotClass}/>
                                            <InputOTPSlot index={5} className={otpSlotClass}/>
                                        </InputOTPGroup>
                                    </InputOTP>
                                )}
                            />
                        </div>
                        {errors.token && (
                            <p className="text-sm text-red-500 text-center mt-1">{errors.token.message}</p>
                        )}
                    </Field>
                    <Field>
                        <Button type="submit" disabled={isLoading} className={cn("w-full")}>
                            {t("auth.email.verify")}
                        </Button>
                        <div className="text-sm text-muted-foreground text-center">
                            <a
                                href="#"
                                className="underline underline-offset-4 transition-colors hover:text-primary"
                            >
                                {t("auth.email.resetPassword")}
                            </a>
                        </div>
                    </Field>
                </FieldGroup>
            </form>
        </>
    )
}

export default ConfirmEmailForm;
