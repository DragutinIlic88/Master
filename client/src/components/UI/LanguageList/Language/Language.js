import React from "react";
import classes from "./Language.module.css";
/**
 * Component displays language passed to it via props object.
 * It is clickable.
 * @param {object} props
 */
const language = (props) => {
  return (
    <span
      onClick={() => {
        props.change();
      }}
      className={classes.Languge}
    >
      {props.language}
    </span>
  );
};

export default language;
