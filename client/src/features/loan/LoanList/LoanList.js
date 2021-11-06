import React from "react";

import LoanItem from "./LoanItem/LoanItem";
import Loader from "../../../components/UI/Loader/Loader";
import { FETCH_DATA_STATUS } from "../../../constants";
import classes from "./LoanList.module.css";

/**
 * Component for displaying list of all loans which user had/have.
 * While loans are loaded from server Loader component will be displayed.
 * In case that there isn't any loans content with proper message will be displayed.
 * In case of error while retrieving loans from server , proper message is displayed.
 * Otherwise list of LoanItem componets is made and presented to the user.
 * @param {object} props
 */
const LoanList = (props) => {
  let loansHistoryContent = null;

  if (props.loadingStatus === FETCH_DATA_STATUS.LOADING) {
    loansHistoryContent = (
      <div>
        <Loader />
      </div>
    );
  } else if (props.loadingStatus === FETCH_DATA_STATUS.SUCEEDED) {
    if (!props.loansList) {
      loansHistoryContent = (
        <div className={classes.LoanListNoLoans}>
          {props.noPreviousLoansMessage}
        </div>
      );
    } else {
      const loans = props.loansList.map((loan) => {
        return (
          <LoanItem
            key={loan.loanId}
            name={loan.loanName}
            amount={loan.totalAmount}
            currency={loan.currency}
            status={loan.loanStatus}
            date={loan.creationDate}
            clicked={() => {
              props.showLoanDetails(loan);
            }}
          />
        );
      });
      loansHistoryContent = (
        <ul className={classes.LoanListContent}>{loans}</ul>
      );
    }
  } else if (props.error) {
    loansHistoryContent = (
      <div className={classes.ErrorMessage}>{props.error}</div>
    );
  }
  return (
    <div className={classes.LoanList}>
      <div className={classes.LoanListHeader}>{props.headerTitle}</div>
      {loansHistoryContent}
    </div>
  );
};

export default LoanList;
