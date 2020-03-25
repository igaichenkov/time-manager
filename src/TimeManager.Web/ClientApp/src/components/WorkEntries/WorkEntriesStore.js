import axios from "../../utils/axios";
import dateformat from "dateformat";

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

export const saveEntry = formState => {
  const payload = {
    date: dateformat(formState.date, "yyyy-mm-dd"),
    hoursSpent: parseFloat(formState.hoursSpent),
    notes: formState.notes
  };

  return formState.id
    ? axios.put(`/api/WorkEntries/${formState.id}`, payload)
    : axios.post("/api/WorkEntries", payload);
};

export const getList = filter => axios.get(buildRequestUrl(filter));
export const deleteEntry = id => axios.delete(`/api/WorkEntries/${id}`);
export const getWorkEntryById = id => axios.get(`/api/WorkEntries/${id}`);
