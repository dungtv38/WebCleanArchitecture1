
import { createBrowserRouter, Navigate } from "react-router-dom"; 

import Home from "@/pages/Home";
import { SignupForm } from "@/components/signup-form";
import Login from "@/pages/Login";
import HotelDetail from "@/pages/HotelDetail";
import Booking from "@/pages/Booking";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Navigate to="/home" replace />, 
  },
  {
    path: "/home",
    element: <Home />,
  },
  {
    path: "/login",
    element: <Login />,
  },
  {
    path: "/register",
    element: <SignupForm />,
    
  },
  {
    path: "/Hotel/:id", // 2. Thêm dấu :id để React Router hiểu đây là tham số động (Guid)
    element: <HotelDetail />,
  },
   {
  path: "/booking",
  element: <Booking />,
},
  
]);