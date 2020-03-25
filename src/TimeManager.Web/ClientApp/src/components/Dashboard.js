import React, { useContext } from "react";
import { Switch, Route, Redirect } from "react-router-dom";
import clsx from "clsx";
import CssBaseline from "@material-ui/core/CssBaseline";
import Drawer from "@material-ui/core/Drawer";
import Box from "@material-ui/core/Box";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";
import Divider from "@material-ui/core/Divider";
import IconButton from "@material-ui/core/IconButton";
import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import MenuIcon from "@material-ui/icons/Menu";
import ChevronLeftIcon from "@material-ui/icons/ChevronLeft";
import SideMenu from "./SideMenu";
import Copyright from "./Copyright";
import WorkEntries from "./WorkEntries/WorkEntries";
import Filters from "./Filters";
import makeStyles from "./Dashboard.styles.js";
import { AuthContext } from "../context/AuthContext";
import FilterContextProvider from "../context/FilterContext";
import Profile from "./Profile";

const Dashboard = () => {
  const classes = makeStyles();

  const [open, setOpen] = React.useState(true);
  const handleDrawerOpen = () => {
    setOpen(true);
  };
  const handleDrawerClose = () => {
    setOpen(false);
  };

  const authContext = useContext(AuthContext);

  if (!authContext.account.isAuthentecated) {
    return <Redirect to="/signin" />;
  }

  return (
    <FilterContextProvider>
      <div className={classes.root}>
        <CssBaseline />
        <AppBar
          position="absolute"
          className={clsx(classes.appBar, open && classes.appBarShift)}
        >
          <Toolbar className={classes.toolbar}>
            <IconButton
              edge="start"
              color="inherit"
              aria-label="open drawer"
              onClick={handleDrawerOpen}
              className={clsx(
                classes.menuButton,
                open && classes.menuButtonHidden
              )}
            >
              <MenuIcon />
            </IconButton>
            <Typography
              component="h1"
              variant="h6"
              color="inherit"
              noWrap
              className={classes.title}
            >
              Dashboard
            </Typography>
          </Toolbar>
        </AppBar>
        <Drawer
          variant="permanent"
          classes={{
            paper: clsx(classes.drawerPaper, !open && classes.drawerPaperClose)
          }}
          open={open}
        >
          <div className={classes.toolbarIcon}>
            <IconButton onClick={handleDrawerClose}>
              <ChevronLeftIcon />
            </IconButton>
          </div>
          <Divider />
          <SideMenu />
          <Divider />
        </Drawer>
        <main className={classes.content}>
          <div className={classes.appBarSpacer} />
          <Container maxWidth="lg" className={classes.container}>
            <Switch>
              <Route path="/dashboard/profile">
                <Profile />
              </Route>

              <Route path="/dashboard">
                <Grid container spacing={3}>
                  <Grid item xs={12}>
                    <Filters />
                  </Grid>
                  <Grid item xs={12}>
                    <Paper className={classes.paper}>
                      <WorkEntries />
                    </Paper>
                  </Grid>
                </Grid>
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

export default Dashboard;
