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

  const fetchProfile = () => {
    return axios.get("http://localhost:5000/api/account/me").then(resp => {
      setAccount({
        isAuthentecated: true,
        initialized: true,
        profile: resp.data
      });
    });
  };

  const login = creds =>
    axios
      .post("http://localhost:5000/api/Account/SignIn", creds)
      .then(() => fetchProfile());

  const signUp = profile =>
    axios
      .post("http://localhost:5000/api/Account/SignUp", profile)
      .then(() => fetchProfile());

  useEffect(() => {
    fetchProfile().catch(() => {
      setAccount({
        isAuthentecated: false,
        initialized: true,
        profile: {}
      });
    });
  }, []);

  return (
    <AuthContext.Provider
      value={{ account: account, login: login, signUp: signUp }}
    >
      {props.children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
