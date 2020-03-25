import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import AuthContextProvider from "./context/auth-context";
import ErrorContextProvider from "./context/ErrorContext";
import LoadingSpinner from "./components/LoadingSpinner";

ReactDOM.render(
  <React.Fragment>
    <LoadingSpinner />
    <AuthContextProvider>
      <ErrorContextProvider>
        <App />
      </ErrorContextProvider>
    </AuthContextProvider>
  </React.Fragment>,
  document.getElementById("root")
);
