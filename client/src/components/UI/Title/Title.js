import React from "react";
import classes from "./Title.module.css";

/**
 * Component displayes logo and name of application inside header.
 * @param {object} props
 */
const title = (props) => {
  return (
    <div className={classes.Header}>
      <div className={classes.Logo}></div>
      <div className={classes.Title}>{props.titleName}</div>
    </div>
  );
};

export default title;
