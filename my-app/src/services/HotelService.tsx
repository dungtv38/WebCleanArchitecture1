import axiosClient from "./axiosClient";

export const getHotels = async () => {
  const response = await axiosClient.get("/Hotel");
  return response.data;
};
export const getHotelById = async (id: string) => {
  const response = await axiosClient.get(`/Hotel/${id}`);
  return response.data;
};
