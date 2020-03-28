import React, { useEffect, useState, useContext } from "react";
import * as usersStore from "../../stores/UsersStore";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Title from "../Title";
import UserRow from "./UserRow";
import { useHistory, Redirect } from "react-router-dom";
import { AuthContext } from "../../context/AuthContext";
import roles from "../../utils/roles";

export default () => {
  const [users, setUsers] = useState([]);
  const authContext = useContext(AuthContext);

  const currentUserRoleName = authContext.account.profile.roleName;

  useEffect(() => {
    usersStore.getUsersList().then(resp => setUsers(resp.data));
  }, [setUsers]);

  const history = useHistory();

  const handleViewUserLog = userId => history.push("/dashboard/" + userId);
  const handleProfileOpen = userId =>
    history.push("/dashboard/profile/" + userId);

  if (!roles.UserManagerRoles.includes(authContext.account.profile.roleName)) {
    return <Redirect to="/dashboard" />;
  }

  return (
    <React.Fragment>
      <Title>Users</Title>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Username</TableCell>
            <TableCell>Role</TableCell>
            <TableCell>First Name</TableCell>
            <TableCell>Last Name</TableCell>
            <TableCell>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map(row => (
            <UserRow
              key={row.id}
              user={row}
              onWorkLogOpen={handleViewUserLog}
              onProfileOpen={handleProfileOpen}
              canViewLog={currentUserRoleName === roles.Admin}
            />
          ))}
        </TableBody>
      </Table>
    </React.Fragment>
  );
};
