import React from "react";

import classes from "./SelectButton.module.css";

/**
 * Component shows custom generic select button used inside application.
 * It can be disabled and it has onChange event passed to it via props object.
 * Component generate option elements based on items array passed to it via props.
 * Items array must have objects which contains value property and text property.
 * @param {object} props
 */
const SelectButton = (props) => {
  const options = props.items.map((item) => {
    return (
      <option key={item.value} value={item.value}>
        {item.text}
      </option>
    );
  });

  return (
    <div
      className={classes.SelectButton}
      style={props.style}
      disabled={props.disabled}
    >
      <label>
        {props.labelText}
        <select value={props.selectValue} onChange={props.onSelectChange}>
          {options}
        </select>
      </label>
    </div>
  );
};

export default SelectButton;
