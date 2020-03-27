import React, { useState } from "react";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import TextField from "@material-ui/core/TextField";
import Title from "../Title";
import formStateHandler from "../../utils/formStateHandler";

export default props => {
  const [passwordState, setPasswordState] = useState({
    oldPassword: "",
    newPassword: ""
  });

  const handleFormChanged = e => formStateHandler(e, setPasswordState);

  const submitButtonClickHandler = e => {
    e.preventDefault();
    props
      .onPasswordChanged(passwordState)
      .then(() => setPasswordState({ oldPassword: "", newPassword: "" }))
      .catch(err => console.error(err));
  };

  return (
    <form noValidate>
      <Title>Change password</Title>
      <Grid container spacing={2} direction="column" alignItems="flex-start">
        {!props.resetMode && (
          <Grid item>
            <TextField
              name="oldPassword"
              variant="outlined"
              type="password"
              required
              fullWidth
              id="oldPassword"
              label="Old Password"
              value={passwordState.oldPassword}
              onChange={handleFormChanged}
            />
          </Grid>
        )}
        <Grid item>
          <TextField
            variant="outlined"
            type="password"
            required
            fullWidth
            id="newPassword"
            label="New Password"
            name="newPassword"
            value={passwordState.newPassword}
            onChange={handleFormChanged}
          />
        </Grid>
        <Grid item>
          <Button
            type="submit"
            variant="contained"
            color="primary"
            onClick={submitButtonClickHandler}
          >
            Submit
          </Button>
        </Grid>
      </Grid>
    </form>
  );
};
