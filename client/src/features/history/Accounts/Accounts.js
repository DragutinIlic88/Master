import React from "react";

import AccountItem from "./AccountItem/AccountItem";
import Loader from "../../../components/UI/Loader/Loader";
import { FETCH_DATA_STATUS } from "../../../constants";

import classes from "./Accounts.module.css";

/**
 * Component creates and displayes list of AccountItem components based on
 * accounts which are passed with props object.
 * If accounts are still loading, Loader component is displayed.
 * If some error occures while loading accounts, it will be displayed as part
 * of this component.
 * Otherwise list of accountItem are created and shown to the user.
 * @param {object} props
 */
const Accounts = (props) => {
  let accountsContent = (
    <div className={classes.AccountsContent}>
      <Loader />
    </div>
  );

  if (props.status === FETCH_DATA_STATUS.LOADING) {
    accountsContent = (
      <div className={classes.AccountsContent}>
        <Loader />
      </div>
    );
  } else if (props.status === FETCH_DATA_STATUS.SUCEEDED) {
    const accounts = props.accounts.map((acc) => {
      return (
        <AccountItem
          key={acc.iban}
          selected={acc.accountNumber === props.selectedAccount.accountNumber}
          accountNumber={acc.accountNumber}
          accountAmount={acc.amount}
          accountCurrency={acc.currency}
          detailsButtonLabel={props.detailsButtonLabel}
          detailsClicked={(e) => {
            props.showTransactionDetails(e, acc);
          }}
          accountClicked={() => {
            props.selectClickedAccount(acc);
          }}
        />
      );
    });
    accountsContent = <ul className={classes.AccountsContent}>{accounts}</ul>;
  } else if (props.status === FETCH_DATA_STATUS.FAILED) {
    accountsContent = (
      <div className={classes.AccountsError}>{props.error}</div>
    );
  }

  return (
    <div className={classes.Accounts}>
      <div className={classes.AccountsHeader}> {props.accountsHeaderTitle}</div>
      {accountsContent}
    </div>
  );
};

export default Accounts;
