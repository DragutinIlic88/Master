import React from "react";

import classes from "./SignUpSwitcher.module.css";
/**
 * Generic component which represents button , and has options
 * for setting label and text of button and also for setting what
 * will be clicked event. It is used as switcher between login and
 * sign up page.
 * @param {object} props
 */
const SignUpSwitcher = (props) => {
  return (
    <div className={classes.SignUpSwitcher}>
      <span className={classes.SignUpSwitcherLabel}>{props.labelText}</span>
      <span className={classes.SignUpSwitcherLink} onClick={props.clicked}>
        {props.linkText}
      </span>
    </div>
  );
};

export default SignUpSwitcher;
