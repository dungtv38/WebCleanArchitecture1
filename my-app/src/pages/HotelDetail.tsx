import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { MapPin } from "lucide-react";
import { Link } from "react-router-dom";


import { getHotelById } from "@/services/HotelService";
import type { HotelDetail } from "@/types/hotel";


export default function HotelDetail() {
    const { id } = useParams();

    const [hotel, setHotel] = useState<HotelDetail | null>(null);


    useEffect(() => {
        async function loadHotel() {
            if (!id) return;

            const data = await getHotelById(id);

            setHotel(data);
        }

        loadHotel();
    }, [id]);
    if (!hotel)
        return (
            <div className="py-20 text-center">
                Loading...
            </div>
        );
    return (
        <div className="bg-gray-100 min-h-screen">

            <div className="mx-auto max-w-7xl p-8">

                <img
                    src={
                        hotel.images.length > 0
                            ? hotel.images[0]
                            : "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=1600"
                    }
                    className="h-[420px] w-full rounded-3xl object-cover"
                />

                <div className="mt-8">

                    <h1 className="text-4xl font-bold">

                        {hotel.name}

                    </h1>

                    <div className="mt-3 flex items-center text-gray-500">

                        <MapPin size={18} />

                        <span className="ml-2">

                            {hotel.address}, {hotel.city}

                        </span>

                    </div>

                    <p className="mt-5 text-gray-600">

                        {hotel.description}

                    </p>

                </div>

                {hotel.roomTypes.map((type) => (
                    <div key={type.id} className="mt-14">



                        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">

                            {type.rooms.map((room) => (

                                <div
                                    key={room.id}
                                    className="overflow-hidden rounded-2xl bg-white shadow transition hover:-translate-y-1 hover:shadow-xl"
                                >

                                    <img
                                        src={
                                            room.images.length > 0
                                                ? room.images[0]
                                                : "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?w=1200"
                                        }
                                        className="h-52 w-full object-cover"
                                    />

                                    <div className="p-5">

                                        <div className="flex items-center justify-between">

                                            <h3 className="text-xl font-bold">
                                                Room {room.roomNumber}
                                            </h3>

                                            <span
                                                className={`rounded-full px-3 py-1 text-xs font-semibold ${room.status === "Available"
                                                    ? "bg-green-100 text-green-700"
                                                    : "bg-red-100 text-red-700"
                                                    }`}
                                            >
                                                {room.status}
                                            </span>

                                        </div>

                                        <div className="mt-4 space-y-2 text-gray-600">

                                            <p>
                                                👥 Max Guests: <b>{room.maxGuests}</b>
                                            </p>

                                            <p>
                                                📝 {room.note}
                                            </p>

                                        </div>

                                        <div className="mt-5 flex items-center justify-between">

                                            <div>

                                                <p className="text-sm text-gray-400">
                                                    Price / Night
                                                </p>

                                                <p className="text-2xl font-bold text-blue-600">
                                                    {room.pricePerNight.toLocaleString()} đ
                                                </p>

                                            </div>

                                        </div>

                                        <Link
                                            to={`/room/${room.id}`} // Đảm bảo room.id có giá trị
                                            className="mt-6 block w-full rounded-xl bg-blue-600 py-3 text-center font-semibold text-white transition hover:bg-blue-700"
                                        >
                                            View Detail
                                        </Link>

                                    </div>

                                </div>

                            ))}

                        </div>

                    </div>
                ))}

            </div>

        </div>
    );
}