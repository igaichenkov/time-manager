import React, { useContext } from "react";
import { AuthContext } from "./context/auth-context";
import SignIn from "./components/Auth/SignIn";
import Dashboard from "./components/Dashboard";

function App() {
  const authContext = useContext(AuthContext);

  if (!authContext.isAuthentecated) {
    return <SignIn />;
  }

  return <Dashboard />;
}

export default App;
