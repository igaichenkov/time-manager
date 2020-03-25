import React, { useContext, useState, useEffect } from "react";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import TextField from "@material-ui/core/TextField";
import Title from "./Title";
import { AuthContext } from "../context/AuthContext";
import makeStyles from "./Profile.styles.js";
import ChangePassword from "./ChangePassword";
import formStateHandler from "../utils/formStateHandler";

export default () => {
  const classes = makeStyles();
  const authContext = useContext(AuthContext);
  const [profileState, setProfileState] = useState({
    ...authContext.account.profile
  });

  useEffect(
    () =>
      setProfileState({
        ...authContext.account.profile
      }),
    [authContext.account.profile]
  );

  const greetingName = authContext.account.profile.firstName
    ? authContext.account.profile.firstName
    : authContext.account.profile.email;

  const handleFormChanged = e => formStateHandler(e, setProfileState);

  const updateProfileHandler = () => authContext.updateProfile(profileState);

  return (
    <Paper className={classes.paper}>
      <Title>Hi {greetingName}!</Title>

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

      <ChangePassword />
    </Paper>
  );
};
