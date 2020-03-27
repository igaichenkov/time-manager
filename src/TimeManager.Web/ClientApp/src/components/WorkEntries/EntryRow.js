import React from "react";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";
import TableCell from "@material-ui/core/TableCell";
import TableRow from "@material-ui/core/TableRow";
import Button from "@material-ui/core/Button";
import dateformat from "dateformat";

export default props => {
  const resolveRowClass = workEntry => {
    if (props.preferredMinHours) {
      return workEntry.hoursSpent >= props.preferredMinHours
        ? "entryTimeLimitOk"
        : "entryUnderTimeLimit";
    }

    return null;
  };

  return (
    <TableRow className={resolveRowClass(props.workEntry)}>
      <TableCell>{dateformat(props.workEntry.date, "yyyy.mm.dd")}</TableCell>
      <TableCell>{props.workEntry.hoursSpent}</TableCell>
      <TableCell>{props.workEntry.notes}</TableCell>
      <TableCell>
        <Button
          onClick={() => props.onEdit(props.workEntry.id)}
          disabled={props.readOnly}
        >
          <EditIcon fontSize="small" />
        </Button>
        <Button
          onClick={() => props.onDelete(props.workEntry.id)}
          disabled={props.readOnly}
        >
          <DeleteIcon fontSize="small" />
        </Button>
      </TableCell>
    </TableRow>
  );
};
