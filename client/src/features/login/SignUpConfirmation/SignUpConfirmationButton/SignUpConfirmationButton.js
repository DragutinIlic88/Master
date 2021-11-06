import React from "react";
import classes from "./SignUpConfrimationButton.module.css";

/**
 * Generic Component for displaying button with custom name and click event
 * @param {object} props
 */
const SignUpConfirmationButton = (props) => {
  return (
    <div onClick={props.clicked} className={classes.ButtonStyle}>
      {props.text}
    </div>
  );
};

export default SignUpConfirmationButton;
