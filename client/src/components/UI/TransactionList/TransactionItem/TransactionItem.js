import React from "react";

import classes from "./TransactionItem.module.css";

/**
 * Compoenents displayes information of transaction passed as props.
 * It displayes amount , currency ,date and name of transaction which is rendered.
 * If clicked parameter is set than it also fires clicked event passed via props object
 * when user clicks on it.
 * @param {object} props
 */
const TransactionItem = (props) => {
  if (props.clicked) {
    return (
      <li
        className={classes.TransactionItem + " " + classes.Clickable}
        onClick={props.clicked}
      >
        <div className={classes.FirstRow}>
          <div className={classes.TransactionItemName}>{props.name}</div>
          <div className={classes.TransactionItemAmount}>
            <span>{props.amount}</span>
            <span>{props.currency?.toUpperCase()}</span>
          </div>
        </div>
        <div className={classes.TransactionItemDate}>{props.date}</div>
      </li>
    );
  }

  return (
    <li className={classes.TransactionItem}>
      <div className={classes.FirstRow}>
        <div className={classes.TransactionItemName}>{props.name}</div>
        <div className={classes.TransactionItemAmount}>
          <span>{props.amount}</span>
          <span>{props.currency?.toUpperCase()}</span>
        </div>
      </div>
      <div className={classes.TransactionItemDate}>{props.date}</div>
    </li>
  );
};

export default TransactionItem;
