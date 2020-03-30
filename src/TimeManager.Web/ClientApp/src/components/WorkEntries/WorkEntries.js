import React, { useContext, useEffect, useState, useCallback } from "react";
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
import * as WorkEntriesStore from "../../stores/WorkEntriesStore";
import { fetchUserProfile } from "../../stores/AccountStore";

export default props => {
  const filterContext = useContext(FilterContext);
  const [entries, setEntries] = useState([]);
  const [isEntryDialogOpen, setEntryDialogOpen] = useState(false);
  const [selectedEntry, setSelectedEntry] = useState(null);
  const { userId } = props;
  const [userProfile, setUserProfile] = useState({});

  const handleEntryDialogOpen = () => {
    setEntryDialogOpen(true);
  };

  const handleEntryDialogClose = () => {
    setEntryDialogOpen(false);
  };

  const refresh = useCallback(() => {
    WorkEntriesStore.getList(userId, filterContext)
      .then(resp => {
        setEntries(resp.data);
      })
      .catch(err => console.error(err));
  }, [userId, filterContext]);

  useEffect(() => {
    fetchUserProfile(userId)
      .then(resp => {
        setUserProfile(resp.data);
        return refresh();
      })
      .catch(err => console.error(err));
  }, [filterContext, userId, setUserProfile, refresh]);

  const handleSaveEntry = formState => {
    const entryState = {
      ...formState,
      userId: props.userId
    };
    WorkEntriesStore.saveEntry(entryState)
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
              preferredMinHours={userProfile.preferredHoursPerDay}
              readOnly={props.readOnly}
            />
          ))}
        </TableBody>
      </Table>
      <ActionsPanel
        onAddEntryClicked={handleEntryDialogOpen}
        onRefreshClicked={refresh}
        exportUrl={WorkEntriesStore.getExportWorkEntriesUrl(
          userId,
          filterContext
        )}
        readOnly={props.readOnly}
      />

      {!props.readOnly && (
        <AddEntryDialog
          isOpen={isEntryDialogOpen}
          onClose={handleEntryDialogClose}
          onEntrySaved={handleSaveEntry}
          entryId={selectedEntry}
        />
      )}
    </React.Fragment>
  );
};
