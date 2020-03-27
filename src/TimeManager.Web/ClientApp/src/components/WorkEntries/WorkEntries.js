import React, { useContext, useEffect, useState } from "react";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Title from "../Title";
import { FilterContext } from "../../context/FilterContext";
import AddEntryDialog from "./AddEntryDialog";
import EntryRow from "./EntryRow";
import ActionsPanel from "./ActionsPanel";
import { AuthContext } from "../../context/AuthContext";
import * as WorkEntriesStore from "./WorkEntriesStore";

export default props => {
  const filterContext = useContext(FilterContext);
  const authContext = useContext(AuthContext);
  const [entries, setEntries] = useState([]);
  const [isEntryDialogOpen, setEntryDialogOpen] = useState(false);
  const [selectedEntry, setSelectedEntry] = useState(null);

  const handleEntryDialogOpen = () => {
    setEntryDialogOpen(true);
  };

  const handleEntryDialogClose = () => {
    setSelectedEntry(null);
    setEntryDialogOpen(false);
  };

  const refresh = () => {
    WorkEntriesStore.getList(props.userId, filterContext)
      .then(resp => setEntries(resp.data))
      .catch(err => console.error(err));
  };

  useEffect(() => refresh(), [filterContext]);

  const handleSaveEntry = formState => {
    WorkEntriesStore.saveEntry(formState)
      .then(() => handleEntryDialogClose())
      .then(() => refresh())
      .catch(err => console.error(err));
  };

  const handleEditRow = id => {
    setSelectedEntry(id);
    setEntryDialogOpen(true);
  };

  const handleDeleteRow = id => {
    WorkEntriesStore.deleteEntry(id)
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
            <EntryRow
              key={row.id}
              workEntry={row}
              onEdit={handleEditRow}
              onDelete={handleDeleteRow}
              preferredMinHours={
                authContext.account.profile.preferredHoursPerDay
              }
            />
          ))}
        </TableBody>
      </Table>
      <ActionsPanel
        onAddEntryClicked={handleEntryDialogOpen}
        onRefreshClicked={refresh}
      />

      <AddEntryDialog
        isOpen={isEntryDialogOpen}
        onClose={handleEntryDialogClose}
        onEntrySaved={handleSaveEntry}
        entryId={selectedEntry}
      />
    </React.Fragment>
  );
};
