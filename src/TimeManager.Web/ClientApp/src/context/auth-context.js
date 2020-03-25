import React, { useState, useEffect } from "react";
import axios from "../utils/axios";

export const AuthContext = React.createContext({
  account: {
    isAuthentecated: false,
    initialized: false,
    profile: {}
  },
  login: () => {},
  signUp: () => {}
});

const AuthContextProvider = props => {
  const [account, setAccount] = useState({
    isAuthentecated: false,
    initialized: false,
    profile: {}
  });

  const setUnauthenticatedProfile = () =>
    setAccount({
      isAuthentecated: false,
      initialized: true,
      profile: {}
    });

  const fetchProfile = () => {
    return axios
      .get("/api/account/me")
      .then(resp => profileReceived(resp.data));
  };

  const profileReceived = profileData =>
    setAccount({
      isAuthentecated: true,
      initialized: true,
      profile: profileData
    });

  const login = creds =>
    axios
      .post("/api/Account/SignIn", creds)
      .then(resp => profileReceived(resp.data));

  const signUp = profile =>
    axios
      .post("/api/Account/SignUp", profile)
      .then(resp => profileReceived(resp.data));

  const signOut = () =>
    axios.post("/api/Account/SignOut").then(() => setUnauthenticatedProfile());

  const changePassword = passwords =>
    axios.put("/api/Account/me/password", passwords);

  const updateProfile = profile =>
    axios
      .put("/api/Account/me/profile", {
        ...profile,
        preferredHoursPerDay: profile.preferredHoursPerDay
          ? parseFloat(profile.preferredHoursPerDay)
          : 0
      })
      .then(resp => profileReceived(resp.data));

  useEffect(() => {
    fetchProfile().catch(() => setUnauthenticatedProfile());
  }, []);

  return (
    <AuthContext.Provider
      value={{
        account: account,
        login: login,
        signUp: signUp,
        signOut: signOut,
        changePassword: changePassword,
        updateProfile: updateProfile,
        setUnauthenticated: setUnauthenticatedProfile
      }}
    >
      {props.children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
