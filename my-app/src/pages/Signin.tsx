import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "sonner";

import { login } from "@/services/AuthService";

import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export default function Signin() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);

  const [form, setForm] = useState({
    email: "",
    password: "",
  });

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (
    e: React.FormEvent
  ) => {
    e.preventDefault();

    try {
      setLoading(true);

      const data = await login(form);

      console.log("Login response:", data);

      localStorage.setItem("token", data.token);

      if (data.user) {
        localStorage.setItem(
          "user",
          JSON.stringify(data.user)
        );
      } else {
        localStorage.removeItem("user");
      }

      toast.success("Đăng nhập thành công");

      navigate("/");
    } catch (err: any) {
      console.log(err.response);

      const message =
        err.response?.data?.message ||
        err.response?.data ||
        "Đăng nhập thất bại.";

      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-100 flex items-center justify-center">
      <Card className="w-full max-w-md">
        <CardContent className="p-8">
          <h1 className="text-3xl font-bold text-center">
            Sign In
          </h1>

          <p className="text-center text-gray-500 mb-6">
            Login to continue booking.
          </p>

          <form
            onSubmit={handleSubmit}
            className="space-y-5"
          >
            <div>
              <label>Email</label>

              <Input
                name="email"
                type="email"
                value={form.email}
                onChange={handleChange}
                required
              />
            </div>

            <div>
              <label>Password</label>

              <Input
                name="password"
                type="password"
                value={form.password}
                onChange={handleChange}
                required
              />
            </div>

            <Button
              type="submit"
              className="w-full"
              disabled={loading}
            >
              {loading ? "Signing..." : "Sign In"}
            </Button>
          </form>

          <div className="text-center mt-5">
            Don't have an account?

            <Link
              to="/signup"
              className="text-blue-600 ml-2"
            >
              Create account
            </Link>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}