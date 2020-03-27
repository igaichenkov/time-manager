import React, { useContext, useEffect, useCallback } from "react";
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

  const handleSetError = useCallback(
    message => enqueueSnackbar(message, { variant: "error" }),
    [enqueueSnackbar]
  );

  const errorHandler = useCallback(
    error => {
      if (error.response.status === 401) {
        authContext.setUnauthenticated();
      } else if (error.response.data && error.response.data.errors) {
        error.response.data.errors.forEach(errorItem =>
          handleSetError(errorItem.description)
        );
      }

      return Promise.reject({ ...error });
    },
    [authContext, handleSetError]
  );

  const interceptor = axios.interceptors.response.use(null, errorHandler);

  useEffect(() => {
    return () => axios.interceptors.response.eject(interceptor);
  }, [errorHandler, interceptor]);

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
