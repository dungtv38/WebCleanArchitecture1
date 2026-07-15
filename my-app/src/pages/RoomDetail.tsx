import { useEffect, useState, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ArrowLeft, CheckCircle2, XCircle, Calendar, CreditCard, Users, BedDouble } from "lucide-react";
import axiosClient from "@/services/axiosClient";
import type { Room } from "@/types/hotel";

export default function RoomDetail() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [room, setRoom] = useState<Room | null>(null);
    const [loading, setLoading] = useState(true);

    // State chọn ngày
    const [checkIn, setCheckIn] = useState("");
    const [checkOut, setCheckOut] = useState("");

    const today = useMemo(() => new Date().toISOString().split("T")[0], []);

    // Tính toán số đêm và tổng tiền
    const { totalNights, totalAmount } = useMemo(() => {
        if (!checkIn || !checkOut || !room) return { totalNights: 0, totalAmount: 0 };
        const start = new Date(checkIn);
        const end = new Date(checkOut);
        const diffDays = Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
        const nights = diffDays > 0 ? diffDays : 0;
        return { totalNights: nights, totalAmount: nights * room.pricePerNight };
    }, [checkIn, checkOut, room]);

    useEffect(() => {
        const getRoom = async () => {
            try {
                const res = await axiosClient.get(`/Rooms/type/${id}`);
                setRoom(res.data);
            } catch (err) {
                console.error("Lỗi tải chi tiết phòng:", err);
            } finally {
                setLoading(false);
            }
        };
        if (id) getRoom();
    }, [id]);

    // Xử lý khi thay đổi ngày nhận: Nếu chọn ngày mới thì reset ngày trả để tránh lỗi
    const handleCheckInChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setCheckIn(e.target.value);
        setCheckOut(""); // Reset ngày trả để bắt buộc chọn lại ngày hợp lệ
    };

    const handleBooking = () => {
        navigate(`/booking/${room?.id}?checkIn=${checkIn}&checkOut=${checkOut}`);
    };

    if (loading) return <div className="py-20 text-center text-blue-600 font-bold">Đang tải thông tin phòng...</div>;
    if (!room) return <div className="py-20 text-center">Không tìm thấy thông tin phòng này.</div>;

    return (
        <div className="max-w-7xl mx-auto py-10 px-6 animate-in fade-in duration-500">
            <button onClick={() => navigate(-1)} className="flex items-center gap-2 mb-6 text-gray-500 hover:text-blue-600 transition">
                <ArrowLeft size={20} /> Quay lại danh sách
            </button>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
                {/* Cột Ảnh */}
                <div className="flex flex-col gap-4">
                    <img src={room.images[0]} className="w-full h-[450px] object-cover rounded-3xl shadow-lg" alt="Room" />
                    <div className="grid grid-cols-3 gap-4">
                        {room.images.slice(1, 4).map((img, idx) => (
                            <img key={idx} src={img} className="w-full h-[160px] object-cover rounded-2xl cursor-pointer hover:opacity-90 transition" alt="Detail" />
                        ))}
                    </div>
                </div>

                {/* Cột Thông Tin & Booking */}
                <div className="flex flex-col">
                    <div className="flex justify-between items-start">
                        <h1 className="text-4xl font-extrabold text-gray-900">Phòng {room.roomNumber}</h1>
                        <span className={`px-4 py-1.5 rounded-full text-sm font-bold flex items-center gap-1 ${room.status === "Available" ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"}`}>
                            {room.status === "Available" ? <CheckCircle2 size={16} /> : <XCircle size={16} />}
                            {room.status}
                        </span>
                    </div>

                    <div className="flex items-center gap-6 mt-4 text-gray-600">
                        <span className="flex items-center gap-2"><Users size={20} /> Tối đa {room.maxGuests} khách</span>
                        <span className="flex items-center gap-2"><BedDouble size={20} /> Tiện nghi cao cấp</span>
                    </div>

                    <p className="mt-6 text-gray-600 leading-relaxed bg-gray-50 p-5 rounded-2xl border border-gray-100">{room.note}</p>

                    {/* Khối Booking */}
                    <div className="mt-8 bg-gray-900 text-white p-8 rounded-3xl shadow-2xl">
                        <h3 className="text-lg font-bold flex items-center gap-2 mb-6"><Calendar size={20} /> Đặt lịch lưu trú</h3>
                        
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="text-[11px] font-bold text-gray-400 uppercase">Ngày nhận</label>
                                <input type="date" min={today} value={checkIn} onChange={handleCheckInChange} className="w-full mt-1 p-3 rounded-xl bg-gray-800 border-none text-white focus:ring-2 focus:ring-blue-500" />
                            </div>
                            <div>
                                <label className="text-[11px] font-bold text-gray-400 uppercase">Ngày trả</label>
                                <input type="date" min={checkIn || today} value={checkOut} onChange={(e) => setCheckOut(e.target.value)} className="w-full mt-1 p-3 rounded-xl bg-gray-800 border-none text-white focus:ring-2 focus:ring-blue-500" />
                            </div>
                        </div>

                        {totalNights > 0 && (
                            <div className="mt-6 space-y-3 border-t border-gray-700 pt-6">
                                <div className="flex justify-between text-gray-400">
                                    <span>Giá mỗi đêm</span>
                                    <span>{room.pricePerNight.toLocaleString()} đ</span>
                                </div>
                                <div className="flex justify-between text-xl font-black">
                                    <span>Tổng ({totalNights} đêm)</span>
                                    <span className="text-blue-400">{totalAmount.toLocaleString()} đ</span>
                                </div>
                            </div>
                        )}

                        <button 
                            onClick={handleBooking}
                            disabled={room.status !== "Available" || totalNights === 0}
                            className={`mt-8 w-full py-4 text-lg font-bold rounded-2xl transition flex items-center justify-center gap-2 ${room.status === "Available" && totalNights > 0 ? "bg-blue-600 hover:bg-blue-700 text-white" : "bg-gray-600 cursor-not-allowed text-gray-400"}`}
                        >
                            <CreditCard size={20} />
                            {room.status === "Available" ? "Tiến hành thanh toán" : "Phòng đã kín lịch"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}