import axiosClient from "./axiosClient";

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
}

export const register = async (data: RegisterRequest) => {
  const response = await axiosClient.post("/Auth/register", data);
  return response.data;
};

export interface LoginResponse {
    token: string;
    user: {
        id: string;
        fullName: string;
        email: string;
        phoneNumber: string;
        role: string;
    };
}

export interface LoginRequest {
  email: string;
  password: string;
}

export const login = async (data: LoginRequest) => {
  const response = await axiosClient.post("/Auth/login", data);
  return response.data;
};

export const logout = () => {

    localStorage.removeItem("token");
    localStorage.removeItem("user");

};