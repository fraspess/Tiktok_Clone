import {RefreshCwIcon} from "lucide-react"

import {Button} from "@/components/ui/button"
import {Field, FieldGroup, FieldLabel,} from "@/components/ui/field"
import {InputOTP, InputOTPGroup, InputOTPSeparator, InputOTPSlot,} from "@/components/ui/input-otp"
import {REGEXP_ONLY_DIGITS} from "input-otp";
import {useTranslation} from "react-i18next";
import {DialogDescription} from "@/components/ui/dialog.tsx";

export function ConfirmEmailForm() {
    const {t} = useTranslation();

    return (
        <>
            <DialogDescription>
                {t("auth.email.description", {email: ""})}
            </DialogDescription>
            <form>
                <FieldGroup>
                    <Field>
                        <div className="flex items-center justify-between w-fit mx-auto">
                            <FieldLabel htmlFor="otp-verification">
                                {t("auth.email.verificationCode")}
                            </FieldLabel>
                            <Button variant="outline" type="button" size="xs">
                                <RefreshCwIcon/>
                                {t("auth.email.resendCode")}
                            </Button>
                        </div>

                        <div className="flex justify-center">
                            <InputOTP maxLength={6} id="otp-verification" required pattern={REGEXP_ONLY_DIGITS}>
                                <InputOTPGroup
                                    className="*:data-[slot=input-otp-slot]:h-12 *:data-[slot=input-otp-slot]:w-11 *:data-[slot=input-otp-slot]:text-xl">
                                    <InputOTPSlot index={0}/>
                                    <InputOTPSlot index={1}/>
                                    <InputOTPSlot index={2}/>
                                </InputOTPGroup>
                                <InputOTPSeparator className="mx-2"/>
                                <InputOTPGroup
                                    className="*:data-[slot=input-otp-slot]:h-12 *:data-[slot=input-otp-slot]:w-11 *:data-[slot=input-otp-slot]:text-xl">
                                    <InputOTPSlot index={3}/>
                                    <InputOTPSlot index={4}/>
                                    <InputOTPSlot index={5}/>
                                </InputOTPGroup>
                            </InputOTP>
                        </div>
                    </Field>
                    <Field>
                        <Button type="submit" className="w-full">
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
