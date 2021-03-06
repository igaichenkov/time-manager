import React, { useState, useEffect } from "react";
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
import useStyles from "./AddEntryDialog.styles";
import * as WorkEntriesStore from "../../stores/WorkEntriesStore";
import HoursInput from "../HoursInput";

const emptyState = {
  date: new Date(),
  hoursSpent: 1,
  notes: ""
};

export default props => {
  const classes = useStyles();

  const [formState, setFormState] = useState(emptyState);
  const [isFormValid, setFormValid] = useState(true);
  const { entryId, isOpen } = props;

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    if (entryId) {
      fetchWorkEntry(entryId);
    } else {
      setFormState(emptyState);
    }
  }, [entryId, isOpen]);

  const fetchWorkEntry = id => {
    WorkEntriesStore.getWorkEntryById(id)
      .then(resp => {
        setFormState(resp.data);
      })
      .catch(err => console.error(err));
  };

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

  const handleErrorStateChanged = state => setFormValid(state.isValid);

  const handleAddBtnClicked = e => {
    e.preventDefault();

    if (!isFormValid) {
      return;
    }

    if (props.onEntrySaved) {
      props.onEntrySaved(formState);
    }
  };

  return (
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
          <h2 id="transition-modal-title">
            {entryId ? "Edit" : "Add"} work entry
          </h2>
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
                  <HoursInput
                    margin="normal"
                    required
                    fullWidth
                    allowMin={false}
                    min={0}
                    max={24}
                    id="hoursSpent"
                    label="Hours spent"
                    name="hoursSpent"
                    value={formState.hoursSpent}
                    onChange={handleTextFieldChanged}
                    errorStateChanged={handleErrorStateChanged}
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
  );
};
