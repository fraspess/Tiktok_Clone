import {RefreshCwIcon} from "lucide-react"

import {Button} from "@/components/ui/button"
import {Field, FieldGroup, FieldLabel,} from "@/components/ui/field"
import {InputOTP, InputOTPGroup, InputOTPSeparator, InputOTPSlot,} from "@/components/ui/input-otp"
import {REGEXP_ONLY_DIGITS} from "input-otp";
import {useTranslation} from "react-i18next";
import {DialogDescription} from "@/components/ui/dialog.tsx";
import {useConfirmEmailMutation} from "@/store/apis/authApi.ts";
import {cn} from "@/lib/utils.ts";
import {Input} from "@/components/ui/input.tsx";
import {Controller, useForm} from "react-hook-form";

interface ConfirmEmailFormData {
    email: string;
    token: string;
}

interface ConfirmEmailFormProps {
    email: string;
}

export function ConfirmEmailForm({email}: ConfirmEmailFormProps) {
    const {t} = useTranslation();
    const [confirmEmail, {isLoading}] = useConfirmEmailMutation();

    const {
        register, handleSubmit, control
    } = useForm<ConfirmEmailFormData>();

    const onSubmit = (data: ConfirmEmailFormData) => {
        console.log(data);
    }


    const otpSlotClass = "h-12 w-10 text-lg sm:h-14 sm:w-12 sm:text-xl";
    return (
        <>
            <DialogDescription>
                {t("auth.email.description", {email: ""})}
            </DialogDescription>
            <form onSubmit={handleSubmit(onSubmit)}>
                <Input type="hidden" id="email" {...register("email")} value={email}></Input>
                <FieldGroup>
                    <Field>
                        <div className="flex items-center justify-between w-full my-1 gap-2 flex-wrap sm:flex-nowrap ">
                            <FieldLabel htmlFor="otp-verification" className="min-w-0">
                                {t("auth.email.verificationCode")}
                            </FieldLabel>
                            <Button variant="outline" type="button" size="xs">
                                <RefreshCwIcon/>
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
                    </Field>
                    <Field>
                        <Button type="submit" className={cn("w-full", isLoading && "disabled")}>
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
