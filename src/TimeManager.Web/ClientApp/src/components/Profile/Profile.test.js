jest.mock("../../stores/AccountStore");

import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import Profile from "./Profile";
import { fetchUserProfile } from "../../stores/AccountStore";

const TestUserName = "some user name";

const mockProfileRequest = firstName => {
  const userProfile = {
    id: "123",
    firstName: firstName,
    lastName: "",
    preferredHoursPerday: 0,
    userName: TestUserName
  };

  fetchUserProfile.mockResolvedValue({ data: userProfile });
};

const triggerInputEventChanged = (inputElement, newValue) => {
  var nativeInputValueSetter = Object.getOwnPropertyDescriptor(
    window.HTMLInputElement.prototype,
    "value"
  ).set;
  nativeInputValueSetter.call(inputElement, newValue);

  var ev = new Event("input", { bubbles: true });
  inputElement.dispatchEvent(ev);
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

it("Profile renders userName if userId is set", async () => {
  mockProfileRequest("");

  await act(async () => {
    render(<Profile userId="123" onProfileSaved={jest.fn()} />, container);
  });

  expect(container.textContent).toContain(TestUserName);
});

it("Profile renders firstName if userId isn't set", async () => {
  const firstName = "some first name";
  mockProfileRequest(firstName);

  await act(async () => {
    render(<Profile onProfileSaved={jest.fn()} />, container);
  });

  expect(container.textContent).toContain(firstName);
  expect(container.textContent).not.toContain(TestUserName);
});

it("Profile renders userName if userId isn't set and firstName is empty", async () => {
  const firstName = "";
  mockProfileRequest(firstName);

  await act(async () => {
    render(<Profile onProfileSaved={jest.fn()} />, container);
  });

  expect(container.textContent).toContain(TestUserName);
});

it("Save button click raises an event, if form state is valid", async () => {
  const firstName = "";
  mockProfileRequest(firstName);

  const profileSavedHandler = jest.fn();
  await act(async () => {
    render(<Profile onProfileSaved={profileSavedHandler} />, container);
  });

  const button = document.getElementById("saveProfileBtn");

  act(() => {
    button.dispatchEvent(new MouseEvent("click", { bubbles: true }));
  });

  expect(profileSavedHandler).toHaveBeenCalledTimes(1);
});

it("Save button click does nothing, if form is invalid", async () => {
  const firstName = "";
  mockProfileRequest(firstName);

  const profileSavedHandler = jest.fn();
  await act(async () => {
    render(<Profile onProfileSaved={profileSavedHandler} />, container);
  });

  const button = document.getElementById("saveProfileBtn");
  const hoursSpentField = document.getElementById("preferredHoursPerDay");

  act(() => {
    triggerInputEventChanged(hoursSpentField, "56");
  });

  act(() => {
    button.dispatchEvent(new MouseEvent("click", { bubbles: true }));
  });

  expect(profileSavedHandler).toHaveBeenCalledTimes(0);
});

it("Change form state to invalid and back to valid. Form gets submitted", async () => {
  const firstName = "";
  mockProfileRequest(firstName);

  const profileSavedHandler = jest.fn();
  await act(async () => {
    render(<Profile onProfileSaved={profileSavedHandler} />, container);
  });

  const button = document.getElementById("saveProfileBtn");
  const hoursSpentField = document.getElementById("preferredHoursPerDay");

  act(() => {
    triggerInputEventChanged(hoursSpentField, "56");
    triggerInputEventChanged(hoursSpentField, "2");
  });

  act(() => {
    button.dispatchEvent(new MouseEvent("click", { bubbles: true }));
  });

  expect(profileSavedHandler).toHaveBeenCalledTimes(1);
});
