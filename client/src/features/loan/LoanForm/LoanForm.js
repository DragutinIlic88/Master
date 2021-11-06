import React from "react";

import { formElementPatternCheckerDecorator } from "../../../utility";

import classes from "./LoanForm.module.css";

/**
 * Component for diplaying form for new loan.
 * It displayes proper error messages and calls submit handler passed by parrent component.
 * All information and handlers are passed by parrent component via props object.
 * @param {object} props
 */
const LoanForm = (props) => {
  const options = props.currencyItems.map((item) => {
    return (
      <option key={item.value} value={item.value}>
        {item.text}
      </option>
    );
  });

  return (
    <div className={classes.LoanFormWrapper}>
      <div className={classes.LoanFormHeader}>{props.headerTitle}</div>
      <form className={classes.LoanForm} onSubmit={props.handleFormSubmit}>
        <div className={classes.InputContent}>
          <div className={classes.FirstColumn}>
            <label className={classes.Label}>
              {props.fromAccountLabel}
              <input
                type="text"
                value={props.fromAccountValue}
                onChange={(event) => {
                  formElementPatternCheckerDecorator(
                    event,
                    props.handleOnFromAccountChanged,
                    /^[0-9-]{0,21}$/
                  );
                }}
                className={classes.FromAccount}
              />
              <div className={classes.Error}>
                {props.fromAccountError !== "" ? props.fromAccountError : null}
              </div>
            </label>
            <label className={classes.Label}>
              {props.receiveAccountLabel}
              <input
                type="text"
                value={props.receiveAccountValue}
                onChange={(event) => {
                  formElementPatternCheckerDecorator(
                    event,
                    props.handleOnReceiveAccountChanged,
                    /^[0-9-]{0,21}$/
                  );
                }}
                className={classes.ReceiveAccount}
              />
              <div className={classes.Error}>
                {props.receiveAccountError !== ""
                  ? props.receiveAccountError
                  : null}
              </div>
            </label>
            <label className={classes.Label}>
              {props.totalAmountLabel}
              <input
                type="number"
                placeholder={0.0}
                value={props.totalAmountValue}
                onChange={props.handleOnTotalAmountChanged}
                {...props.paymentAmountAdditionalAttrs}
                className={classes.TotalAmount}
              />
              <div className={classes.Error}>
                {props.totalAmountError !== "" ? props.totalAmountError : null}
              </div>
            </label>
            <div className={classes.SameRow}>
              <label className={classes.Label}>
                {props.participationLabel}
                <input
                  type="number"
                  placeholder={0.0}
                  value={
                    props.participationValue ? props.participationValue : ""
                  }
                  onChange={props.handleOnParticipationChanged}
                  {...props.participationAdditionalAttrs}
                  className={classes.Participation}
                />
                <div className={classes.Error}>
                  {props.participationError !== ""
                    ? props.participationError
                    : null}
                </div>
              </label>
              <label className={classes.CurrencyLabel}>
                {props.currencyLabel}
                <select
                  className={classes.Currency}
                  value={props.selectedCurrency}
                  onChange={props.handleOnCurrencyChange}
                >
                  {options}
                </select>
              </label>
            </div>
          </div>
          <div className={classes.SecondColumn}>
            <label className={classes.Label}>
              {props.startDateLabel}
              <input
                type="date"
                value={props.startDate}
                onChange={props.handleOnStartDateChanged}
                {...props.startDateAdditionalAttrs}
                className={classes.StartDate}
              />
              <div className={classes.Error}>
                {props.startDateError !== "" ? props.startDateError : null}
              </div>
            </label>
            <label className={classes.Label}>
              {props.endDateLabel}
              <input
                type="date"
                value={props.endDate}
                onChange={props.handleOnEndDateChanged}
                {...props.endDateAdditionalAttrs}
                className={classes.EndDate}
              />
              <div className={classes.Error}>
                {props.endDateError !== "" ? props.endDateError : null}
              </div>
            </label>
            <label className={classes.Label}>
              {props.purposeLabel}
              <textarea
                value={props.purposeValue}
                onChange={props.handleOnPPurposeChanged}
                className={classes.Purpose}
              />
              <div className={classes.Error}>
                {props.purposeError !== "" ? props.purposeError : null}
              </div>
            </label>
            <label className={classes.Label}>
              {props.collateralLabel}
              <textarea
                value={props.colateralValue ? props.colateralValue : ""}
                onChange={props.handleOnCollateralChanged}
                className={classes.Collateral}
              />
            </label>
          </div>
        </div>
        <div className={classes.SubmitButtonWrapper}>
          <input
            type="submit"
            value={props.submitButtonValue}
            className={classes.SubmitButton}
          />
        </div>
      </form>
    </div>
  );
};

export default LoanForm;
