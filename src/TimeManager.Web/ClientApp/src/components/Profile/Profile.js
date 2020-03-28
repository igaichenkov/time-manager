import React, { useState, useEffect } from "react";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import TextField from "@material-ui/core/TextField";
import Title from "../Title";
import makeStyles from "./Profile.styles.js";
import ChangePassword from "./ChangePassword";
import formStateHandler from "../../utils/formStateHandler";
import {
  fetchUserProfile,
  changePassword,
  resetPassword
} from "../../stores/AccountStore";
import HoursInput from "../HoursInput";

export default props => {
  const classes = makeStyles();
  const [profileState, setProfileState] = useState({
    firstName: "",
    lastName: "",
    preferredHoursPerDay: 0
  });

  const [isFormValid, setFormValid] = useState(true);
  const { userId, onProfileSaved } = props;

  useEffect(() => {
    fetchUserProfile(userId).then(resp => {
      setProfileState({
        firstName: resp.data.firstName || "",
        lastName: resp.data.lastName || "",
        preferredHoursPerDay: resp.data.preferredHoursPerDay
      });
    });
  }, [userId, setProfileState]);

  const getGreetingName = () =>
    profileState.firstName ? profileState.firstName : profileState.userName;

  const greeting = userId
    ? profileState.userName + " profile"
    : `Hi, ${getGreetingName()}!`;

  const handleFormChanged = e => formStateHandler(e, setProfileState);
  const handleErrorStateChanged = state => setFormValid(state.isValid);

  const updateProfileHandler = () => {
    if (isFormValid) {
      return onProfileSaved(userId, profileState);
    }
  };

  const handleChangePassword = passwords =>
    userId
      ? resetPassword(userId, passwords.newPassword)
      : changePassword(passwords);

  return (
    <Paper className={classes.paper}>
      <Title>{greeting}</Title>

      <Grid
        container
        alignItems="flex-start"
        spacing={2}
        item
        direction="column"
        className={classes.profile}
      >
        <Grid item>
          <TextField
            autoComplete="fname"
            name="firstName"
            variant="outlined"
            id="firstName"
            label="First Name"
            autoFocus
            value={profileState.firstName}
            onChange={handleFormChanged}
          />
        </Grid>
        <Grid item>
          <TextField
            variant="outlined"
            id="lastName"
            label="Last Name"
            name="lastName"
            autoComplete="lname"
            value={profileState.lastName}
            onChange={handleFormChanged}
          />
        </Grid>
        <Grid item>
          <HoursInput
            variant="outlined"
            id="preferredHoursPerDay"
            label="Preferred Hours Per Day"
            name="preferredHoursPerDay"
            allowMin={true}
            min={0}
            max={24}
            value={profileState.preferredHoursPerDay}
            onChange={handleFormChanged}
            errorStateChanged={handleErrorStateChanged}
          />
        </Grid>
        <Grid item>
          <Button
            type="submit"
            color="primary"
            variant="contained"
            onClick={updateProfileHandler}
          >
            Save
          </Button>
        </Grid>
      </Grid>

      <ChangePassword
        resetMode={!!userId}
        onPasswordChanged={handleChangePassword}
      />
    </Paper>
  );
};
