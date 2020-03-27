import axios from "../utils/axios";

export const fetchProfile = () => axios.get("/api/account/me");

export const fetchUserProfile = userId =>
  userId ? axios.get("/api/account/users/" + userId) : fetchProfile();

export const login = creds => axios.post("/api/Account/SignIn", creds);

export const signUp = profile => axios.post("/api/Account/SignUp", profile);

export const signOut = () => axios.post("/api/Account/SignOut");

export const changePassword = passwords =>
  axios.put("/api/Account/me/password", passwords);

export const resetPassword = (userId, newPassword) =>
  axios.put(`/api/Account/users/${userId}/password`, {
    newPassword: newPassword
  });

export const updateProfile = profile =>
  axios.put("/api/Account/me", createUserProfileRequest(profile));

export const updateUserProfile = (userId, profile) =>
  axios.put("/api/Account/users/" + userId, createUserProfileRequest(profile));

const createUserProfileRequest = formState => ({
  ...formState,
  preferredHoursPerDay: formState.preferredHoursPerDay
    ? parseFloat(formState.preferredHoursPerDay)
    : 0
});
