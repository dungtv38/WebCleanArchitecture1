import { useState } from "react";
import { MapPin, CalendarDays, Users, Search } from "lucide-react";

export default function SearchBox() {
  const [destination, setDestination] = useState("");
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");
  const [guests, setGuests] = useState(2);

  const handleSearch = () => {
    console.log({
      destination,
      checkIn,
      checkOut,
      guests,
    });

    // Buổi sau sẽ gọi API tại đây
  };

  return (
    <div className="relative -mt-16 z-20">
      <div className="max-w-7xl mx-auto bg-white rounded-2xl shadow-xl p-6">

        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">

          {/* Destination */}
          <div>
            <label className="text-sm font-medium text-gray-500">
              Destination
            </label>

            <div className="flex items-center border rounded-lg px-3 py-2 mt-1">
              <MapPin size={18} className="text-gray-400" />
              <input
                className="ml-2 w-full outline-none"
                placeholder="Where are you going?"
                value={destination}
                onChange={(e) => setDestination(e.target.value)}
              />
            </div>
          </div>

          {/* Check In */}
          <div>
            <label className="text-sm font-medium text-gray-500">
              Check In
            </label>

            <div className="flex items-center border rounded-lg px-3 py-2 mt-1">
              <CalendarDays size={18} className="text-gray-400" />
              <input
                type="date"
                className="ml-2 w-full outline-none"
                value={checkIn}
                onChange={(e) => setCheckIn(e.target.value)}
              />
            </div>
          </div>

          {/* Check Out */}
          <div>
            <label className="text-sm font-medium text-gray-500">
              Check Out
            </label>

            <div className="flex items-center border rounded-lg px-3 py-2 mt-1">
              <CalendarDays size={18} className="text-gray-400" />
              <input
                type="date"
                className="ml-2 w-full outline-none"
                value={checkOut}
                onChange={(e) => setCheckOut(e.target.value)}
              />
            </div>
          </div>

          {/* Guests */}
          <div>
            <label className="text-sm font-medium text-gray-500">
              Guests
            </label>

            <div className="flex items-center border rounded-lg px-3 py-2 mt-1">
              <Users size={18} className="text-gray-400" />

              <select
                className="ml-2 w-full outline-none"
                value={guests}
                onChange={(e) => setGuests(Number(e.target.value))}
              >
                {[1,2,3,4,5,6].map((g) => (
                  <option key={g} value={g}>
                    {g} Guest{g > 1 ? "s" : ""}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* Button */}
          <div className="flex items-end">
            <button
              onClick={handleSearch}
              className="w-full bg-blue-600 hover:bg-blue-700 text-white rounded-lg py-3 flex justify-center items-center gap-2"
            >
              <Search size={18} />
              Search
            </button>
          </div>

        </div>
      </div>
    </div>
  );
}