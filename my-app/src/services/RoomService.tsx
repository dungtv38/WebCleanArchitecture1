import axiosClient from "./axiosClient";
import type { Room } from "@/types/hotel";



// Thêm hàm này vào file RoomService của bạn
export const getRoomById = async (id: string): Promise<Room> => {
    const res = await axiosClient.get(`/Rooms/type/${id}`);
    return res.data;
};