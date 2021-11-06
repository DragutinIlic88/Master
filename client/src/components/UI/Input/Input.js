import React from "react";
import classes from "./Input.module.css";

/**
 * Genric component displays custom input button with passed type,
 * name, id , required, pattern, value and onChange event handler
 * via props object.
 * @param {object} props
 */
const input = (props) => {
  //TODO: replace this with method from util module
  const onHandleInputChanged = (event) => {
    if (props.pattern) {
      var regex = new RegExp(props.pattern);
      if (event.target.value === "" || regex.test(event.target.value)) {
        props.onValueChange(event.target.value);
      }
    } else {
      props.onValueChange(event.target.value);
    }
  };

  return (
    <div>
      <input
        type={props.inputType}
        name={props.inputName}
        className={classes.message}
        id={props.inputId}
        required={props.isRequired}
        autoComplete="off"
        placeholder=" "
        pattern={props.inputPattern ? props.inputPattern : undefined}
        value={props.inputValue}
        onChange={onHandleInputChanged}
      />
      <label htmlFor={props.inputId}>
        <span>{props.spanText}</span>
      </label>
    </div>
  );
};

export default input;
