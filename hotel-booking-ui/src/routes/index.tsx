import { createBrowserRouter } from "react-router-dom";

import Home from "@/pages/Home";


import { SignupForm } from "@/components/signup-form";
import Login from "@/pages/Login";

export const router = createBrowserRouter([
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
]);