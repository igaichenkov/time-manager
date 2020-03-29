import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import Copyright from "./Copyright";

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

describe("<Copyright />", () => {
  it("Copyright component prints correct name", () => {
    act(() => {
      render(<Copyright />, container);
    });

    expect(container.textContent).toContain("Igor Gaichenkov");
  });
});
