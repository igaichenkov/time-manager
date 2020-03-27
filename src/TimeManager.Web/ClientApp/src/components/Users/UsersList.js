import React, { useEffect, useState } from "react";
import * as usersStore from "./UsersStore";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Title from "../Title";
import UserRow from "./UserRow";
import { useHistory } from "react-router-dom";

export default props => {
  const [users, setUsers] = useState([]);

  useEffect(() => {
    usersStore.getUsersList().then(resp => setUsers(resp.data));
  }, [setUsers]);

  const history = useHistory();

  const handleViewUserLog = userId => history.push("/dashboard/" + userId);

  return (
    <React.Fragment>
      <Title>Users</Title>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Username</TableCell>
            <TableCell>Role</TableCell>
            <TableCell>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map(row => (
            <UserRow
              key={row.id}
              user={row}
              onWorkLogOpen={handleViewUserLog}
            />
          ))}
        </TableBody>
      </Table>
    </React.Fragment>
  );
};
