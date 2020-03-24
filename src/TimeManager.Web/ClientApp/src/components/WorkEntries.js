import React, { useContext, useEffect, useState } from "react";
import Grid from "@material-ui/core/Grid";
import { makeStyles } from "@material-ui/core/styles";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import dateformat from "dateformat";
import Title from "./Title";
import { FilterContext } from "../context/filter-context";
import { AuthContext } from "../context/auth-context";
import axios from "axios";
import Button from "@material-ui/core/Button";
import AddEntryDialog from "./AddEntryDialog";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";

const useStyles = makeStyles(theme => ({
  actionPanel: {
    marginTop: theme.spacing(3)
  },
  entryUnderTimeLimit: {
    color: "red"
  },
  entryTimeLimitOk: {
    color: "green"
  }
}));

const buildRequestUrl = filter => {
  const queryParams = [];
  const basePath = "/api/WorkEntries";

  if (filter.minDate) {
    queryParams.push("minDate=" + dateformat(filter.minDate, "yyyy-mm-dd"));
  }

  if (filter.maxDate) {
    queryParams.push("maxDate=" + dateformat(filter.maxDate, "yyyy-mm-dd"));
  }

  return queryParams.length > 0
    ? basePath + "?" + queryParams.join("&")
    : basePath;
};

const RowActions = React.memo(({ id, onEdit, onDelete }) => (
  <React.Fragment>
    <Button onClick={() => onEdit(id)}>
      <EditIcon fontSize="small" />
    </Button>
    <Button>
      <DeleteIcon fontSize="small" onClick={() => onDelete(id)} />
    </Button>
  </React.Fragment>
));

export default function WorkEntries() {
  const classes = useStyles();
  const filterContext = useContext(FilterContext);
  const authContext = useContext(AuthContext);

  const [entries, setEntries] = useState([]);
  const [isEntryDialogOpen, setEntryDialogOpen] = useState(false);
  const [selectedEntry, setSelectedEntry] = useState(null);

  const handleEntryDialogOpen = () => {
    setEntryDialogOpen(true);
    setSelectedEntry(null);
  };

  const handleEntryDialogClose = () => {
    setEntryDialogOpen(false);
  };

  const resolveRowClass = workEntry => {
    if (authContext.account.profile.preferredHoursPerDay) {
      return workEntry.hoursSpent >
        authContext.account.profile.preferredHoursPerDay
        ? "entryTimeLimitOk"
        : "entryUnderTimeLimit";
    }

    return null;
  };

  const refresh = () => {
    axios
      .get(buildRequestUrl(filterContext))
      .then(resp => setEntries(resp.data))
      .catch(err => console.error(err));
  };

  useEffect(() => refresh(), [filterContext]);

  const handleSaveEntry = formState => {
    const payload = {
      date: dateformat(formState.date, "yyyy-mm-dd"),
      hoursSpent: parseFloat(formState.hoursSpent),
      notes: formState.notes
    };

    const promise = formState.id
      ? axios.put(`/api/WorkEntries/${formState.id}`, payload)
      : axios.post("/api/WorkEntries", payload);

    promise
      .then(() => handleEntryDialogClose())
      .then(() => refresh())
      .catch(err => console.error(err));
  };

  const handleEditRow = id => {
    setSelectedEntry(id);
    setEntryDialogOpen(true);
  };

  const handleDeleteRow = id => {
    axios
      .delete(`/api/WorkEntries/${id}`)
      .then(() => refresh())
      .catch(err => console.error(err));
  };

  return (
    <React.Fragment>
      <Title>Work entries</Title>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Date</TableCell>
            <TableCell>Duration</TableCell>
            <TableCell>Notes</TableCell>
            <TableCell>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {entries.map(row => (
            <TableRow key={row.id} className={resolveRowClass(row)}>
              <TableCell>{dateformat(row.date, "yyyy.mm.dd")}</TableCell>
              <TableCell>{row.hoursSpent}</TableCell>
              <TableCell>{row.notes}</TableCell>
              <TableCell>
                <RowActions
                  id={row.id}
                  onEdit={handleEditRow}
                  onDelete={handleDeleteRow}
                />
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      <div className={classes.actionPanel}>
        <Grid container spacing={1}>
          <Grid item>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              onClick={handleEntryDialogOpen}
            >
              Add
            </Button>
          </Grid>
          <Grid item>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="default"
              onClick={refresh}
            >
              Refresh
            </Button>
          </Grid>
        </Grid>
      </div>
      <AddEntryDialog
        isOpen={isEntryDialogOpen}
        onClose={handleEntryDialogClose}
        onEntrySaved={handleSaveEntry}
        entryId={selectedEntry}
      />
    </React.Fragment>
  );
}
