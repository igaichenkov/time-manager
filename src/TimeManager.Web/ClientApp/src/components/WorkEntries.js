import React, { useContext, useEffect, useState } from "react";
import Link from "@material-ui/core/Link";
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

function preventDefault(event) {
  event.preventDefault();
}

const useStyles = makeStyles(theme => ({
  seeMore: {
    marginTop: theme.spacing(3)
  }
}));

const buildRequestUrl = filter => {
  const queryParams = [];
  const basePath = "/api/WorkEntries";

  if (filter.minDate) {
    queryParams.push("minDate=" + dateformat("yyyy-mm-dd", filter.minDate));
  }

  if (filter.maxDate) {
    queryParams.push("maxDate=" + dateformat("yyyy-mm-dd", filter.maxDate));
  }

  return queryParams.length > 0
    ? basePath + "?" + queryParams.join("&")
    : basePath;
};

export default function WorkEntries() {
  const classes = useStyles();
  const filterContext = useContext(FilterContext);
  const [entries, setEntries] = useState([]);

  useEffect(() => {
    axios
      .get(buildRequestUrl(filterContext))
      .then(resp => setEntries(resp.data))
      .catch(err => console.error(err));
  }, []);

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
        <Link color="primary" href="#" onClick={preventDefault}>
          See more orders
        </Link>
      </div>
    </React.Fragment>
  );
}
