export interface Hotel {
  id: string;
  name: string;
  address: string;
  description: string;
  images: string[];
}
export interface Room {
    id: string;
    roomNumber: string;
    status: string;
    note: string;
    pricePerNight: number;
    maxGuests: number;
    images: string[];
}

export interface RoomType {
    id: string;
    name: string;
    rooms: Room[];
}

export interface HotelDetail {
    id: string;
    name: string;
    description: string;
    address: string;
    city: string;
    images: string[];
    roomTypes: RoomType[];
}