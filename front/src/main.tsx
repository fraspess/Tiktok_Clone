import "./i18n.ts"
import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {ThemeProvider} from "next-themes";
import {Provider} from "react-redux";
import {store} from "@/store/store.ts";
import {BrowserRouter} from "react-router-dom";
import {TooltipProvider} from "@/components/ui/tooltip.tsx";
import {GoogleOAuthProvider} from "@react-oauth/google";
import {GOOGLE_CLIENT_ID} from "@/env.ts";
import AuthBootstrap from "@/components/auth/AuthBootstrap.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <Provider store={store}>
            <ThemeProvider attribute="class" defaultTheme="system" disableTransitionOnChange enableSystem>
                <TooltipProvider>
                    <BrowserRouter>
                        <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
                            <AuthBootstrap>
                                <App/>
                            </AuthBootstrap>
                        </GoogleOAuthProvider>
                    </BrowserRouter>
                </TooltipProvider>
            </ThemeProvider>
        </Provider>
    </StrictMode>
)
