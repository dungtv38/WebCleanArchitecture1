import { useState, useEffect } from "react";
import api from "../components/layout/api";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";

// Định nghĩa dữ liệu Lịch sử đặt phòng
interface BookingHistoryItem {
    id: string;
    checkIn: string;
    checkOut: string;
    totalAmount: number;
    status: string;
}

// Định nghĩa dữ liệu Hồ sơ cá nhân
interface UserProfile {
    fullName: string;
    email: string;
    phoneNumber: string;
}

export default function CustomerDashboard() {
    const [activeTab, setActiveTab] = useState<"profile" | "bookings">("profile");
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [bookings, setBookings] = useState<BookingHistoryItem[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchDashboardData = async () => {
            try {
                setLoading(true);
                // 1. Gọi API lấy thông tin cá nhân (Nếu bạn đã có endpoint này)
                const profileRes = await api.get<UserProfile>("/Auth/profile").catch(() => null);
                if (profileRes) setProfile(profileRes.data);

                // 2. Gọi API lấy lịch sử đặt phòng đã viết ở BookingController
                const bookingsRes = await api.get<BookingHistoryItem[]>("/booking/my-history");
                setBookings(bookingsRes.data);
            } catch (err) {
                console.error("Lỗi tải dữ liệu Dashboard:", err);
            } finally {
                setLoading(false);
            }
        };
        fetchDashboardData();
    }, []);

    const formatDate = (dateStr: string) => {
        const d = new Date(dateStr);
        return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`;
    };

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    return (
        <>
            <Header />
            <main className="max-w-5xl mx-auto px-4 py-10 min-h-screen grid grid-cols-1 md:grid-cols-4 gap-8">
                
                {/* 1. THANH SIDEBAR BÊN TRÁI */}
                <div className="md:col-span-1 space-y-2">
                    <div className="bg-white border border-slate-200 rounded-2xl p-4 text-center shadow-sm">
                        <div className="w-16 h-16 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center text-2xl font-bold mx-auto mb-3">
                            {profile?.fullName?.substring(0, 1) || "U"}
                        </div>
                        <h3 className="font-bold text-gray-800">{profile?.fullName || "Người dùng"}</h3>
                        <p className="text-xs text-gray-400">{profile?.email}</p>
                    </div>

                    <div className="bg-white border border-slate-200 rounded-2xl p-2 space-y-1 shadow-sm">
                        <button
                            onClick={() => setActiveTab("profile")}
                            className={`w-full text-left px-4 py-2.5 rounded-xl text-sm font-bold transition ${
                                activeTab === "profile" ? "bg-blue-50 text-blue-600" : "text-gray-600 hover:bg-slate-50"
                            }`}
                        >
                            👤 Hồ sơ cá nhân
                        </button>
                        <button
                            onClick={() => setActiveTab("bookings")}
                            className={`w-full text-left px-4 py-2.5 rounded-xl text-sm font-bold transition ${
                                activeTab === "bookings" ? "bg-blue-50 text-blue-600" : "text-gray-600 hover:bg-slate-50"
                            }`}
                        >
                            🧳 Lịch sử đặt phòng
                        </button>
                    </div>
                </div>

                {/* 2. KHU VỰC HIỂN THỊ NỘI DUNG CHI TIẾT BÊN PHẢI */}
                <div className="md:col-span-3 bg-white border border-slate-200 rounded-2xl p-6 shadow-sm">
                    {activeTab === "profile" ? (
                        /* Giao diện Tab Hồ Sơ */
                        <div className="space-y-6">
                            <h2 className="text-xl font-bold text-gray-800 border-b pb-3">ℹ️ Thông tin cá nhân</h2>
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                <div>
                                    <label className="text-xs font-bold text-gray-400 uppercase block mb-1">Họ và tên</label>
                                    <input type="text" readOnly value={profile?.fullName || "Nguyễn Văn A"} className="w-full border px-4 py-2.5 rounded-xl bg-slate-50 text-gray-700 outline-none" />
                                </div>
                                <div>
                                    <label className="text-xs font-bold text-gray-400 uppercase block mb-1">Địa chỉ Email</label>
                                    <input type="email" readOnly value={profile?.email || "user@gmail.com"} className="w-full border px-4 py-2.5 rounded-xl bg-slate-50 text-gray-700 outline-none" />
                                </div>
                                <div>
                                    <label className="text-xs font-bold text-gray-400 uppercase block mb-1">Số điện thoại</label>
                                    <input type="text" readOnly value={profile?.phoneNumber || "0987654321"} className="w-full border px-4 py-2.5 rounded-xl bg-slate-50 text-gray-700 outline-none" />
                                </div>
                            </div>
                        </div>
                    ) : (
                      
                        <div className="space-y-4">
                            <h2 className="text-xl font-bold text-gray-800 border-b pb-3">🧳 Danh sách phòng đã đặt</h2>
                            {bookings.length === 0 ? (
                                <p className="text-center py-10 text-gray-400 text-sm">Bạn chưa có lịch trình đặt phòng nào.</p>
                            ) : (
                                <div className="space-y-3">
                                    {bookings.map((b) => (
                                        <div key={b.id} className="border p-4 rounded-xl flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3 bg-slate-50/50">
                                            <div>
                                                <p className="font-mono text-xs font-bold text-slate-500">Mã đơn: #{b.id.substring(0, 8).toUpperCase()}</p>
                                                <p className="text-sm font-semibold text-gray-700 mt-1">
                                                    📅 Lưu trú: {formatDate(b.checkIn)} - {formatDate(b.checkOut)}
                                                </p>
                                            </div>
                                            <div className="flex sm:flex-col items-end gap-2 sm:gap-1 w-full sm:w-auto justify-between">
                                                <span className="text-sm font-black text-blue-600">{(b.totalAmount || 0).toLocaleString("vi-VN")} đ</span>
                                                <span className={`px-3 py-1 rounded-full text-xs font-bold ${
                                                    b.status === "Confirmed" ? "bg-green-100 text-green-700" : "bg-yellow-100 text-yellow-700"
                                                }`}>
                                                    {b.status === "Confirmed" ? "Đã xác nhận" : "Chờ xử lý"}
                                                </span>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    )}
                </div>

            </main>
            <Footer />
        </>
    );
}