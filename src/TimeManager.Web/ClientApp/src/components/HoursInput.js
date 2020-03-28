import React, { useState } from "react";
import TextField from "@material-ui/core/TextField";

export default props => {
  const [error, setError] = useState({
    isInvalid: false,
    text: ""
  });
  const {
    value,
    onChange,
    errorStateChanged,
    allowMin,
    min,
    max,
    ...rest
  } = props;

  const resetError = () => setError({ isInvalid: false, text: "" });

  const handleChange = e => {
    const floatValue = parseFloat(e.target.value);

    if (
      (!allowMin && floatValue === min) ||
      floatValue < min ||
      floatValue > max
    ) {
      setError({ isInvalid: true, text: "Value is out of range" });
      errorStateChanged({ isValid: false });
    } else {
      resetError();
      errorStateChanged({ isValid: true });
    }

    onChange(e);
  };

  return (
    <TextField
      type="number"
      value={value}
      onChange={handleChange}
      error={error.isInvalid}
      helperText={error.text}
      {...rest}
    />
  );
};
