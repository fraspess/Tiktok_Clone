import "./i18n.ts"
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {ThemeProvider} from "next-themes";
import {Provider} from "react-redux";
import {store} from "@/store/store.ts";
import {BrowserRouter} from "react-router-dom";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <Provider store={store}>
            <BrowserRouter>
                <ThemeProvider attribute="class" defaultTheme="dark">
                    <App/>
                </ThemeProvider>
            </BrowserRouter>
        </Provider>
    </StrictMode>
)
