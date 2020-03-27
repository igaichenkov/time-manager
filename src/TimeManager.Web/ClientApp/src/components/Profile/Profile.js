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
  fetchProfile,
  fetchUserProfile,
  changePassword,
  resetPassword
} from "../../stores/AccountStore";

export default props => {
  const classes = makeStyles();
  const [profileState, setProfileState] = useState({
    firstName: "",
    lastName: "",
    preferredHoursPerDay: 0
  });

  const { userId, onProfileSaved } = props;

  useEffect(() => {
    const promise = userId ? fetchUserProfile(userId) : fetchProfile();
    promise.then(resp => {
      setProfileState(resp.data);
    });
  }, [userId, setProfileState]);

  const greetingName = profileState.firstName
    ? profileState.firstName
    : profileState.userName;

  const greeting = userId ? greetingName + " profile" : `Hi, ${greetingName}!`;

  const handleFormChanged = e => formStateHandler(e, setProfileState);

  const updateProfileHandler = () => onProfileSaved(userId, profileState);

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
          <TextField
            variant="outlined"
            id="preferredHoursPerDay"
            label="Preferred Hours Per Day"
            name="preferredHoursPerDay"
            type="number"
            value={profileState.preferredHoursPerDay}
            onChange={handleFormChanged}
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
        resetMode={userId}
        onPasswordChanged={handleChangePassword}
      />
    </Paper>
  );
};
