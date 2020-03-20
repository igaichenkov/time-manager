import React, { useState, useEffect } from "react";
import axios from "axios";
import SignUp from "../components/Auth/SignUp";

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
      .get("http://localhost:5000/api/account/me")
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
      .post("http://localhost:5000/api/Account/SignIn", creds)
      .then(resp => profileReceived(resp.data));

  const signUp = profile =>
    axios
      .post("http://localhost:5000/api/Account/SignUp", profile)
      .then(resp => profileReceived(resp.data));

  const signOut = () =>
    axios
      .post("http://localhost:5000/api/Account/SignOut")
      .then(() => setUnauthenticatedProfile());

  useEffect(() => {
    fetchProfile().catch(() => setUnauthenticatedProfile());
  }, []);

  return (
    <AuthContext.Provider
      value={{
        account: account,
        login: login,
        signUp: signUp,
        signOut: signOut
      }}
    >
      {props.children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
