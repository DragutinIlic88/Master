import React from "react";

import classes from "./PaymentSummary.module.css";
import { FETCH_DATA_STATUS } from "../../../constants";
import Loader from "../../../components/UI/Loader/Loader";

/**
 * Component renders summary infromations for payment which user inserted.
 * Information shown are passed via props object and contains beneficiary customer,
 * beneficiary customer's account, amount , model , reference number, payment code and
 * purpose of payment. Component gives options for canceling or submiting payment.
 * If payment is submited proper message is shown to the user in case of success or in
 * case of failure.
 * @param {object} props
 */
const PaymentSummary = (props) => {
  let content = (
    <div className={classes.PaymentSummary}>
      <div className={classes.PaymentSummeryHeader}>{props.headerLabel}</div>
      <div className={classes.PaymentSummaryDetails}>
        <div className={classes.Row}>
          <div className={classes.Label}>{props.beneficiaryCustomerLabel}</div>
          <div className={classes.Value}>{props.beneficiaryCustomerValue}</div>
        </div>
        <div className={classes.Row}>
          <div className={classes.Label}>
            {props.beneficiaryCustomerAccountLabel}
          </div>
          <div className={classes.Value}>
            {props.beneficiaryCustomerAccountValue}
          </div>
        </div>
        <div className={classes.Row}>
          <div className={classes.Label}>{props.paymentAmountLabel}</div>
          <div className={classes.Value}>{props.paymentAmountValue}</div>
        </div>
        {props.modelValue ? (
          <div className={classes.Row}>
            <div className={classes.Label}>{props.modelLabel}</div>
            <div className={classes.Value}>{props.modelValue}</div>
          </div>
        ) : null}
        {props.referenceValue ? (
          <div className={classes.Row}>
            <div className={classes.Label}>{props.referenceLabel}</div>
            <div className={classes.Value}>{props.referenceValue}</div>
          </div>
        ) : null}
        {props.paymentCodeValue ? (
          <div className={classes.Row}>
            <div className={classes.Label}>{props.paymentCodeLabel}</div>
            <div className={classes.Value}>{props.paymentCodeValue}</div>
          </div>
        ) : null}
        <div className={classes.Row}>
          <div className={classes.Label}>{props.paymentPurposeLabel}</div>
          <div className={classes.Value}>{props.paymentPurposeValue}</div>
        </div>
      </div>
      <div className={classes.NavigationWrapper}>
        <div className={classes.Close} onClick={props.handleOnClose}>
          {props.closeLabel}
        </div>
        <div className={classes.Submit} onClick={props.handleOnSubmit}>
          {props.submitLabel}
        </div>
      </div>
    </div>
  );

  if (props.paymentStatus === FETCH_DATA_STATUS.LOADING) {
    content = <Loader />;
  } else if (props.paymentError) {
    content = (
      <div className={classes.PaymentSummary}>
        <div className={classes.ErrorContent}>{props.paymentError}</div>
      </div>
    );
  } else if (props.paymentStatus === FETCH_DATA_STATUS.SUCEEDED) {
    content = (
      <div className={classes.PaymentSummary}>
        <div className={classes.SuccessContent}>
          {props.paymentSuceededMessage}
        </div>
      </div>
    );
  }

  return <div>{content}</div>;
};

export default PaymentSummary;
