import React from "react";
import Loader from "../../../components/UI/Loader/Loader";
import { FETCH_DATA_STATUS } from "../../../constants";

import classes from "./LoanConfirmation.module.css";

/**
 * Component for showing summary information of new loan.
 * User can choose to cancel these information or to submit it.
 * All informations are passed from parent component via props object
 * @component
 * @param {object} props
 */
const LoanConfirmation = (props) => {
  const {
    fromAccount,
    receiveAccount,
    totalAmount,
    participation,
    currency,
    startDate,
    endDate,
    purpose,
    collateral,
  } = props.data;
  const {
    fromAccountLabel,
    receiveAccountLabel,
    totalAmountLabel,
    participationLabel,
    currencyLabel,
    startDateLabel,
    endDateLabel,
    purposeLabel,
    collateralLabel,
    submitLabel,
    cancelLabel,
  } = props.labels;

  /**
   * While information are proccessed Loader component will be shown
   */
  if (props.status === FETCH_DATA_STATUS.LOADING) {
    return <Loader />;
  }

  /**
   * If there is some error with submiting loan , that error will be shown to the user
   */
  if (props.error) {
    return (
      <div className={classes.LoanConfirmation}>
        <div className={classes.Summary}>
          <div className={classes.Failed}>{props.error}</div>
        </div>
        <div className={classes.Confirm}>
          <div
            className={classes.Cancel}
            onClick={props.handleOnConfirmationPageClosed}
          >
            {cancelLabel}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className={classes.LoanConfirmation}>
      <div className={classes.Summary}>
        <div className={classes.ContentWrapper}>
          <div className={classes.Label}>{fromAccountLabel}</div>
          <div className={classes.Value}>{fromAccount}</div>
        </div>
        <div className={classes.ContentWrapper}>
          <div className={classes.Label}>{receiveAccountLabel}</div>
          <div className={classes.Value}>{receiveAccount}</div>
        </div>
        <div className={classes.ContentWrapper}>
          <div className={classes.Label}>{totalAmountLabel}</div>
          <div className={classes.Value}>{totalAmount}</div>
        </div>
        <div className={classes.ContentWrapper}>
          <div className={classes.Label}>{participationLabel}</div>
          <div className={classes.Value}>{participation}</div>
        </div>
        <div className={classes.ContentWrapper}>
          <div className={classes.Label}>{currencyLabel}</div>
          <div className={classes.Value}>{currency}</div>
        </div>
        <div className={classes.ContentWrapper}>
          <div className={classes.Label}>{startDateLabel}</div>
          <div className={classes.Value}>{startDate}</div>
        </div>
        <div className={classes.ContentWrapper}>
          <div className={classes.Label}>{endDateLabel}</div>
          <div className={classes.Value}>{endDate}</div>
        </div>
        {purpose ? (
          <div className={classes.ContentRowWrapper}>
            <div className={classes.Label}>{purposeLabel}</div>
            <div className={classes.Value}>{purpose}</div>
          </div>
        ) : null}

        {collateral ? (
          <div className={classes.ContentRowWrapper}>
            <div className={classes.Label}>{collateralLabel}</div>
            <div className={classes.Value}>{collateral}</div>
          </div>
        ) : null}
      </div>
      <div className={classes.Confirm}>
        <div
          className={classes.Submit}
          onClick={props.handleOnConfirmationPageSubmit}
        >
          {submitLabel}
        </div>
        <div
          className={classes.Cancel}
          onClick={props.handleOnConfirmationPageClosed}
        >
          {cancelLabel}
        </div>
      </div>
    </div>
  );
};

export default LoanConfirmation;
