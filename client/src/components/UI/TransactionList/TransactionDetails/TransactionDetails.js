import React from "react";
import { format } from "date-fns";
import { TRANSACTION_STATUS } from "../../../../constants";
import classes from "./TransactionDetails.module.css";

/**
 * Component displays name of transaction,
 * account number on which transaction is preformed,
 * type of transaction, date of transaction,
 * details and status of transaction and amount used in
 * transaction. It has button for closing component.
 * All details are passed by props object.
 * @param {object} props
 */
const TransactionDetails = (props) => {
  return (
    <div className={classes.TransactionContent}>
      <div className={classes.FirstColumn}>
        <div className={classes.TransactionName}>
          <div className={classes.TransactionLabel}>
            {props.transactionNameLabel}
          </div>
          <div className={classes.TransactionNameValue}>
            {props.transactionNameValue}
          </div>
        </div>
        <div className={classes.AccountNumber}>
          <div className={classes.TransactionLabel}>
            {props.accountNumberLabel}
          </div>
          <div className={classes.AccountNumberValue}>
            {props.accountNumberValue}
          </div>
        </div>
        <div className={classes.TransactionType}>
          <div className={classes.TransactionLabel}>
            {props.transactionTypeLabel}
          </div>
          <div className={classes.TransactionTypeValue}>
            {props.transactionTypeValue}
          </div>
        </div>
        <div className={classes.TransactionDate}>
          <div className={classes.TransactionLabel}>
            {props.transactionDateLabel}
          </div>
          <div className={classes.TransactionDateValue}>
            {format(
              new Date(props.transactionDateValue),
              "dd.MM.yyyy hh:mm:ss"
            )}
          </div>
        </div>
      </div>
      <div className={classes.SecondColumn}>
        <div className={classes.TransactionDetails}>
          <div className={classes.TransactionLabel}>
            {props.transactionDetailsLabel}
          </div>
          <div className={classes.TransactionDetailsValue}>
            {props.transactionDetailsValue}
          </div>
        </div>
        <div className={classes.TransactionStatus}>
          <div className={classes.TransactionLabel}>
            {props.transactionStatusLabel}
          </div>
          <div className={classes.TransactionStatusValue}>
            {TRANSACTION_STATUS[props.transactionStatusValue]}
          </div>
        </div>
        <div className={classes.TransactionAmount}>
          <div className={classes.TransactionLabel}>
            {props.transactionAmountLabel}
          </div>
          <div className={classes.TransactionAmountValue}>
            <span>{props.transactionAmount}</span>
            <span>{props.transactionCurrency?.toUpperCase()}</span>
          </div>
        </div>
        <div className={classes.PositionButton}>
          <div
            className={classes.HideTransactionContent}
            onClick={props.hideContent}
          >
            {props.buttonLabel}
          </div>
        </div>
      </div>
    </div>
  );
};

export default TransactionDetails;
