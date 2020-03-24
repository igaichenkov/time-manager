import React from "react";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";
import TableCell from "@material-ui/core/TableCell";
import TableRow from "@material-ui/core/TableRow";
import Button from "@material-ui/core/Button";
import dateformat from "dateformat";

const RowActions = ({ id, onEdit, onDelete }) => (
  <React.Fragment>
    <Button onClick={() => onEdit(id)}>
      <EditIcon fontSize="small" />
    </Button>
    <Button>
      <DeleteIcon fontSize="small" onClick={() => onDelete(id)} />
    </Button>
  </React.Fragment>
);

export default props => {
  const resolveRowClass = workEntry => {
    if (props.preferredMinHours) {
      return workEntry.hoursSpent > props.preferredMinHours
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
        <RowActions
          id={props.workEntry.id}
          onEdit={props.onEdit}
          onDelete={props.onDelete}
        />
      </TableCell>
    </TableRow>
  );
};
