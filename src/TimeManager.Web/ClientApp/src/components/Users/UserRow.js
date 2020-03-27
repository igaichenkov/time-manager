import React from "react";
import TableCell from "@material-ui/core/TableCell";
import TableRow from "@material-ui/core/TableRow";
import ListIcon from "@material-ui/icons/List";
import PersonIcon from "@material-ui/icons/Person";
import IconButton from "@material-ui/core/IconButton";
import Tooltip from "@material-ui/core/Tooltip";

export default props => {
  return (
    <TableRow>
      <TableCell>{props.user.userName}</TableCell>
      <TableCell>{props.user.roleName}</TableCell>
      <TableCell>{props.user.firstName}</TableCell>
      <TableCell>{props.user.lastName}</TableCell>
      <TableCell>
        {props.canViewLog && (
          <Tooltip title="View work log">
            <IconButton onClick={() => props.onWorkLogOpen(props.user.id)}>
              <ListIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        )}
        <Tooltip title="View profile">
          <IconButton onClick={() => props.onProfileOpen(props.user.id)}>
            <PersonIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      </TableCell>
    </TableRow>
  );
};
