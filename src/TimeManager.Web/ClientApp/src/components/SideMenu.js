import React from 'react';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import DashboardIcon from '@material-ui/icons/Dashboard';
import ExitToApp from '@material-ui/icons/ExitToApp';
import AccountCircle from '@material-ui/icons/AccountCircle';

export default () => (
  <List>
    <ListItem button>
      <ListItemIcon>
        <DashboardIcon />
      </ListItemIcon>
      <ListItemText primary="Dashboard" />
    </ListItem>
    <ListItem button>
      <ListItemIcon>
        <AccountCircle />
      </ListItemIcon>
      <ListItemText primary="Profile" />
    </ListItem>
    <ListItem button>
      <ListItemIcon>
        <ExitToApp />
      </ListItemIcon>
      <ListItemText primary="Logout" />
    </ListItem>
  </List>
);
