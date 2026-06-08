import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../components/layout/api";

export default function Login() {
  const navigate = useNavigate(); // 👈 THÊM CÁI NÀY

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleLogin = async () => {
    try {
      const res = await api.post("/Auth/login", {
        email,
        password,
      });

      localStorage.setItem(
        "token",
        res.data.accessToken
      );

      navigate("/"); // 👈 CHUYỂN VỀ HOME

    } catch (err) {
      console.error(err);
      alert("Sai tài khoản hoặc mật khẩu");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100">
      <div className="bg-white w-full max-w-md rounded-xl shadow-lg p-6">
        <img
          src="https://picsum.photos/400/200"
          alt="Login Banner"
          className="w-full h-48 object-cover rounded-lg mb-4"
        />

        <h1 className="text-3xl font-bold mb-6">
          Đăng nhập
        </h1>

        <input
          type="email"
          placeholder="Email"
          className="w-full border p-3 rounded-lg mb-4"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <input
          type="password"
          placeholder="Mật khẩu"
          className="w-full border p-3 rounded-lg mb-4"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button
          onClick={handleLogin}
          className="w-full bg-blue-600 text-white py-3 rounded-lg"
        >
          Đăng nhập
        </button>
      </div>
    </div>
  );
}