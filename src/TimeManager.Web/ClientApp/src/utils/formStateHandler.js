export default (e, setState) => {
  const newValue = e.target.value;
  const fieldName = e.target.name;

  setState(prevState => {
    const newState = { ...prevState };
    newState[fieldName] = newValue;

    return newState;
  });
};
