import React from "react";
import axios from "../utils/axios";
import Backdrop from "@material-ui/core/Backdrop";
import CircularProgress from "@material-ui/core/CircularProgress";
import { makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles(theme => ({
  backdrop: {
    zIndex: theme.zIndex.drawer + 1,
    color: "#fff"
  }
}));

export default React.memo(props => {
  const classes = useStyles();

  const [open, setOpen] = React.useState(false);

  const handleClose = () => {
    setOpen(false);
  };

  const handlerSuccess = shouldOpen =>
    function(r) {
      setOpen(shouldOpen);
      return Promise.resolve(r);
    };

  const handlerRejected = shouldOpen =>
    function(r) {
      setOpen(false);
      return Promise.reject(r);
    };

  axios.interceptors.request.use(handlerSuccess(true), handlerRejected(true));
  axios.interceptors.response.use(
    handlerSuccess(false),
    handlerRejected(false)
  );

  return (
    <Backdrop className={classes.backdrop} open={open} onClick={handleClose}>
      <CircularProgress color="inherit" />
    </Backdrop>
  );
});
