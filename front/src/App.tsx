import {Route, Routes} from "react-router-dom";
import MainLayout from "@/components/layout/MainLayout.tsx";
import HomePage from "@/pages/HomePage.tsx";
import LoginPage from "@/pages/LoginPage.tsx";
import RegisterPage from "@/pages/RegisterPage.tsx";


function App() {

  return (
    <>
        <Routes>
            <Route path="/" element={<MainLayout />} >
                <Route index element={<HomePage/>} />
            </Route>

            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
        </Routes>
    </>
  )
}

export default App
