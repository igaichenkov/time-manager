import React from "react";
import Link from "@material-ui/core/Link";
import { makeStyles } from "@material-ui/core/styles";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import dateformat from "dateformat";
import Title from "./Title";

// Generate Order Data
function createData(id, date, durations, notes) {
  return { id, date, durations, notes };
}

const rows = [
  createData(0, new Date("2020-03-17"), [3, 5], "Note1, note2, note3, note4"),
  createData(1, new Date("2020-03-16"), [2], "Note1, note2, note3, note4"),
  createData(2, new Date("2020-03-15"), [4], "Note1, note2, note3, note4")
];

function preventDefault(event) {
  event.preventDefault();
}

const useStyles = makeStyles(theme => ({
  seeMore: {
    marginTop: theme.spacing(3)
  }
}));

export default function WorkEntries() {
  const classes = useStyles();
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
          {rows.map(row => (
            <TableRow key={row.id}>
              <TableCell>{dateformat(row.date, "yyyy.mm.dd")}</TableCell>
              <TableCell>
                {row.durations.map((i, duration) => (
                  <p key={i}>{duration}h</p>
                ))}
              </TableCell>
              <TableCell>{row.notes}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      <div className={classes.seeMore}>
        <Link color="primary" href="#" onClick={preventDefault}>
          See more orders
        </Link>
      </div>
    </React.Fragment>
  );
}
