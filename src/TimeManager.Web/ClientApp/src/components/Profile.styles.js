import { makeStyles } from "@material-ui/core/styles";

export default makeStyles(theme => ({
  paper: {
    padding: theme.spacing(2),
    display: "flex",
    overflow: "auto",
    flexDirection: "column"
  },

  profile: {
    paddingTop: theme.spacing(2),
    paddingBottom: theme.spacing(4)
  }
}));
