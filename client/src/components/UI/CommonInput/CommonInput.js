import React from "react";

import classes from "./CommonInput.module.css";

/**
 * Generic component used inside login/signup page.
 * It can be disabled, it has onChange event ,type, text
 * and additional options passed to it via props.
 * OnChange is wrapped inside onHandleInputChanged method
 * which checks if pattern is passed to the component and if
 * that is the case performs checking if value is valid.
 * @param {object} props
 */
const CommonInput = (props) => {
  //TODO: replace this with method from util module
  const onHandleInputChanged = (event) => {
    if (props.pattern) {
      var regex = new RegExp(props.pattern);
      if (event.target.value === "" || regex.test(event.target.value)) {
        props.onInputChange(event);
      }
    } else {
      props.onInputChange(event);
    }
  };

  return (
    <div
      className={classes.CommonInput}
      style={props.style}
      disabled={props.disabled}
    >
      <span className={classes.LabelText}>{props.labelText}</span>
      <input
        type={props.inputType}
        value={props.inputValue}
        onChange={onHandleInputChanged}
        {...props.additionalProps}
        style={props.inputStyle}
      />
      {props.error ? (
        <span className={classes.Error}>{props.error}</span>
      ) : null}
    </div>
  );
};

export default CommonInput;
