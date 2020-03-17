import React from 'react';
import Grid from '@material-ui/core/Grid';
import DateFnsUtils from '@date-io/date-fns';
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker
} from '@material-ui/pickers';
import { makeStyles } from '@material-ui/core/styles';
import ExpansionPanel from '@material-ui/core/ExpansionPanel';
import ExpansionPanelSummary from '@material-ui/core/ExpansionPanelSummary';
import ExpansionPanelDetails from '@material-ui/core/ExpansionPanelDetails';
import Typography from '@material-ui/core/Typography';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';

const useStyles = makeStyles(theme => ({
  root: {
    width: '100%'
  },
  heading: {
    fontSize: theme.typography.pxToRem(15),
    fontWeight: theme.typography.fontWeightRegular
  }
}));

export default () => {
  const [selectedDate, setSelectedDate] = React.useState(
    new Date('2014-08-18T21:11:54')
  );

  const handleDateChange = date => {
    setSelectedDate(date);
  };

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
                  id="date-picker-inline"
                  label="Date from"
                  value={selectedDate}
                  onChange={handleDateChange}
                  KeyboardButtonProps={{
                    'aria-label': 'change date'
                  }}
                />
              </Grid>
              <Grid key="2" item>
                <KeyboardDatePicker
                  disableToolbar
                  variant="inline"
                  format="yyyy.MM.dd"
                  margin="normal"
                  id="date-picker-inline"
                  label="Date to"
                  value={selectedDate}
                  onChange={handleDateChange}
                  KeyboardButtonProps={{
                    'aria-label': 'change date'
                  }}
                />
              </Grid>
            </Grid>
          </MuiPickersUtilsProvider>
        </ExpansionPanelDetails>
      </ExpansionPanel>
    </div>
  );
};
