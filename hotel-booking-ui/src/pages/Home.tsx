import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import api from "../components/layout/api";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"; // 👈 Thêm useNavigate để bấm Book


interface Hotel {
  id: string; // Đổi thành string vì Backend dùng Guid
  name: string;
  description: string;
  address: string;
  city: string;
  images: string[];
}

const Home = () => {
  const navigate = useNavigate();
  const [hotels, setHotels] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchHotels();
  }, []);

  const fetchHotels = async () => {
    try {
      const response = await api.get<Hotel[]>("/Hotel/");
      console.log("Hotels:", response.data);
      setHotels(response.data);
    } catch (error) {
      console.error("Lỗi gọi API:", error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <>
      <Header />

      <main className="max-w-7xl mx-auto px-5 mb-10">
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
            <input type="date" className="border rounded-xl px-4 py-3 text-black" />
            <input type="date" className="border rounded-xl px-4 py-3 text-black" />
            <button className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl font-semibold">
              Search
            </button>
          </div>
        </section>

        {/* HOTEL LIST */}
        <section className="mt-14">
          <h2 className="text-3xl font-bold mb-6">Popular Hotels</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {hotels.map((hotel) => (
              <div
                key={hotel.id}
                className="bg-white rounded-2xl shadow-md overflow-hidden hover:shadow-xl transition duration-300 flex flex-col justify-between"
              >
                <div>
                  {/* Sửa lỗi hiển thị ảnh ở đây */}
                  <img
                    src={hotel.images?.[1] || "https://picsum.photos/800/400"}
                    alt={hotel.name}
                    className="w-full h-64 object-cover"
                  />

                  <div className="p-5">
                    <div className="flex items-start justify-between gap-2">
                      <h3 className="text-xl font-bold line-clamp-1">{hotel.name}</h3>
                      <span className="bg-blue-100 text-blue-800 px-2 py-1 rounded-lg text-xs font-semibold whitespace-nowrap">
                        {hotel.city}
                      </span>
                    </div>

                    <p className="text-gray-500 text-sm mt-2 line-clamp-2">
                      {hotel.description}
                    </p>
                  </div>
                </div>

                <div className="p-5 pt-0 mt-auto">
                  <div className="border-t pt-4 flex items-center justify-between">
                    <div className="flex flex-col">
                      <span className="text-xs text-gray-400">Địa chỉ</span>
                      <span className="text-sm font-semibold text-gray-700 line-clamp-1">
                        {hotel.address}
                      </span>
                    </div>

                    {/* Điều hướng sang trang chi tiết khách sạn để xem phòng & đặt */}
                    <button
                      onClick={() => navigate(`/hotel/${hotel.id}`)}
                      className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-xl transition font-medium"
                    >
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