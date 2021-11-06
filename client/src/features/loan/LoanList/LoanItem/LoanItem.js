import React from "react";

import classes from "./LoanItem.module.css";

/**
 * Component for displaying information about one loan in list.
 * It handles logic in case that loan is clicked.
 * @param {object} props
 */
const LoanItem = (props) => {
  return (
    <li className={classes.LoanItem} onClick={props.clicked}>
      <div className={classes.FirstRow}>
        <div className={classes.ItemName}>{props.name}</div>
        <div className={classes.ItemAmount}>
          <span>{props.amount}</span>
          <span>{props.currency}</span>
        </div>
        <div className={classes.ItemStatus}>{props.status}</div>
      </div>
      <div className={classes.ItemDate}>{props.date}</div>
    </li>
  );
};

export default LoanItem;
