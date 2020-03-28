import React, { useContext, useCallback } from "react";
import { Switch, Route, Redirect } from "react-router-dom";
import CssBaseline from "@material-ui/core/CssBaseline";
import Box from "@material-ui/core/Box";
import Container from "@material-ui/core/Container";
import Copyright from "../Copyright";
import makeStyles from "./Dashboard.styles.js";
import { AuthContext } from "../../context/AuthContext";
import FilterContextProvider from "../../context/FilterContext";
import Profile from "../Profile/Profile";
import UsersList from "../Users/UsersList";
import roles from "../../utils/roles";
import SideBar from "./SideBar";
import WorkEntries from "../WorkEntries/WorkEntries";
import Filters from "./Filters";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import * as AccountStore from "../../stores/AccountStore";

const Dashboard = () => {
  const classes = makeStyles();

  const authContext = useContext(AuthContext);

  const isReadOnlyMode = pageUserId =>
    authContext.account.profile.roleName !== roles.Admin &&
    pageUserId !== authContext.account.profile.id;

  const updateUserProfile = useCallback(
    (userId, profile) => {
      if (!userId) {
        authContext.updateProfile(profile);
      } else {
        AccountStore.updateUserProfile(userId, profile);
      }
    },
    [authContext]
  );

  if (!authContext.account.isAuthentecated) {
    return <Redirect to="/signin" />;
  }

  const getUserIdFromRoute = p => {
    const routeUserId = p.match.params.userId;
    return routeUserId === authContext.account.profile.id ? 0 : routeUserId;
  };

  return (
    <FilterContextProvider>
      <div className={classes.root}>
        <CssBaseline />
        <SideBar />
        <main className={classes.content}>
          <div className={classes.appBarSpacer} />
          <Container maxWidth="lg" className={classes.container}>
            <Switch>
              <Route path="/dashboard/users">
                <UsersList />
              </Route>

              <Route
                path="/dashboard/profile/:userId"
                render={p => (
                  <Profile
                    userId={getUserIdFromRoute(p)}
                    onProfileSaved={updateUserProfile}
                  />
                )}
              />

              <Route path="/dashboard/profile">
                <Profile onProfileSaved={updateUserProfile} />
              </Route>

              <Route
                path="/dashboard/:userId"
                render={p => (
                  <WorkEntriesPage
                    userId={getUserIdFromRoute(p)}
                    readOnly={isReadOnlyMode(p.match.params.userId)}
                  />
                )}
              ></Route>

              <Route path="/dashboard">
                <WorkEntriesPage readOnly={false} />
              </Route>
            </Switch>
            <Box pt={4}>
              <Copyright />
            </Box>
          </Container>
        </main>
      </div>
    </FilterContextProvider>
  );
};

const WorkEntriesPage = React.memo(props => {
  const classes = makeStyles();

  return (
    <Grid container spacing={3}>
      <Grid item xs={12}>
        <Filters />
      </Grid>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
          <WorkEntries userId={props.userId} readOnly={props.readOnly} />
        </Paper>
      </Grid>
    </Grid>
  );
});

export default Dashboard;
