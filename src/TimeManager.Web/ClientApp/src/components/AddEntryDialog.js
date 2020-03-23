import React, { useState, useEffect } from "react";
import { makeStyles } from "@material-ui/core/styles";
import Modal from "@material-ui/core/Modal";
import Backdrop from "@material-ui/core/Backdrop";
import Fade from "@material-ui/core/Fade";
import Grid from "@material-ui/core/Grid";
import DateFnsUtils from "@date-io/date-fns";
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker
} from "@material-ui/pickers";
import TextField from "@material-ui/core/TextField";
import Button from "@material-ui/core/Button";

const useStyles = makeStyles(theme => ({
  modal: {
    display: "flex",
    alignItems: "center",
    justifyContent: "center"
  },
  paper: {
    backgroundColor: theme.palette.background.paper,
    border: "2px solid #000",
    boxShadow: theme.shadows[5],
    padding: theme.spacing(2, 4, 3)
  }
}));

export default function AddEntryDialog(props) {
  const classes = useStyles();

  const [formState, setFormState] = useState({
    date: new Date(),
    hoursSpent: 0,
    notes: ""
  });

  useEffect(
    () =>
      setFormState({
        date: new Date(),
        hoursSpent: 0,
        notes: ""
      }),
    [props.isOpen]
  );

  const handleFormChanged = (fieldName, fieldValue) => {
    setFormState(prevState => {
      const newState = { ...prevState };
      newState[fieldName] = fieldValue;

      return newState;
    });
  };

  const handleTextFieldChanged = e =>
    handleFormChanged(e.target.name, e.target.value);

  const handleEntryDateChanged = date => handleFormChanged("date", date);

  const handleAddBtnClicked = e => {
    e.preventDefault();

    if (props.onEntrySaved) {
      props.onEntrySaved(formState);
    }
  };

  return (
    <div>
      <Modal
        aria-labelledby="transition-modal-title"
        aria-describedby="transition-modal-description"
        className={classes.modal}
        open={props.isOpen}
        onClose={props.onClose}
        closeAfterTransition
        BackdropComponent={Backdrop}
        BackdropProps={{
          timeout: 500
        }}
      >
        <Fade in={props.isOpen}>
          <div className={classes.paper}>
            <h2 id="transition-modal-title">Add work entry</h2>
            <MuiPickersUtilsProvider utils={DateFnsUtils}>
              <form noValidate>
                <Grid container spacing={2}>
                  <Grid item xs={12} sm={6}>
                    <KeyboardDatePicker
                      autoOk
                      variant="inline"
                      format="yyyy.MM.dd"
                      margin="normal"
                      id="filter-date"
                      label="Date"
                      value={formState.date}
                      onChange={handleEntryDateChanged}
                      KeyboardButtonProps={{
                        "aria-label": "change date"
                      }}
                      maxDate={new Date()}
                    />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField
                      margin="normal"
                      type="number"
                      required
                      fullWidth
                      id="hoursSpent"
                      label="Hours spent"
                      name="hoursSpent"
                      value={formState.hoursSpent}
                      onChange={handleTextFieldChanged}
                      InputLabelProps={{ shrink: true }}
                    />
                  </Grid>
                </Grid>
                <Grid
                  container
                  direction="column"
                  justify="flex-start"
                  spacing={2}
                >
                  <Grid item>
                    <TextField
                      margin="normal"
                      required
                      fullWidth
                      id="notes"
                      label="Notes"
                      name="notes"
                      value={formState.notes}
                      onChange={handleTextFieldChanged}
                      multiline
                      variant="outlined"
                    />
                  </Grid>

                  <Grid item>
                    <Button
                      type="submit"
                      fullWidth
                      variant="contained"
                      color="primary"
                      onClick={handleAddBtnClicked}
                    >
                      Save
                    </Button>
                  </Grid>
                </Grid>
              </form>
            </MuiPickersUtilsProvider>
          </div>
        </Fade>
      </Modal>
    </div>
  );
}
