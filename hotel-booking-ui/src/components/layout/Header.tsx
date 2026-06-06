import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";

const Header = () => {
  const navigate = useNavigate();

  const [token, setToken] = useState(
    localStorage.getItem("token")
  );

  const handleLogout = () => {
    localStorage.removeItem("token");
    setToken(null); // update UI ngay
    navigate("/home");
  };

  return (
    <header className="bg-blue-600 text-white">
      <div className="max-w-7xl mx-auto px-5 py-4 flex items-center justify-between">
        <Link to="/" className="text-2xl font-bold">
          Hotel Booking
        </Link>

        <nav className="flex gap-3 items-center">
          {token ? (
            <>
              <Link
                to="/booking"
                className="bg-white text-blue-600 px-4 py-2 rounded-xl font-semibold"
              >
                Booking
              </Link>

              <button
                onClick={handleLogout}
                className="bg-red-500 px-4 py-2 rounded-xl font-semibold"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login">Login</Link>

              <Link
                to="/register"
                className="bg-white text-blue-600 px-4 py-2 rounded-xl font-semibold"
              >
                Register
              </Link>
            </>
          )}
        </nav>
      </div>
    </header>
  );
};

export default Header;