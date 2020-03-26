import React, { useContext, useState, useEffect } from "react";
import Avatar from "@material-ui/core/Avatar";
import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import LockOutlinedIcon from "@material-ui/icons/LockOutlined";
import Typography from "@material-ui/core/Typography";
import Container from "@material-ui/core/Container";
import Copyright from "../Copyright";
import makeStyles from "./SignIn.styles";
import { AuthContext } from "../../context/AuthContext";
import { useHistory, Link } from "react-router-dom";
import formStateHandler from "../../utils/formStateHandler";

export default function SignIn() {
  const authContext = useContext(AuthContext);
  const [loginFormState, setLoginFormState] = useState({
    email: "mail2@test.com",
    password: "ABCD1234asd123#",
    rememberMe: false
  });

  const classes = makeStyles();
  const history = useHistory();

  useEffect(() => {
    if (authContext.account.isAuthentecated) {
      history.push("/");
    }
  }, [authContext, history]);

  const loginHandler = e => {
    e.preventDefault();

    authContext
      .login(loginFormState)
      .then(() => history.push("/dashboard"))
      .catch(err => console.error(err));
  };

  const loginFormChanged = e => formStateHandler(e, setLoginFormState);

  const rememberMeChanged = event => {
    const newRememberMe = event.target.checked;
    setLoginFormState(prevState => ({
      ...prevState,
      rememberMe: newRememberMe
    }));
  };

  return (
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <div className={classes.paper}>
        <Avatar className={classes.avatar}>
          <LockOutlinedIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Sign in
        </Typography>
        <form className={classes.form} noValidate>
          <TextField
            variant="outlined"
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
            autoFocus
            value={loginFormState.email}
            onChange={loginFormChanged}
          />
          <TextField
            variant="outlined"
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="current-password"
            value={loginFormState.password}
            onChange={loginFormChanged}
          />
          <FormControlLabel
            control={
              <Checkbox
                color="primary"
                value="remember"
                checked={loginFormState.rememberMe}
                onChange={rememberMeChanged}
              />
            }
            label="Remember me"
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            color="primary"
            className={classes.submit}
            onClick={loginHandler}
          >
            Sign In
          </Button>
          <Grid container>
            <Grid item>
              <Link to="/signup">{"Don't have an account? Sign Up"}</Link>
            </Grid>
          </Grid>
        </form>
      </div>
      <Box mt={8}>
        <Copyright />
      </Box>
    </Container>
  );
}
