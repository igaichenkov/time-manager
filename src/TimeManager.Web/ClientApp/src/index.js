import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import AuthContextProvider from "./context/auth-context";
import ErrorContextProvider from "./context/ErrorContext";
import { SnackbarProvider } from "notistack";

ReactDOM.render(
  <AuthContextProvider>
    <SnackbarProvider maxSnack={3}>
      <ErrorContextProvider>
        <App />
      </ErrorContextProvider>
    </SnackbarProvider>
  </AuthContextProvider>,
  document.getElementById("root")
);
