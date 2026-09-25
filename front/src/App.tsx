import {Route, Routes} from 'react-router-dom'
import HomePage from "@/pages/HomePage.tsx";
import ProfilePage from "@/pages/ProfilePage.tsx";
import ProfileVideoFeedPage from "@/pages/ProfileVideoFeedPage.tsx";
import MainLayout from "@/components/layout/MainLayout.tsx";
import UploadVideoPage from "@/pages/UploadVideoPage.tsx";
import MyVideosPage from "@/pages/MyVideosPage.tsx";
import ProtectedRoute from "@/routes/ProtectedRoute.tsx";
import ResetPasswordPage from "@/pages/ResetPasswordPage.tsx";
import FollowingPage from "@/pages/FollowingPage.tsx";
import SearchPage from "@/pages/SearchPage.tsx";
import MessagesPage from "@/pages/MessagesPage.tsx";
import AdminPage from "@/pages/AdminPage.tsx";
import AdminRoute from "@/routes/AdminRoute.tsx";
import SharedVideoPage from "@/pages/SharedVideoPage.tsx";
import {useVideoProcessingHub} from "@/hooks/useVideoProcessingHub.ts";

function App() {
    useVideoProcessingHub();


    return (
        <>
            <Routes>
                <Route path="/" element={<MainLayout/>}>
                    <Route index element={<HomePage/>}/>
                    <Route path="search" element={<SearchPage/>}/>
                    <Route path="video/:videoId" element={<SharedVideoPage/>}/>
                    <Route path=":username" element={<ProfilePage/>}/>
                    <Route path=":username/video/:videoId" element={<ProfileVideoFeedPage/>}/>
                    <Route path=":username/:collection/video/:videoId" element={<ProfileVideoFeedPage/>}/>
                    <Route path="reset-password" element={<ResetPasswordPage/>}/>

                    <Route element={<ProtectedRoute/>}>
                        <Route path="upload" element={<UploadVideoPage/>}/>
                        <Route path="studio" element={<MyVideosPage/>}/>
                        <Route path="following" element={<FollowingPage/>}/>
                        <Route path="messages" element={<MessagesPage/>}/>
                    </Route>
                    <Route element={<AdminRoute/>}>
                        <Route path="admin" element={<AdminPage/>}/>
                    </Route>
                </Route>
            </Routes>
        </>
    )
}

export default App;
