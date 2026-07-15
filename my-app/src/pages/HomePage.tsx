import { useEffect } from "react";

import Navbar from "@/components/Navbar";
import Hero from "@/components/Hero";
import SearchBox from "@/components/SearchBox";
import PopularHotels from "@/components/PopularHotels";
// import WhyChooseUs from "@/components/WhyChooseUs";
import Footer from "@/components/Footer";

export default function HomePage() {
  useEffect(() => {
    document.title = "TravelBooking";
  }, []);

  return (
    <div className="min-h-screen bg-slate-50">

      <Navbar />

      <Hero />

      <SearchBox />

      <PopularHotels />

      {/* <WhyChooseUs /> */}

      <Footer />

    </div>
  );
}