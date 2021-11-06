import React from "react";
import TransactionItem from "./TransactionItem/TransactionItem";
import Loader from "../Loader/Loader";
import { FETCH_DATA_STATUS } from "../../../constants";

import classes from "./TransactionList.module.css";

/**
 * Component for creating and displaying list of transactionItems components,
 * based on transactions paramter passed by props object.
 * Compoenent checks status parameter of props object and if it has LOADING value
 * displays Loader component, if it has FAILED value displayes error messge passed to
 * it as props and if it has SUCEEDED value makes list of TransactionItem objects and
 * displayes it to the user.
 * @param {object} props
 */
const TransactionList = (props) => {
  let transactionListContent = (
    <div>
      <Loader />
    </div>
  );

  if (props.status === FETCH_DATA_STATUS.LOADING) {
    transactionListContent = (
      <div>
        <Loader />
      </div>
    );
  } else if (props.status === FETCH_DATA_STATUS.SUCEEDED) {
    if (props.transactions.length === 0) {
      transactionListContent = (
        <div className={classes.Message}>{props.noTransactionsMessage}</div>
      );
    } else {
      const transactions = props.transactions.map((tr) => {
        if (props.showTransactionDetails) {
          return (
            <TransactionItem
              key={tr.transactionId}
              name={tr.transactionName}
              amount={tr.transactionAmount.total}
              currency={tr.transactionAmount.currency}
              date={tr.creationTime}
              clicked={() => {
                props.showTransactionDetails(tr);
              }}
            />
          );
        }
        return (
          <TransactionItem
            key={tr.transactionId}
            name={tr.transactionName}
            amount={tr.transactionAmount.total}
            currency={tr.transactionAmount.currency}
            date={tr.creationTime}
          />
        );
      });
      transactionListContent = (
        <ul className={classes.TransactionList}>{transactions}</ul>
      );
    }
  } else if (props.status === FETCH_DATA_STATUS.FAILED) {
    transactionListContent = (
      <div className={classes.ErrorMessage}>{props.error}</div>
    );
  }

  return transactionListContent;
};

export default TransactionList;
