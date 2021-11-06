import React from "react";

import classes from "./AccountItem.module.css";

/**
 * Component displays information of account passed by props.
 * It also has button for displaying detaild information of passed account.
 * @param {object} props
 */
const AccountItem = (props) => {
  let styleClasses = props.selected
    ? [classes.AccountItem, classes.SelectedAccount]
    : [classes.AccountItem];
  return (
    <div className={styleClasses.join(" ")} onClick={props.accountClicked}>
      <div className={classes.ContentDiv}>{props.accountNumber}</div>
      <div className={classes.ContentDiv}>
        <span>{props.accountAmount}</span>
        <span>{props.accountCurrency.toUpperCase()}</span>
      </div>
      <div className={classes.ContentDiv}>
        <input
          type="button"
          value={props.detailsButtonLabel}
          onClick={props.detailsClicked}
        />
      </div>
    </div>
  );
};

export default AccountItem;
