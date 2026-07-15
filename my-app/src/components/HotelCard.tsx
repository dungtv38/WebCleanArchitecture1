import { MapPin } from "lucide-react";
import { useNavigate } from "react-router-dom";
import type { Hotel } from "@/types/hotel";

interface Props {
  hotel: Hotel;
}

export default function HotelCard({ hotel }: Props) {
  const navigate = useNavigate();

  const image =
    hotel.images.length > 0
      ? hotel.images[0]
      : "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=1200";

  return (
    <div className="overflow-hidden rounded-2xl bg-white shadow-md transition duration-300 hover:-translate-y-1 hover:shadow-xl">

      <img
        src={image}
        alt={hotel.name}
        className="h-60 w-full object-cover"
      />

      <div className="p-5">

        <h3 className="text-xl font-bold">
          {hotel.name}
        </h3>

        <div className="mt-2 flex items-center text-gray-500">
          <MapPin size={16} />
          <span className="ml-2">
            {hotel.address}
          </span>
        </div>

        <p className="mt-4 line-clamp-3 text-gray-600">
          {hotel.description}
        </p>

        <button
          onClick={() => navigate(`/hotel/${hotel.id}`)}
          className="mt-6 w-full rounded-lg bg-blue-600 py-3 text-white transition hover:bg-blue-700"
        >
          View Details
        </button>

      </div>

    </div>
  );
}