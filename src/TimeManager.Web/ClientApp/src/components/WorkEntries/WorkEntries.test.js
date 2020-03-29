import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import WorkEntries from "./WorkEntries";
import dateformat from "dateformat";
import FilterContextProvider from "../../context/FilterContext";

jest.mock("../../stores/AccountStore");
const { fetchUserProfile } = require("../../stores/AccountStore");

jest.mock("../../stores/WorkEntriesStore");
const { getList } = require("../../stores/WorkEntriesStore");

const mockProfileRequest = () => {
  const userProfile = {
    id: "123",
    firstName: "First",
    lastName: "Last",
    preferredHoursPerDay: 4,
    userName: "TestUserName"
  };

  fetchUserProfile.mockResolvedValue({ data: userProfile });
};

const workEntries = [
  {
    id: "1",
    date: "2020-03-01",
    hoursSpent: 1,
    userId: "123",
    notes: ""
  },
  {
    id: "2",
    date: "2020-03-20",
    hoursSpent: 5,
    userId: "123",
    notes: ""
  }
];

const mockGetWorkEntriesList = () => {
  getList.mockResolvedValue({ data: workEntries });
};

let container = null;
beforeEach(() => {
  container = document.createElement("div");
  document.body.appendChild(container);
});

afterEach(() => {
  unmountComponentAtNode(container);
  container.remove();
  container = null;
});

describe("<WorkEntries />", () => {
  it("Rows get highlighted if preferred hours per day setting is set", async () => {
    mockProfileRequest();
    mockGetWorkEntriesList();

    await act(async () => {
      render(
        <FilterContextProvider>
          <WorkEntries userId="123" readOnly={false} />
        </FilterContextProvider>,
        container
      );
    });

    expect(
      container.querySelector("tr.entryUnderTimeLimit td").textContent
    ).toContain(dateformat(workEntries[0].date, "yyyy.mm.dd"));
    expect(
      container.querySelector("tr.entryTimeLimitOk td").textContent
    ).toContain(dateformat(workEntries[1].date, "yyyy.mm.dd"));
  });
});
