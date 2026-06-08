import { useEffect, useState, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../components/layout/api";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";

interface RoomDTO {
    id: string;
    roomNumber: string;
    status: "Available" | "Occupied" | "Cleaning" | "Maintenance" | "Booked";
    note?: string;
    pricePerNight: number; // 🌟 ĐÃ ĐỒNG BỘ: Chuyển giá xuống từng phòng vật lý
    maxGuests: number;     // 🌟 ĐÃ ĐỒNG BỘ: Chuyển sức chứa xuống từng phòng vật lý
    images: string[];
}

interface RoomTypeDTO {
    id: string;
    name: string;
    rooms: RoomDTO[];
}

interface HotelDetailDTO {
    id: string;
    name: string;
    description: string;
    address: string;
    city: string;
    images: { id: string; imageUrl: string }[];
    roomTypes: RoomTypeDTO[];
}

type FilterType = "ALL" | "AVAILABLE" | "2_GUESTS" | "3_GUESTS" | "VIP";

export default function HotelDetail() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [hotel, setHotel] = useState<HotelDetailDTO | null>(null);
    const [loading, setLoading] = useState(true);
    const [activeFilter, setActiveFilter] = useState<FilterType>("ALL");

    useEffect(() => {
        const fetchHotelDetail = async () => {
            try {
                const response = await api.get<HotelDetailDTO>(`/hotel/${id}`);
                setHotel(response.data);
            } catch (error) {
                console.error("Lỗi gọi API chi tiết khách sạn:", error);
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchHotelDetail();
    }, [id]);

    // 🌟 SỬA LỖI LOGIC: Lọc chuẩn dựa trên thuộc tính bên trong mảng rooms
    const filteredRoomTypes = useMemo(() => {
        if (!hotel) return [];

        return hotel.roomTypes
            .map((type) => {
                // Lọc theo hạng phòng VIP
                if (activeFilter === "VIP" && !type.name.toLowerCase().includes("vip")) return null;

                let filteredRooms = type.rooms || [];

                // Lọc theo trạng thái trống hoặc sức chứa thực tế của phòng đó
                if (activeFilter === "AVAILABLE") {
                    filteredRooms = filteredRooms.filter((room) => room.status === "Available");
                } else if (activeFilter === "2_GUESTS") {
                    filteredRooms = filteredRooms.filter((room) => room.maxGuests === 2);
                } else if (activeFilter === "3_GUESTS") {
                    filteredRooms = filteredRooms.filter((room) => room.maxGuests === 3);
                }

                // Nếu loại phòng sau khi lọc không còn phòng nào hợp lệ thì ẩn cả cụm hạng phòng đó
                if (filteredRooms.length === 0) return null;

                return {
                    ...type,
                    rooms: filteredRooms,
                };
            })
            .filter((type) => type !== null) as RoomTypeDTO[];
    }, [hotel, activeFilter]);

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    if (!hotel) return <div className="p-10 text-center text-red-500">Không tìm thấy thông tin khách sạn!</div>;

    return (
        <>
            <Header />
            <main className="max-w-6xl mx-auto px-4 py-8 min-h-screen">

                {/* Banner & Thông tin Khách Sạn */}
                <div className="bg-white rounded-2xl shadow-sm border overflow-hidden mb-8">
                    <img
                        src={hotel.images?.[0]?.imageUrl || "https://picsum.photos/800/400"}
                        alt={hotel.name}
                        className="w-full h-64 object-cover"
                    />
                    <div className="p-6">
                        <h1 className="text-3xl font-bold text-gray-900">{hotel.name}</h1>
                        <p className="text-gray-500 mt-1">📍 {hotel.address}, {hotel.city}</p>
                        <p className="text-gray-600 mt-4 border-t pt-4">{hotel.description}</p>
                    </div>
                </div>

                {/* Thanh bộ lọc nhanh */}
                <div className="mb-6 border-b pb-4">
                    <p className="text-sm font-semibold text-gray-500 mb-3">Bộ lọc tìm phòng nhanh:</p>
                    <div className="flex flex-wrap gap-2">
                        {[
                            { id: "ALL", label: "✨ Tất cả phòng" },
                            { id: "AVAILABLE", label: "🟢 Chỉ phòng còn trống" },
                            { id: "2_GUESTS", label: "👥 Phòng 2 người" },
                            { id: "3_GUESTS", label: "👪 Phòng 3 người" },
                            { id: "VIP", label: "👑 Phòng hạng VIP" },
                        ].map((tab) => (
                            <button
                                key={tab.id}
                                onClick={() => setActiveFilter(tab.id as FilterType)}
                                className={`px-4 py-2 rounded-xl text-sm font-medium transition duration-200 ${
                                    activeFilter === tab.id
                                        ? "bg-blue-600 text-white shadow-md shadow-blue-200"
                                        : "bg-slate-100 text-gray-600 hover:bg-slate-200"
                                }`}
                            >
                                {tab.label}
                            </button>
                        ))}
                    </div>
                </div>

                <h2 className="text-2xl font-bold mb-6 text-gray-800">Kết quả tìm kiếm</h2>

                {filteredRoomTypes.length === 0 ? (
                    <div className="bg-slate-50 rounded-2xl p-12 text-center border text-gray-500">
                        Không có phòng nào phù hợp với bộ lọc được chọn.
                    </div>
                ) : (
                    <div className="space-y-8">
                        {filteredRoomTypes.map((type) => (
                            <div key={type.id} className="bg-white border rounded-2xl p-6 shadow-sm">
                                
                                {/* Header Loại Phòng */}
                                <h3 className="text-xl font-bold text-blue-600 mb-4 pb-2 border-b border-dashed">
                                    Hạng phòng: {type.name}
                                </h3>

                                {/* Grid danh sách phòng thực tế */}
                                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                                    {type.rooms.map((room) => (
                                        <div
                                            key={room.id}
                                            className={`border rounded-2xl overflow-hidden flex flex-col justify-between transition bg-white shadow-sm hover:shadow-md ${
                                                room.status === "Available" ? "border-slate-200" : "border-gray-200 opacity-75"
                                            }`}
                                        >
                                            {/* Ảnh phòng & Tag Sức chứa */}
                                            <div className="relative h-40 bg-gray-100 w-full overflow-hidden">
                                                <img
                                                    src={room.images && room.images.length > 0 ? room.images[0] : "https://images.unsplash.com/photo-1566665797739-1674de7a421a?q=80&w=400&auto=format&fit=crop"}
                                                    alt={`Phòng ${room.roomNumber}`}
                                                    className="w-full h-full object-cover"
                                                />
                                                <span className="absolute top-2 left-2 bg-black/60 text-white text-[11px] px-2 py-0.5 rounded-md font-medium backdrop-blur-sm">
                                                    👤 Tối đa: {room.maxGuests} khách
                                                </span>
                                            </div>

                                            {/* Chi tiết phòng & Giá cả / Đặt phòng */}
                                            <div className="p-4 flex-1 flex flex-col justify-between">
                                                <div>
                                                    <div className="flex justify-between items-center mb-1">
                                                        <span className="font-extrabold text-gray-800 text-lg">Phòng {room.roomNumber}</span>
                                                        <span className={`text-[11px] px-2 py-0.5 rounded-md font-bold ${
                                                            room.status === "Available" ? "bg-green-100 text-green-800" :
                                                            room.status === "Occupied" ? "bg-red-100 text-red-800" :
                                                            room.status === "Cleaning" ? "bg-blue-100 text-blue-800" :
                                                            room.status === "Maintenance" ? "bg-gray-100 text-gray-800" :
                                                            "bg-amber-100 text-amber-800"
                                                        }`}>
                                                            {
                                                                room.status === "Available" ? "Trống" :
                                                                room.status === "Occupied" ? "Đang ở" :
                                                                room.status === "Cleaning" ? "Dọn phòng" :
                                                                room.status === "Maintenance" ? "Bảo trì" : "Đã đặt"
                                                            }
                                                        </span>
                                                    </div>
                                                    {room.note && <p className="text-xs text-gray-400 italic mb-2">🔹 {room.note}</p>}
                                                </div>

                                                {/* Khu vực hiển thị giá tiền và hành động đặt phòng theo từng phòng */}
                                                <div className="mt-4 pt-3 border-t flex items-center justify-between">
                                                    <div>
                                                        <p className="text-[10px] text-gray-400 uppercase tracking-wider">Giá 1 đêm</p>
                                                        <span className="text-base font-bold text-orange-600">
                                                            {room.pricePerNight.toLocaleString("vi-VN")} đ
                                                        </span>
                                                    </div>

                                                    <button
                                                        disabled={room.status !== "Available"}
                                                        onClick={() => navigate(`/booking?hotelId=${hotel.id}&roomId=${room.id}`)}
                                                        className={`px-3 py-2 rounded-xl text-xs font-bold text-white transition duration-150 ${
                                                            room.status === "Available"
                                                                ? "bg-blue-600 hover:bg-blue-700 shadow-sm shadow-blue-100"
                                                                : "bg-gray-300 cursor-not-allowed"
                                                        }`}
                                                    >
                                                        {room.status === "Available" ? "Đặt ngay" : "Hết phòng"}
                                                    </button>
                                                </div>
                                            </div>

                                        </div>
                                    ))}
                                </div>

                            </div>
                        ))}
                    </div>
                )}
            </main>
            <Footer />
        </>
    );
}