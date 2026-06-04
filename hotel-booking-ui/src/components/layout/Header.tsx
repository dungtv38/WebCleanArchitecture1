const Header = () => {
  return (
    <header className="bg-blue-600 text-white">
      <div className="max-w-7xl mx-auto px-5 py-4 flex items-center justify-between">
        <h1 className="text-2xl font-bold">
          Hotel Booking
        </h1>

        <nav className="flex gap-5">
          <button className="hover:text-gray-200">
            Login
          </button>

          <button className="bg-white text-blue-600 px-4 py-2 rounded-xl font-semibold">
            Register
          </button>
        </nav>
      </div>
    </header>
  );
};

export default Header;