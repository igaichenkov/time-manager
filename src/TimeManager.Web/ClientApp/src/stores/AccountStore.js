import axios from "../utils/axios";

export const fetchProfile = () => axios.get("/api/account/me");

export const fetchUserProfile = userId =>
  axios.get("/api/account/users/" + userId);

export const login = creds => axios.post("/api/Account/SignIn", creds);

export const signUp = profile => axios.post("/api/Account/SignUp", profile);

export const signOut = () => axios.post("/api/Account/SignOut");

export const changePassword = passwords =>
  axios.put("/api/Account/me/password", passwords);

export const updateProfile = profile =>
  axios.put("/api/Account/me/profile", {
    ...profile,
    preferredHoursPerDay: profile.preferredHoursPerDay
      ? parseFloat(profile.preferredHoursPerDay)
      : 0
  });
