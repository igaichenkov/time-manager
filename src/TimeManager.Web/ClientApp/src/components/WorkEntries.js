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
import axios from "axios";
import Button from "@material-ui/core/Button";
import AddEntryDialog from "./AddEntryDialog";

const useStyles = makeStyles(theme => ({
  seeMore: {
    marginTop: theme.spacing(3)
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

export default function WorkEntries() {
  const classes = useStyles();
  const filterContext = useContext(FilterContext);
  const [entries, setEntries] = useState([]);
  const [isEntryDialogOpen, setEntryDialogOpen] = React.useState(false);

  const handleEntryDialogOpen = () => {
    setEntryDialogOpen(true);
  };

  const handleEntryDialogClose = () => {
    setEntryDialogOpen(false);
  };

  const refresh = () => {
    axios
      .get(buildRequestUrl(filterContext))
      .then(resp => setEntries(resp.data))
      .catch(err => console.error(err));
  };

  const handleSaveEntry = formState => {
    axios
      .post("/api/WorkEntries", {
        date: dateformat(formState.date, "yyyy-mm-dd"),
        hoursSpent: parseFloat(formState.hoursSpent),
        notes: formState.notes
      })
      .then(() => handleEntryDialogClose())
      .then(() => refresh())
      .catch(err => console.error(err));
  };

  useEffect(() => refresh(), [filterContext]);

  return (
    <React.Fragment>
      <Title>Work entries</Title>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Date</TableCell>
            <TableCell>Duration</TableCell>
            <TableCell>Notes</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {entries.map(row => (
            <TableRow key={row.id}>
              <TableCell>{dateformat(row.date, "yyyy.mm.dd")}</TableCell>
              <TableCell>{row.hoursSpent}</TableCell>
              <TableCell>{row.notes}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      <div className={classes.seeMore}>
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
      />
    </React.Fragment>
  );
}
