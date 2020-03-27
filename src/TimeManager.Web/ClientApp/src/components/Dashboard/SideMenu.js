import React, { useContext } from "react";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import DashboardIcon from "@material-ui/icons/Dashboard";
import ExitToApp from "@material-ui/icons/ExitToApp";
import PersonIcon from "@material-ui/icons/Person";
import { AuthContext } from "../../context/AuthContext";
import { useHistory } from "react-router-dom";
import roles from "../../utils/roles";
import SupervisorAccountIcon from "@material-ui/icons/SupervisorAccount";

export default () => {
  const authContext = useContext(AuthContext);
  const history = useHistory();

  const logoutHandler = () => {
    authContext.signOut();
  };

  const navigateTo = addr => history.push(addr);

  return (
    <List>
      <ListItem button onClick={() => navigateTo("/dashboard")}>
        <ListItemIcon>
          <DashboardIcon />
        </ListItemIcon>
        <ListItemText primary="Dashboard" />
      </ListItem>
      {[roles.Admin, roles.Manager].includes(
        authContext.account.profile.roleName
      ) && (
        <ListItem button onClick={() => navigateTo("/dashboard/users")}>
          <ListItemIcon>
            <SupervisorAccountIcon />
          </ListItemIcon>
          <ListItemText primary="Users" />
        </ListItem>
      )}
      <ListItem button onClick={() => navigateTo("/dashboard/profile")}>
        <ListItemIcon>
          <PersonIcon />
        </ListItemIcon>
        <ListItemText primary="Profile" />
      </ListItem>
      <ListItem button onClick={logoutHandler}>
        <ListItemIcon>
          <ExitToApp />
        </ListItemIcon>
        <ListItemText primary="Logout" />
      </ListItem>
    </List>
  );
};
