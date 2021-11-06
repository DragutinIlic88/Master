import React from "react";
import { format } from "date-fns";
import { ACCOUNT_TYPE } from "../../../../constants";
import classes from "./AccountDetails.module.css";

/**
 * Component displayes detailed account information to the user.
 * Account number, type, creation date, details and balance are passed by props.
 * @param {object} props
 */
const AccountDetails = (props) => {
  return (
    <div className={classes.AccountContent}>
      <div className={classes.FirstColumn}>
        <div className={classes.AccountNumber}>
          <div className={classes.AccountLabel}>{props.accountNumberLabel}</div>
          <div className={classes.AccountNumberValue}>
            {props.accountNumberValue}
          </div>
        </div>
        <div className={classes.AccountType}>
          <div className={classes.AccountLabel}>{props.accountTypeLabel}</div>
          <div className={classes.AccountTypeValue}>
            {ACCOUNT_TYPE[props.accountTypeValue]}
          </div>
        </div>
        <div className={classes.AccountDate}>
          <div className={classes.AccountLabel}>{props.accountDateLabel}</div>
          <div className={classes.AccountDateValue}>
            {format(new Date(props.accountDateValue), "dd.MM.yyyy hh:mm:ss")}
          </div>
        </div>
      </div>
      <div className={classes.SecondColumn}>
        <div className={classes.AccountDetails}>
          <div className={classes.AccountLabel}>
            {props.accountDetailsLabel}
          </div>
          <div className={classes.AccountDetailsValue}>
            {props.accountDetailsValue}
          </div>
        </div>
        <div className={classes.AccountState}>
          <div className={classes.AccountLabel}>{props.accountStateLabel}</div>
          <div className={classes.AccountStateValue}>
            <span>{props.accountStateAmount}</span>
            <span>{props.accountStateCurrency}</span>
          </div>
        </div>
        <div className={classes.PositionButton}>
          <div
            className={classes.HideAccountContent}
            onClick={props.hideContent}
          >
            {props.buttonLabel}
          </div>
        </div>
      </div>
    </div>
  );
};

export default AccountDetails;
