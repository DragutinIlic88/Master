import React from "react";
import classes from "./Backdrop.module.css";

/**
 * Component for bluring background while modal is displayed. It has click event passed
 * via props object. It will be shown in case that show property is true.
 * @param {object} props
 */
const Backdrop = (props) =>
  props.show ? (
    <div className={classes.Backdrop} onClick={props.clicked}></div>
  ) : null;

export default Backdrop;
