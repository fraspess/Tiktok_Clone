import {Route, Routes} from 'react-router-dom'
import HomePage from "@/pages/HomePage.tsx";
import MainLayout from "@/components/layout/MainLayout.tsx";

function App() {

  return (
    <>
      <Routes>
        <Route path="/" element={<MainLayout />} >
          <Route index element={<HomePage/>} />
        </Route>

      </Routes>
    </>
  )
}

export default App
