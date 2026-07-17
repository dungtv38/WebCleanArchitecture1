import { Hotel } from "lucide-react";
import { Link } from "react-router-dom";

interface User {
    id?: string;
    fullName?: string;
    email?: string;
}

function getStoredUser(): User | null {
    const userString = localStorage.getItem("user");

    if (
        !userString ||
        userString === "undefined" ||
        userString === "null"
    ) {
        return null;
    }

    try {
        return JSON.parse(userString) as User;
    } catch {
        localStorage.removeItem("user");
        return null;
    }
}

export default function Navbar() {
    const user = getStoredUser();

    return (
        <header className="sticky top-0 z-50 bg-white shadow-sm">
            <div className="max-w-7xl mx-auto h-20 px-6 flex items-center justify-between">

                <Link to="/" className="flex items-center gap-3">
                    <Hotel className="text-blue-600" size={28} />

                    <div>
                        <h1 className="text-xl font-bold text-blue-600">
                            TravelBooking
                        </h1>

                        <p className="text-xs text-gray-500">
                            Book your perfect stay
                        </p>
                    </div>
                </Link>

                <nav className="hidden md:flex gap-8">
                    <Link to="/">Hotels</Link>
                    <Link to="/">Rooms</Link>
                    <Link to="/">About</Link>
                    <Link to="/">Contact</Link>
                </nav>

                {!user ? (
                    <div className="flex gap-3">
                        <Link
                            to="/signin"
                            className="px-4 py-2 border rounded-lg"
                        >
                            Sign In
                        </Link>

                        <Link
                            to="/signup"
                            className="px-4 py-2 bg-blue-600 text-white rounded-lg"
                        >
                            Register
                        </Link>
                    </div>
                ) : (
                    <div className="flex items-center gap-4">
                        <span className="font-semibold text-blue-600">
                            Xin chào, {user.fullName || "Khách hàng"}
                        </span>
                    </div>
                )}

            </div>
        </header>
    );
}