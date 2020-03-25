import React, { useContext } from "react";
import axios from "../utils/axios";
import { useSnackbar } from "notistack";
import { AuthContext } from "../context/auth-context";

const createEmptyErrorState = () => ({});

export const ErrorContext = React.createContext(createEmptyErrorState());

const ErrorContextProvider = props => {
  const { enqueueSnackbar } = useSnackbar();
  const authContext = useContext(AuthContext);

  const errorHandler = error => {
    if (error.response.status == 401) {
      authContext.setUnauthenticated();
    } else {
      error.response.data.errors.forEach(errorItem =>
        handleSetError(errorItem.description)
      );
    }

    return Promise.reject({ ...error });
  };

  axios.interceptors.response.use(null, errorHandler);

  const handleSetError = message =>
    enqueueSnackbar(message, { variant: "error" });

  return <ErrorContext.Provider>{props.children}</ErrorContext.Provider>;
};

export default ErrorContextProvider;
