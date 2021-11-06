import React from "react";

import classes from "./NavigationBarListItem.module.css";

/**
 * Component renderes name which is passed to it via props,
 * and calls passed handler when it is clicked
 * @component
 * @param {object} props
 */
const NavigationBarListItem = (props) => {
  const styleClasses = props.isActive
    ? [classes.NavigationBarListItem, classes.Active].join(" ")
    : classes.NavigationBarListItem;
  return (
    <li className={styleClasses} onClick={props.clicked}>
      {props.listItemName}
    </li>
  );
};

export default NavigationBarListItem;
