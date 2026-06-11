import { useEffect, useState, useMemo } from "react"; 
import { useSearchParams, useNavigate } from "react-router-dom";

import api from "../components/layout/api";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";

// Định nghĩa Interface dữ liệu phòng nhận về từ API
interface RoomDetailDTO {
    id: string;
    roomNumber: string;
    pricePerNight: number;
    maxGuests: number;
    note?: string;
}

export default function Booking() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    
    // Đọc tham số URL chấp nhận cả viết hoa lẫn viết thường để tránh bị rỗng (null) dữ liệu
    const hotelId = searchParams.get("hotelId") || searchParams.get("HotelId");
    const roomId = searchParams.get("roomId") || searchParams.get("RoomId");

    // Các State quản lý dữ liệu giao diện
    const [roomInfo, setRoomInfo] = useState<RoomDetailDTO | null>(null);
    const [loading, setLoading] = useState(true);
    const [checkIn, setCheckIn] = useState("");
    const [checkOut, setCheckOut] = useState("");
    const [errorMessage, setErrorMessage] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    // 1. Gọi API lấy chi tiết căn phòng cụ thể
    useEffect(() => {
        const fetchRoomDetail = async () => {
            if (!roomId) {
                setErrorMessage("⚠️ Không tìm thấy mã phòng (roomId) trên URL.");
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                setErrorMessage(""); 

                // Gọi API lấy thông tin phòng chi tiết
                const response = await api.get<RoomDetailDTO>(`/Rooms/type/${roomId}`);
                console.log("Dữ liệu thực tế nhận về từ API:", response.data);

                if (response.data) {
                    setRoomInfo(response.data);
                } else {
                    setErrorMessage("⚠️ Không có dữ liệu trả về cho phòng này.");
                }
            } catch (error: any) {
                console.error("Lỗi API Rooms:", error);
                const serverMessage = error.response?.data?.message || error.message;
                setErrorMessage(`❌ Không thể tải thông tin phòng. Lỗi: ${serverMessage}`);
            } finally {
                setLoading(false);
            }
        };

        fetchRoomDetail();
    }, [roomId]);

    // 2. Logic tính toán số đêm lưu trú (Dựa trên ngày khách chọn)
    const totalNights = useMemo(() => {
        if (!checkIn || !checkOut) return 0;
        const date1 = new Date(checkIn);
        const date2 = new Date(checkOut);
        
        // Trừ mili-giây và đổi ra số ngày lưu trú
        const diffTime = date2.getTime() - date1.getTime();
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        
        return diffDays > 0 ? diffDays : 0;
    }, [checkIn, checkOut]);

    // 3. Tính tổng tiền hóa đơn tạm tính
    const totalAmount = roomInfo ? roomInfo.pricePerNight * totalNights : 0;

    // 4. Hàm xử lý gửi Đơn đặt phòng lên Backend
    const handleConfirmBooking = async () => {
        setErrorMessage("");
        
        if (!hotelId || !roomId) {
            setErrorMessage("❌ Lỗi hệ thống: Thiếu mã khách sạn hoặc mã phòng trên đường dẫn URL.");
            return;
        }

        if (!checkIn || !checkOut) {
            setErrorMessage("⚠️ Vui lòng chọn đầy đủ ngày nhận và ngày trả phòng.");
            return;
        }

        if (totalNights <= 0) {
            setErrorMessage("⚠️ Ngày trả phòng phải sau ngày nhận phòng ít nhất 1 ngày.");
            return;
        }

        setIsSubmitting(true);
        try {
            // Truyền chuỗi ngày YYYY-MM-DD trực tiếp, không bọc .toISOString() để tránh lệch múi giờ
            const requestBody = {
                hotelId: hotelId,
                roomIds: [roomId], 
                checkIn: checkIn,
                checkOut: checkOut
            };

            console.log("Dữ liệu UI gửi lên API đặt phòng:", requestBody);

            // Gửi dữ liệu lên API endpoint
            const response = await api.post("/booking", requestBody);
            
            alert("🎉 Đặt phòng thành công! Hệ thống đang chuyển hướng tới trang thanh toán.");
            
            // Điều hướng sang trang thanh toán dựa trên ID đơn hàng trả về từ Backend
            navigate(`/payment/${response.data.id}`); 
        } catch (error: any) {
            console.error("Lỗi đặt phòng từ giao diện:", error);
            
            // Bắt câu báo lỗi chi tiết từ backend (như lỗi CORS, 401 hoặc lỗi trùng lịch)
            const backendMessage = error.response?.data?.message;
            if (error.response?.status === 401) {
                setErrorMessage("❌ Lỗi 401: Bạn chưa đăng nhập hoặc Token đã hết hạn. Vui lòng đăng nhập lại.");
            } else {
                setErrorMessage(backendMessage || "Đã xảy ra lỗi kết nối với hệ thống Backend (CORS/Network Error).");
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    // Lấy ngày hiện tại định dạng YYYY-MM-DD để chặn khách chọn ngày trong quá khứ trên giao diện lịch
    const todayStr = useMemo(() => {
        const d = new Date();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${d.getFullYear()}-${month}-${day}`;
    }, []);

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
            <main className="max-w-4xl mx-auto px-4 py-10 min-h-screen grid grid-cols-1 md:grid-cols-3 gap-8">
                
                {/* CỘT TRÁI (Khối chiếm 2/3): Form chọn ngày tháng */}
                <div className="md:col-span-2 space-y-6">
                    <div className="bg-white border rounded-2xl p-6 shadow-sm">
                        <h2 className="text-xl font-bold text-gray-800 mb-6 flex items-center gap-2">
                            📅 Chọn thời gian lưu trú
                        </h2>
                        
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            <div>
                                <label className="block text-xs font-bold text-gray-500 uppercase mb-2">Ngày nhận phòng (Check-in)</label>
                                <input 
                                    type="date" 
                                    min={todayStr}
                                    value={checkIn} 
                                    onChange={(e) => setCheckIn(e.target.value)} 
                                    className="w-full border border-slate-200 rounded-xl p-3 font-medium focus:outline-none focus:border-blue-500 transition" 
                                />
                            </div>
                            <div>
                                <label className="block text-xs font-bold text-gray-500 uppercase mb-2">Ngày trả phòng (Check-out)</label>
                                <input 
                                    type="date" 
                                    min={checkIn || todayStr}
                                    value={checkOut} 
                                    onChange={(e) => setCheckOut(e.target.value)} 
                                    className="w-full border border-slate-200 rounded-xl p-3 font-medium focus:outline-none focus:border-blue-500 transition" 
                                />
                            </div>
                        </div>

                        {/* Khối hiển thị thông báo lỗi nếu có */}
                        {errorMessage && (
                            <div className="mt-4 p-3 bg-red-50 text-red-600 text-sm font-medium rounded-xl border border-red-100">
                                {errorMessage}
                            </div>
                        )}
                    </div>

                    {/* Khối chính sách lưu ý */}
                    <div className="bg-slate-50 rounded-2xl p-6 border text-sm text-gray-600 space-y-2">
                        <h4 className="font-bold text-gray-700">📌 Lưu ý quy định khách sạn:</h4>
                        <p>• Thời gian nhận phòng tiêu chuẩn: 14:00. Thời gian trả phòng: 12:00 trưa.</p>
                        <p>• Vui lòng xuất trình CCCD hoặc Hộ chiếu khi làm thủ tục check-in tại quầy lễ tân.</p>
                    </div>
                </div>

                {/* CỘT PHẢI (Khối chiếm 1/3): Tóm tắt hóa đơn và thông tin phòng */}
                <div className="md:col-span-1">
                    <div className="bg-white border rounded-2xl p-6 shadow-sm sticky top-6 space-y-6">
                        <h3 className="text-lg font-bold text-gray-800 border-b pb-3">Tóm tắt đơn hàng</h3>
                        
                        {roomInfo && (
                            <div className="space-y-3 text-sm">
                                <div className="flex justify-between">
                                    <span className="text-gray-500">Mã số phòng:</span>
                                    <span className="font-bold text-gray-800">Phòng {roomInfo.roomNumber}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-500">Sức chứa:</span>
                                    <span className="font-medium text-gray-700">👥 Tối đa {roomInfo.maxGuests} khách</span>
                                </div>
                                {roomInfo.note && (
                                    <p className="text-xs text-amber-600 italic bg-amber-50 p-2 rounded-lg">
                                        ℹ️ Lưu ý: {roomInfo.note}
                                    </p>
                                )}
                                
                                <hr className="my-4 border-dashed" />

                                <div className="flex justify-between">
                                    <span className="text-gray-500">Giá phòng lẻ:</span>
                                    <span className="font-semibold text-gray-800">
                                        {roomInfo.pricePerNight.toLocaleString("vi-VN")} đ / đêm
                                    </span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-500">Số đêm ở:</span>
                                    <span className="font-bold text-blue-600">{totalNights} đêm</span>
                                </div>

                                <div className="bg-orange-50 border border-orange-100 rounded-xl p-4 mt-4 flex justify-between items-center">
                                    <span className="text-sm font-bold text-gray-700">Tổng thanh toán:</span>
                                    <span className="text-xl font-black text-orange-600">
                                        {totalAmount.toLocaleString("vi-VN")} đ
                                    </span>
                                </div>
                            </div>
                        )}

                        <button 
                            onClick={handleConfirmBooking}
                            disabled={isSubmitting}
                            className={`w-full text-white font-bold py-3.5 rounded-xl transition shadow-md text-center ${
                                isSubmitting 
                                ? "bg-blue-400 cursor-not-allowed" 
                                : "bg-blue-600 hover:bg-blue-700 active:scale-[0.99]"
                            }`}
                        >
                            {isSubmitting ? "Đang xử lý..." : "Xác nhận đặt ngay"}
                        </button>
                    </div>
                </div>

            </main>
            <Footer />
        </>
    );
}