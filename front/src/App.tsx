import {Route, Routes} from 'react-router-dom'
import HomePage from "@/pages/HomePage.tsx";
import ProfilePage from "@/pages/ProfilePage.tsx";
import ProfileVideoFeedPage from "@/pages/ProfileVideoFeedPage.tsx";
import MainLayout from "@/components/layout/MainLayout.tsx";
import UploadVideoPage from "@/pages/UploadVideoPage.tsx";
import ProtectedRoute from "@/routes/ProtectedRoute.tsx";

function App() {

    return (
        <>
            <Routes>
                <Route path="/" element={<MainLayout/>}>
                    <Route index element={<HomePage/>}/>
                    <Route path=":username" element={<ProfilePage/>}/>
                    <Route path=":username/video/:videoId" element={<ProfileVideoFeedPage/>}/>

                    <Route element={<ProtectedRoute/>}>
                        <Route path="upload" element={<UploadVideoPage/>}/>
                    </Route>
                </Route>
            </Routes>
        </>
    )
}

export default App
