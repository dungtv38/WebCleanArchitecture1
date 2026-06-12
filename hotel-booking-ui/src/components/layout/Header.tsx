import { useNavigate, Link } from "react-router-dom";
import { useState } from "react";

export default function Header() {
    const navigate = useNavigate();
    const [showDropdown, setShowDropdown] = useState(false);
    
    // Giả sử bạn lưu tên người dùng trong localStorage lúc đăng nhập
    const username = localStorage.getItem("username") || "Khách hàng"; 

    const handleLogout = () => {
        localStorage.clear(); // Xóa token và dữ liệu user
        navigate("/login");
    };

    return (
        <header className="bg-blue-600 text-white shadow-md">
            <div className="max-w-6xl mx-auto px-4 h-16 flex justify-between items-center">
                {/* Logo */}
                <Link to="/home" className="text-xl font-black tracking-wider">
                    Hotel Booking
                </Link>

                {/* Khu vực góc phải điều hướng */}
                <div className="flex items-center gap-4 relative">
                    <Link to="/home" className="text-sm font-semibold hover:text-blue-200 transition">
                        Trang chủ
                    </Link>

                    {/* Menu Tài khoản góc phải */}
                    <div className="relative">
                        <button 
                            onClick={() => setShowDropdown(!showDropdown)}
                            className="bg-white/10 hover:bg-white/20 transition px-4 py-2 rounded-xl text-sm font-bold flex items-center gap-2 border border-white/20"
                        >
                            👤 {username} ▼
                        </button>

                        {/* Dropdown Menu hiện ra khi nhấn vào tên */}
                        {showDropdown && (
                            <div className="absolute right-0 mt-2 w-48 bg-white rounded-2xl shadow-xl border border-slate-100 py-2 z-50 animate-in fade-in slide-in-from-top-1">
                                <Link 
                                    to="/customer/profile" // Trang quản lý chung (Hồ sơ + Lịch sử)
                                    onClick={() => setShowDropdown(false)}
                                    className="block px-4 py-2.5 text-sm text-gray-700 hover:bg-slate-50 font-medium"
                                >
                                    ℹ️ Hồ sơ của tôi
                                </Link>
                               
                                <hr className="my-1 border-slate-100" />
                                <button
                                    onClick={handleLogout}
                                    className="w-full text-left px-4 py-2.5 text-sm text-red-600 hover:bg-red-50 font-bold"
                                >
                                    🚪 Đăng xuất
                                </button>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </header>
    );
}