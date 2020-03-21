import React, { useContext } from "react";
import Grid from "@material-ui/core/Grid";
import DateFnsUtils from "@date-io/date-fns";
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker
} from "@material-ui/pickers";
import { makeStyles } from "@material-ui/core/styles";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import Typography from "@material-ui/core/Typography";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import { FilterContext } from "../context/filter-context";

const useStyles = makeStyles(theme => ({
  root: {
    width: "100%"
  },
  heading: {
    fontSize: theme.typography.pxToRem(15),
    fontWeight: theme.typography.fontWeightRegular
  }
}));

export default () => {
  const filterContext = useContext(FilterContext);

  const minDateChanged = date => filterContext.setMinDate(date);
  const maxDateChanged = date => filterContext.setMaxDate(date);

  const classes = useStyles();

  return (
    <div className={classes.root}>
      <ExpansionPanel>
        <ExpansionPanelSummary
          expandIcon={<ExpandMoreIcon />}
          aria-controls="panel1a-content"
          id="panel1a-header"
        >
          <Typography className={classes.heading}>Filters</Typography>
        </ExpansionPanelSummary>
        <ExpansionPanelDetails>
          <MuiPickersUtilsProvider utils={DateFnsUtils}>
            <Grid container justify="flex-start" spacing={4}>
              <Grid key="1" item>
                <KeyboardDatePicker
                  disableToolbar
                  variant="inline"
                  format="yyyy.MM.dd"
                  margin="normal"
                  id="filter-min-date"
                  label="Date from"
                  value={filterContext.minDate}
                  onChange={minDateChanged}
                  KeyboardButtonProps={{
                    "aria-label": "change date"
                  }}
                  maxDate={new Date()}
                />
              </Grid>
              <Grid key="2" item>
                <KeyboardDatePicker
                  disableToolbar
                  variant="inline"
                  format="yyyy.MM.dd"
                  margin="normal"
                  id="filter-max-date"
                  label="Date to"
                  value={filterContext.maxDate}
                  onChange={maxDateChanged}
                  KeyboardButtonProps={{
                    "aria-label": "change date"
                  }}
                  maxDate={new Date()}
                />
              </Grid>
            </Grid>
          </MuiPickersUtilsProvider>
        </ExpansionPanelDetails>
      </ExpansionPanel>
    </div>
  );
};
