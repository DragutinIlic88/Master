import React from "react";

import classes from "./CommonButton.module.css";

/**
 * Generic component represents button used in this application.
 * It has name label, click event and disable option passed via props object.
 * @param {object} props
 */
const CommonButton = (props) => {
  return (
    <input
      type="button"
      value={props.buttonLabel}
      onClick={props.buttonClicked}
      style={props.style}
      className={classes.CommonButton}
      disabled={props.disabled}
    />
  );
};

export default CommonButton;
