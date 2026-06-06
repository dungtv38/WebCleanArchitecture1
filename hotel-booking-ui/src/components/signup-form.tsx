"use client";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import api from "./layout/api";
import { useNavigate } from "react-router-dom";

const signUpSchema = z.object({
  fullName: z.string().min(1, "Họ tên bắt buộc"),
  email: z.string().email("Email không hợp lệ"),
  password: z.string().min(6, "Mật khẩu ít nhất 6 ký tự"),
  phoneNumber: z.string().min(10, "Số điện thoại không hợp lệ"),
});

type SignUpFormValues = z.infer<typeof signUpSchema>;

export function SignupForm({
  className,
  ...props
}: React.ComponentProps<"div">) {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SignUpFormValues>({
    resolver: zodResolver(signUpSchema),
  });

  const onSubmit = async (data: SignUpFormValues) => {
    try {
      await api.post("/Auth/Register", data);

      alert("Đăng ký thành công");
      navigate("/home");
    } catch (err) {
      console.error(err);
      alert("Đăng ký thất bại");
    }
  };

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card className="overflow-hidden p-0">
        <CardContent className="grid p-0 md:grid-cols-2">

          {/* FORM */}
          <form className="p-6 md:p-8" onSubmit={handleSubmit(onSubmit)}>
            <div className="flex flex-col gap-5">

              {/* TITLE */}
              <div className="text-center">
                <h1 className="text-2xl font-bold">Đăng ký tài khoản</h1>
                <p className="text-muted-foreground">
                  Tạo tài khoản để bắt đầu sử dụng
                </p>
              </div>

              {/* FULL NAME */}
              <div className="space-y-2">
                <Label htmlFor="fullName">Họ tên</Label>
                <Input id="fullName" {...register("fullName")} />
                {errors.fullName && (
                  <p className="text-red-500 text-sm">
                    {errors.fullName.message}
                  </p>
                )}
              </div>

              {/* EMAIL */}
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input id="email" type="email" {...register("email")} />
                {errors.email && (
                  <p className="text-red-500 text-sm">
                    {errors.email.message}
                  </p>
                )}
              </div>

              {/* PHONE */}
              <div className="space-y-2">
                <Label htmlFor="phoneNumber">Số điện thoại</Label>
                <Input id="phoneNumber" {...register("phoneNumber")} />
                {errors.phoneNumber && (
                  <p className="text-red-500 text-sm">
                    {errors.phoneNumber.message}
                  </p>
                )}
              </div>

              {/* PASSWORD */}
              <div className="space-y-2">
                <Label htmlFor="password">Mật khẩu</Label>
                <Input
                  id="password"
                  type="password"
                  {...register("password")}
                />
                {errors.password && (
                  <p className="text-red-500 text-sm">
                    {errors.password.message}
                  </p>
                )}
              </div>

              {/* BUTTON */}
              <Button type="submit" disabled={isSubmitting} className="w-full">
                {isSubmitting ? "Đang đăng ký..." : "Đăng ký"}
              </Button>

              {/* LOGIN */}
              <p className="text-center text-sm">
                Đã có tài khoản?{" "}
                <a href="/signin" className="underline">
                  Đăng nhập
                </a>
              </p>
            </div>
          </form>

          {/* IMAGE */}
          <div className="relative hidden md:block bg-muted">
            <img
              src="/placeholderSignUp.png"
              className="absolute inset-0 h-full w-full object-cover"
              alt="signup"
            />
          </div>

        </CardContent>
      </Card>
    </div>
  );
}