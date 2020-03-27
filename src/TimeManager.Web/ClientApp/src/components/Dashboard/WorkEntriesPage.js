import React from "react";
import Filters from "./Filters";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import { makeStyles } from "@material-ui/core/styles";
import WorkEntries from "../WorkEntries/WorkEntries";

const styles = makeStyles(theme => ({
  paper: {
    padding: theme.spacing(2),
    display: "flex",
    overflow: "auto",
    flexDirection: "column"
  }
}));

export default props => {
  const classes = styles();

  return (
    <Grid container spacing={3}>
      <Grid item xs={12}>
        <Filters />
      </Grid>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
          <WorkEntries userId={props.userId} />
        </Paper>
      </Grid>
    </Grid>
  );
};
