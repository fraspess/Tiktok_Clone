import {useState} from "react";
import {Dialog, DialogContent, DialogHeader, DialogTitle} from "@/components/ui/dialog.tsx";
import {useAppDispatch, useAppSelector} from "@/store/hooks.ts";
import {useTranslation} from "react-i18next";
import SignInForm from "@/components/modals/AuthModal/SignInForm.tsx";
import {closeModal} from "@/store/slices/authModalSlice.ts";
import SignUpForm from "@/components/modals/AuthModal/SignUpForm.tsx";
import ConfirmEmailForm from "@/components/modals/AuthModal/ConfirmEmailForm.tsx";

const AuthModal = () => {
    const [step, setStep] = useState<string>("signIn");
    const isOpened = useAppSelector(state => state.authModal.isOpened)
    const {t} = useTranslation();
    const dispatch = useAppDispatch();
    const [currentConfirmEmail, setCurrentConfirmEmail] = useState<string>("");
    const handleClose = () => {
        dispatch(closeModal());
        setStep("signIn");
    }

    const onSwitchToSignUp = () => {
        setStep("signUp");
    }

    const onSwitchToSignIn = () => {
        setStep("signIn");
    }

    const onUnConfirmedEmail = (email: string) => {
        setCurrentConfirmEmail(email);
        setStep("confirmEmail");
    }

    return (
        <Dialog open={isOpened} onOpenChange={handleClose}>
            <DialogContent onInteractOutside={(e) => {
                e.preventDefault()
            }} className="sm:max-w-sm max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle className="text-4xl">
                        {step === "signIn" && t("auth.signInTitle")}
                        {step === "signUp" && t("auth.signUpTitle")}
                        {step === "confirmEmail" && t("auth.confirmEmailTitle")}
                    </DialogTitle>
                </DialogHeader>

                {step === "signIn" && (
                    <SignInForm onSwitchToSignUp={onSwitchToSignUp} onUnConfirmedEmail={onUnConfirmedEmail}
                                onSuccess={() => {
                                    dispatch(closeModal())
                                }}/>
                )}
                {step === "signUp" && (
                    <SignUpForm onSwitchToSignIn={onSwitchToSignIn} onSuccess={(email) => {
                        setCurrentConfirmEmail(email);
                        setStep("confirmEmail");
                    }}/>
                )}
                {step === "confirmEmail" && (
                    <ConfirmEmailForm onSuccess={onSwitchToSignIn} email={currentConfirmEmail}/>
                )}
            </DialogContent>
        </Dialog>
    )
}

export default AuthModal;