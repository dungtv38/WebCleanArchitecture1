import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "sonner";

import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { register } from "@/services/AuthService";

export function SignupForm() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    password: "",
    confirmPassword: "",
  });

  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (
      !form.fullName ||
      !form.email ||
      !form.phoneNumber ||
      !form.password ||
      !form.confirmPassword
    ) {
      toast.error("Vui lòng nhập đầy đủ thông tin.");
      return;
    }

    if (form.password !== form.confirmPassword) {
      toast.error("Mật khẩu xác nhận không khớp.");
      return;
    }

    try {
      setLoading(true);

      await register({
        fullName: form.fullName,
        email: form.email,
        password: form.password,
        phoneNumber: form.phoneNumber,
      });

      toast.success("🎉 Tạo tài khoản thành công!");

      setTimeout(() => {
        navigate("/signin");
      }, 1500);
    } catch (error: any) {
      toast.error(
        error.response?.data?.message ||
          error.response?.data ||
          "Đăng ký thất bại."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card className="w-full max-w-lg shadow-xl rounded-2xl">
      <CardContent className="p-8">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold">Create Account</h1>

          <p className="text-gray-500 mt-2">
            Create an account to book your favorite hotel.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label className="font-medium">Full Name</label>

            <Input
              name="fullName"
              value={form.fullName}
              onChange={handleChange}
              placeholder="Nguyễn Văn A"
            />
          </div>

          <div>
            <label className="font-medium">Email</label>

            <Input
              name="email"
              type="email"
              value={form.email}
              onChange={handleChange}
              placeholder="example@gmail.com"
            />
          </div>

          <div>
            <label className="font-medium">Phone Number</label>

            <Input
              name="phoneNumber"
              value={form.phoneNumber}
              onChange={handleChange}
              placeholder="0987654321"
            />
          </div>

          <div>
            <label className="font-medium">Password</label>

            <Input
              name="password"
              type="password"
              value={form.password}
              onChange={handleChange}
            />
          </div>

          <div>
            <label className="font-medium">Confirm Password</label>

            <Input
              name="confirmPassword"
              type="password"
              value={form.confirmPassword}
              onChange={handleChange}
            />
          </div>

          <Button
            className="w-full h-11"
            type="submit"
            disabled={loading}
          >
            {loading ? "Creating..." : "Create Account"}
          </Button>
        </form>

        <div className="text-center mt-6">
          <span className="text-gray-500">
            Already have an account?
          </span>

          <Link
            to="/signin"
            className="text-blue-600 ml-2 hover:underline"
          >
            Sign In
          </Link>
        </div>
      </CardContent>
    </Card>
  );
}