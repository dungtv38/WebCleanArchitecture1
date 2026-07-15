export default function Hero() {
  return (
    <section className="relative h-[650px]">
      {/* Background */}
      <img
        src="https://images.unsplash.com/photo-1566073771259-6a8506099945?w=1600"
        alt="Hotel"
        className="absolute inset-0 h-full w-full object-cover"
      />

      {/* Overlay */}
      <div className="absolute inset-0 bg-black/50" />

      {/* Content */}
      <div className="relative z-10 flex h-full items-center">
        <div className="mx-auto max-w-7xl px-6 text-white">

          <h1 className="max-w-3xl text-5xl font-bold leading-tight md:text-6xl">
            Find Your Perfect Stay
          </h1>

          <p className="mt-6 max-w-2xl text-lg text-gray-200">
            Discover thousands of hotels, resorts and apartments at the best
            prices with secure online booking.
          </p>

          <div className="mt-8 flex gap-4">
            <button className="rounded-lg bg-blue-600 px-6 py-3 font-semibold hover:bg-blue-700">
              Book Now
            </button>

            <button className="rounded-lg border border-white px-6 py-3 hover:bg-white hover:text-black">
              Explore
            </button>
          </div>

        </div>
      </div>
    </section>
  );
}