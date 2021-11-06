import React from "react";
import classes from "./SubmitButton.module.css";

/**
 * Generic component represents submit button used inside application.
 * It can be disabled and has click event and text passed via props.
 * @param {object} props
 */
const submitButton = (props) => {
  let submit = (
    <div className={classes.DisabledSubmitButton} onClick={props.clicked}>
      {props.value}
    </div>
  );

  if (props.enabledSubmit) {
    submit = (
      <div className={classes.SubmitButton} onClick={props.clicked}>
        {props.value}
      </div>
    );
  }

  return submit;
};

export default submitButton;
