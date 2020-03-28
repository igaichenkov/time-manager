import axios from "../utils/axios";

export const getUsersList = () => axios.get("/api/account/users");
