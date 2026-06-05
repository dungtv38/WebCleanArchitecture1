import { useState } from "react";
import api from "../components/layout/api";

export default function Register() {
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    password: "",
    phoneNumber: "",
    role: "Customer",
  });

  const handleRegister = async () => {
    try {
      const res = await api.post("/Auth/Register", form);

      localStorage.setItem("token", res.data.token);

      alert("Đăng ký thành công");
    } catch (err) {
      console.error(err);
      alert("Đăng ký thất bại");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100">
      <div className="bg-white w-full max-w-md rounded-xl shadow-lg p-6">
        <h1 className="text-3xl font-bold mb-6">
          Đăng ký
        </h1>

        <input
          placeholder="Họ tên"
          className="w-full border p-3 rounded-lg mb-3"
          onChange={(e) =>
            setForm({
              ...form,
              fullName: e.target.value,
            })
          }
        />

        <input
          placeholder="Email"
          className="w-full border p-3 rounded-lg mb-3"
          onChange={(e) =>
            setForm({
              ...form,
              email: e.target.value,
            })
          }
        />

        <input
          placeholder="Số điện thoại"
          className="w-full border p-3 rounded-lg mb-3"
          onChange={(e) =>
            setForm({
              ...form,
              phoneNumber: e.target.value,
            })
          }
        />

        <input
          type="password"
          placeholder="Mật khẩu"
          className="w-full border p-3 rounded-lg mb-4"
          onChange={(e) =>
            setForm({
              ...form,
              password: e.target.value,
            })
          }
        />


        <button
          onClick={handleRegister}
          className="w-full bg-green-600 text-white py-3 rounded-lg"
        >
          Đăng ký
        </button>
      </div>
    </div>
  );
}