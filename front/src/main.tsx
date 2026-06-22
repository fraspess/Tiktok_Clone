import './i18n'
import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {Provider} from "react-redux";
import {store} from "@/store/store.ts";
import {BrowserRouter} from "react-router-dom";
import {ThemeProvider} from "next-themes";

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

