import React from "react";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import { makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles(theme => ({
  actionPanel: {
    marginTop: theme.spacing(3)
  }
}));

export default props => {
  const classes = useStyles();

  return (
    <div className={classes.actionPanel}>
      <Grid container spacing={1}>
        <Grid item>
          <Button
            type="submit"
            fullWidth
            variant="contained"
            color="primary"
            onClick={props.onAddEntryClicked}
            disabled={props.readOnly}
          >
            Add
          </Button>
        </Grid>
        <Grid item>
          <Button
            type="submit"
            fullWidth
            variant="contained"
            color="default"
            onClick={props.onRefreshClicked}
          >
            Refresh
          </Button>
        </Grid>
      </Grid>
    </div>
  );
};
