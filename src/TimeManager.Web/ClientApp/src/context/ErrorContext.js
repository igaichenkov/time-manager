import React, { useContext, useEffect } from "react";
import axios from "../utils/axios";
import { useSnackbar, SnackbarProvider } from "notistack";
import { AuthContext } from "./AuthContext";

const createEmptyErrorState = () => ({
  addError: () => {}
});

export const ErrorContext = React.createContext(createEmptyErrorState());

const ErrorContextProvider = props => {
  const { enqueueSnackbar } = useSnackbar();
  const authContext = useContext(AuthContext);

  const errorHandler = error => {
    if (error.response.status === 401) {
      authContext.setUnauthenticated();
    } else if (error.response.data && error.response.data.errors) {
      error.response.data.errors.forEach(errorItem =>
        handleSetError(errorItem.description)
      );
    }

    return Promise.reject({ ...error });
  };

  const interceptor = axios.interceptors.response.use(null, errorHandler);

  useEffect(() => {
    return () => axios.interceptors.response.eject(interceptor);
  }, [errorHandler]);

  const handleSetError = message =>
    enqueueSnackbar(message, { variant: "error" });

  return (
    <ErrorContext.Provider
      value={{
        addError: handleSetError
      }}
    >
      {props.children}
    </ErrorContext.Provider>
  );
};

const withSnackbarContext = WrappedComponent => {
  return props => (
    <SnackbarProvider maxSnack={3}>
      <WrappedComponent {...props} />
    </SnackbarProvider>
  );
};

export default withSnackbarContext(ErrorContextProvider);
