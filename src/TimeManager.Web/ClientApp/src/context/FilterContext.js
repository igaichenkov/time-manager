import React, { useState } from "react";

export const FilterContext = React.createContext({
  minDate: null,
  maxDate: null,
  setMinDate: () => {},
  setMaxDate: () => {}
});

const FilterContextProvider = props => {
  const [filter, setFilterValue] = useState({
    minDate: null,
    maxDate: null
  });

  const setMinDate = date => {
    setFilterValue(prevFilter => ({
      ...prevFilter,
      minDate: date
    }));
  };

  const setMaxDate = date => {
    setFilterValue(prevFilter => ({
      ...prevFilter,
      maxDate: date
    }));
  };

  return (
    <FilterContext.Provider
      value={{
        minDate: filter.minDate,
        maxDate: filter.maxDate,
        setMinDate: setMinDate,
        setMaxDate: setMaxDate
      }}
    >
      {props.children}
    </FilterContext.Provider>
  );
};

export default FilterContextProvider;
