import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import api from "../components/layout/api";
import { useEffect, useState } from "react";

interface Hotel {
  id: number;
  name: string;
  description: string;
  address: string;
  city: string;
  ownerId: string;
  images: any[];
}

const Home = () => {

  const [hotels, setHotels] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchHotels();
  }, []);

  const fetchHotels = async () => {
    try {
      const response = await api.get<Hotel[]>("/hotel");

      console.log("Hotels:", response.data);

      setHotels(response.data);
    } catch (error) {
      console.error("Lỗi gọi API:", error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="p-10">Loading...</div>;
  }
  return (
    <>
      <Header />

      <main className="max-w-7xl mx-auto px-5">
        {/* HERO */}
        <section className="bg-blue-600 rounded-3xl mt-10 p-10 text-white">
          <h1 className="text-5xl font-bold leading-tight">
            Find your next stay
          </h1>

          <p className="mt-4 text-lg text-blue-100">
            Search low prices on hotels, homes and much more...
          </p>

          {/* SEARCH */}
          <div className="bg-white rounded-2xl p-4 mt-8 flex flex-col md:flex-row gap-4">
            <input
              type="text"
              placeholder="Where are you going?"
              className="flex-1 border rounded-xl px-4 py-3 text-black outline-none"
            />

            <input
              type="date"
              className="border rounded-xl px-4 py-3 text-black"
            />

            <input
              type="date"
              className="border rounded-xl px-4 py-3 text-black"
            />

            <button className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl font-semibold">
              Search
            </button>
          </div>
        </section>

        {/* HOTEL LIST */}
        <section className="mt-14">
          <h2 className="text-3xl font-bold mb-6">
            Popular Hotels
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {hotels.map((hotel) => (
              <div
                 key={hotel.id}
                className="bg-white rounded-2xl shadow-md overflow-hidden hover:shadow-xl transition duration-300"
              >
                <img
                  src="{images[0]}"
                  className="h-56 w-full object-cover"
                />

                <div className="p-5">
                  <div className="flex items-center justify-between">
                    <h3 className="text-xl font-bold">
                     {hotel.name}
                    </h3>

                    <span className="bg-blue-600 text-white px-2 py-1 rounded-lg text-sm">
                           {hotel.description}
                    </span>
                  </div>

                  <p className="text-gray-500 mt-2">
                  {hotel.city}
                  </p>

                  <div className="mt-5 flex items-center justify-between">
                    <div>
                      <span className="text-2xl font-bold">
          {hotel.address}
                      </span>

                    
                    </div>

                    <button className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-xl">
                      Book
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </section>
      </main>

      <Footer />
    </>
  );
};

export default Home;