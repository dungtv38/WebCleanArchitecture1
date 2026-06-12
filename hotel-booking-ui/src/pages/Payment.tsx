import { useEffect, useState, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../components/layout/api";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";

interface BookingPaymentDetail {
    id: string;
    hotelId: string;
    checkIn: string;
    checkOut: string;
    totalAmount: number;
    status: string;
}

export default function Payment() {
    // FIX 1: Lấy đúng tham số ':id' từ cấu hình Route của bạn để gán vào biến bookingId
  const { bookingId } = useParams<{ bookingId: string }>();
    const navigate = useNavigate();

    const [bookingData, setBookingData] = useState<BookingPaymentDetail | null>(null);
    const [loading, setLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState("");
    const [isProcessing, setIsProcessing] = useState(false);

    // 1. Lấy thông tin hóa đơn đặt phòng từ Backend dựa vào BookingId
    useEffect(() => {
    const fetchBookingDetail = async () => {
        if (!bookingId) {
            setErrorMessage("❌ Không tìm thấy mã đơn đặt phòng trên đường dẫn URL.");
            setLoading(false);
            return;
        }

        try {
            setLoading(true);
            setErrorMessage("");
            
            // Gọi chuẩn endpoint GET /api/booking/{id} của Backend vừa viết
            const response = await api.get<BookingPaymentDetail>(`/booking/${bookingId}`);
            setBookingData(response.data);
        } catch (error: any) {
            console.error("Lỗi tải hóa đơn thanh toán:", error);
            setErrorMessage("❌ Không thể tải thông tin hóa đơn từ hệ thống. Vui lòng kiểm tra lại đơn hàng.");
        } finally {
            setLoading(false);
        }
    };

    fetchBookingDetail();
}, [bookingId]);
    // 2. FIX 2: Hàm xử lý thanh toán khớp 100% với cấu trúc Swagger của bạn
    const handlePaymentSubmit = async () => {
        if (!bookingId) return;
        
        setIsProcessing(true);
        setErrorMessage("");

        try {
            // Cấu trúc request body đúng như hình ảnh Swagger image_a1dd7d.png
            const requestBody = {
                bookingId: bookingId,
                paymentMethod: "vietqr" // Bạn có thể để "cash" hoặc "vietqr" tùy ý
            };

            console.log("Dữ liệu gửi lên API Payments/process:", requestBody);

            // Gọi đúng endpoint đã test trên Swagger
            await api.post("/Payments/process", requestBody);

            alert("🎉 Thanh toán qua cổng trực tuyến thành công!");
            navigate("/my-bookings"); // Điều hướng về trang danh sách phòng đã đặt của User
        } catch (error: any) {
            console.error("Lỗi xử lý thanh toán:", error);
            setErrorMessage(error.response?.data?.message || "Đã xảy ra lỗi khi xác thực thanh toán qua API.");
        } finally {
            setIsProcessing(false);
        }
    };

    const formatDate = (dateStr: string) => {
        if (!dateStr) return "";
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
            <main className="max-w-4xl mx-auto px-4 py-10 min-h-screen grid grid-cols-1 md:grid-cols-2 gap-8">
                
                {/* CỘT TRÁI: Thông tin hóa đơn */}
                <div className="space-y-6">
                    <div className="bg-white border rounded-2xl p-6 shadow-sm space-y-4">
                        <h2 className="text-xl font-bold text-gray-800 border-b pb-3 flex items-center gap-2">
                            💳 Thông tin thanh toán
                        </h2>
                        
                        {errorMessage && (
                            <div className="p-3 bg-red-50 text-red-600 text-sm font-medium rounded-xl border border-red-100">
                                {errorMessage}
                            </div>
                        )}

                        {bookingData && (
                            <div className="space-y-3 text-sm text-gray-600">
                                <div className="flex justify-between">
                                    <span>Mã đơn đặt phòng:</span>
                                    <span className="font-mono font-bold text-gray-800">{bookingData.id}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Ngày nhận phòng:</span>
                                    <span className="font-semibold text-gray-800">{formatDate(bookingData.checkIn)}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Ngày trả phòng:</span>
                                    <span className="font-semibold text-gray-800">{formatDate(bookingData.checkOut)}</span>
                                </div>
                                <hr className="border-dashed" />
                                <div className="flex justify-between items-center bg-blue-50/50 p-3 rounded-xl border border-blue-100/50">
                                    <span className="font-bold text-gray-700">Tổng số tiền cần trả:</span>
                                    <span className="text-xl font-black text-blue-600">
                                        {bookingData.totalAmount.toLocaleString("vi-VN")} đ
                                    </span>
                                </div>
                            </div>
                        )}
                    </div>

                    <div className="bg-slate-50 rounded-2xl p-6 border text-xs text-gray-500 space-y-2">
                        <h4 className="font-bold text-gray-700 text-sm">📌 Hướng dẫn thanh toán:</h4>
                        <p>1. Sử dụng ứng dụng Ngân hàng quét mã QR bên phải để tự động điền thông tin.</p>
                        <p>2. Kiểm tra kỹ số tiền trước khi xác nhận chuyển khoản.</p>
                        <p>3. Sau khi hoàn tất chuyển khoản thành công, nhấn nút bên dưới để cập nhật trạng thái hóa đơn.</p>
                    </div>
                </div>

                {/* CỘT PHẢI: Quét mã QR */}
                <div className="bg-white border rounded-2xl p-6 shadow-sm flex flex-col items-center justify-center text-center space-y-4">
                    <h3 className="text-lg font-bold text-gray-800">Quét mã VietQR để thanh toán</h3>
                    
                    {bookingData ? (
                        <div className="bg-slate-50 p-4 rounded-2xl border border-slate-100">
                            <img 
                                src={`https://img.vietqr.io/image/mbbank-123456789999-compact2.png?amount=${bookingData.totalAmount}&addInfo=DATPHONG%20${bookingData.id.substring(0,8)}&accountName=KHACH%20SAN%20BOOKING`}
                                alt="Mã QR Thanh Toán"
                                className="w-64 h-64 object-contain mx-auto rounded-xl shadow-sm"
                            />
                        </div>
                    ) : (
                        <div className="w-64 h-64 bg-gray-100 rounded-2xl flex items-center justify-center text-gray-400">
                            Đang tạo mã QR...
                        </div>
                    )}

                    <div className="text-xs text-gray-400">
                        Nội dung chuyển khoản mặc định: <span className="font-bold text-gray-700 font-mono">DATPHONG {bookingId?.substring(0,8)}</span>
                    </div>

                    <button
                        onClick={handlePaymentSubmit}
                        disabled={isProcessing || !bookingId}
                        className={`w-full max-w-xs text-white font-bold py-3 rounded-xl transition shadow-md ${
                            isProcessing
                            ? "bg-emerald-400 cursor-not-allowed"
                            : "bg-emerald-600 hover:bg-emerald-700 active:scale-[0.99]"
                        }`}
                    >
                        {isProcessing ? "Đang xử lý..." : "✔️ Tôi đã thanh toán thành công"}
                    </button>
                </div>

            </main>
            <Footer />
        </>
    );
}