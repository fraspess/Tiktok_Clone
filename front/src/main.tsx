import "./i18n.ts"
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {ThemeProvider} from "next-themes";
import {Provider} from "react-redux";
import {store} from "@/store/store.ts";
import {BrowserRouter} from "react-router-dom";
import {TooltipProvider} from "@/components/ui/tooltip.tsx";
import {GoogleOAuthProvider} from "@react-oauth/google";
import {GOOGLE_CLIENT_ID} from "@/env.ts";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <Provider store={store}>
            <BrowserRouter>
                <ThemeProvider attribute="class" defaultTheme="system" disableTransitionOnChange enableSystem>
                    <TooltipProvider>
                        <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
                            <App/>
                        </GoogleOAuthProvider>
                    </TooltipProvider>
                </ThemeProvider>
            </BrowserRouter>
        </Provider>
    </StrictMode>
)
