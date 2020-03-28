import axios from "../utils/axios";
import dateformat from "dateformat";

const buildRequestUrl = (userId, filter) => {
  const basePath = "/api/WorkEntries" + (userId ? "/users/" + userId : "");

  const query = buildQueryParams(filter);

  return query.length > 0 ? basePath + "?" + query : basePath;
};

const buildQueryParams = filter => {
  const queryParams = [];
  if (filter.minDate) {
    queryParams.push("minDate=" + dateformat(filter.minDate, "yyyy-mm-dd"));
  }

  if (filter.maxDate) {
    queryParams.push("maxDate=" + dateformat(filter.maxDate, "yyyy-mm-dd"));
  }

  return queryParams.join("&");
};

export const saveEntry = formState => {
  const payload = {
    date: dateformat(formState.date, "yyyy-mm-dd"),
    hoursSpent: parseFloat(formState.hoursSpent),
    notes: formState.notes,
    userId: formState.userId
  };

  return formState.id
    ? axios.put(`/api/WorkEntries/${formState.id}`, payload)
    : axios.post("/api/WorkEntries", payload);
};

export const getList = (userId, filter) =>
  axios.get(buildRequestUrl(userId, filter));

export const deleteEntry = id => axios.delete(`/api/WorkEntries/${id}`);
export const getWorkEntryById = id => axios.get(`/api/WorkEntries/${id}`);

export const getExportWorkEntriesUrl = (userId, filter) => {
  const basePath = `/api/export/users/${userId || "me"}/work-entries`;

  const query = buildQueryParams(filter);

  return query.length > 0 ? basePath + "?" + query : basePath;
};
