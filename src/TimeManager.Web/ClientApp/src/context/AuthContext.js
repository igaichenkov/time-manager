import React, { useState, useEffect, useCallback } from "react";
import * as AccountStore from "../stores/AccountStore";

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

  const profileReceived = useCallback(
    profileData =>
      setAccount({
        isAuthentecated: true,
        initialized: true,
        profile: profileData
      }),
    [setAccount]
  );

  const fetchProfile = useCallback(() => {
    return AccountStore.fetchProfile().then(resp => profileReceived(resp.data));
  }, [profileReceived]);

  const login = creds =>
    AccountStore.login(creds).then(resp => profileReceived(resp.data));

  const signUp = profile =>
    AccountStore.signUp(profile).then(resp => profileReceived(resp.data));

  const signOut = () =>
    AccountStore.signOut().then(() => setUnauthenticatedProfile());

  const changePassword = passwords => AccountStore.changePassword(passwords);

  const updateProfile = profile =>
    AccountStore.updateProfile(profile).then(resp =>
      profileReceived(resp.data)
    );

  useEffect(() => {
    fetchProfile().catch(() => setUnauthenticatedProfile());
  }, [fetchProfile]);

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
