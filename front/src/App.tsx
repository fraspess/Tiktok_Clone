import {Route, Routes} from 'react-router-dom'
import HomePage from "@/pages/HomePage.tsx";
import ProfilePage from "@/pages/ProfilePage.tsx";
import ProfileVideoFeedPage from "@/pages/ProfileVideoFeedPage.tsx";
import MainLayout from "@/components/layout/MainLayout.tsx";
import UploadVideoPage from "@/pages/UploadVideoPage.tsx";
import ProtectedRoute from "@/routes/ProtectedRoute.tsx";
import ResetPasswordPage from "@/pages/ResetPasswordPage.tsx";
import FollowingPage from "@/pages/FollowingPage.tsx";
import SearchPage from "@/pages/SearchPage.tsx";
import MessagesPage from "@/pages/MessagesPage.tsx";

function App() {

    return (
        <>
            <Routes>
                <Route path="/" element={<MainLayout/>}>
                    <Route index element={<HomePage/>}/>
                    <Route path="search" element={<SearchPage/>}/>
                    <Route path=":username" element={<ProfilePage/>}/>
                    <Route path=":username/video/:videoId" element={<ProfileVideoFeedPage/>}/>
                    <Route path="reset-password" element={<ResetPasswordPage/>}/>

                    <Route element={<ProtectedRoute/>}>
                        <Route path="upload" element={<UploadVideoPage/>}/>
                        <Route path="following" element={<FollowingPage/>}/>
                        <Route path="messages" element={<MessagesPage/>}/>
                    </Route>
                </Route>
            </Routes>
        </>
    )
}

export default App
