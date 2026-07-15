import { BrowserRouter, Route, Routes } from "react-router-dom";
import { Toaster } from "sonner";

import Signup from "./pages/Signup";
import Signin from "./pages/Signin";
import HomePage from "./pages/HomePage";
import HotelDetail from "./pages/HotelDetail";
import RoomDetail from "./pages/RoomDetail";

function App() {
  return (
    <>
      <Toaster richColors />

      <BrowserRouter>
        <Routes>
          <Route path="/" element={<HomePage />} />

          <Route path="/hotel/:id" element={<HotelDetail />} />
          <Route path="/room/:id" element={<RoomDetail />}/>

          <Route path="/signin" element={<Signin />} />
          <Route path="/signup" element={<Signup />} />
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;