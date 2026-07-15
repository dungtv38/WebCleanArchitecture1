import { useEffect, useState } from "react";
import HotelCard from "./HotelCard";
import type { Hotel } from "@/types/hotel";
import { getHotels } from "@/services/HotelService";

export default function PopularHotels() {
  const [hotels, setHotels] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadHotels() {
      try {
        const data = await getHotels();
        setHotels(data);
      } catch (error) {
        console.error("Load hotels failed:", error);
      } finally {
        setLoading(false);
      }
    }

    loadHotels();
  }, []);

  if (loading) {
    return (
      <div className="max-w-7xl mx-auto py-10 text-center">
        Loading...
      </div>
    );
  }

  return (
    <section className="mx-auto mt-20 max-w-7xl px-6">
      <h2 className="text-4xl font-bold mb-8">
        Popular Hotels
      </h2>

      <div className="grid gap-8 md:grid-cols-2 lg:grid-cols-3">
        {hotels.map((hotel) => (
          <HotelCard
            key={hotel.id}
            hotel={hotel}
          />
        ))}
      </div>
    </section>
  );
}