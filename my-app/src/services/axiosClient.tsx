import axios from "axios";

const axiosClient = axios.create({
  baseURL: "http://localhost:5254/api",
});

export default axiosClient;