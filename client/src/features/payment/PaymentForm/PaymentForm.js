import React from "react";

import classes from "./PaymentForm.module.css";
import { formElementPatternCheckerDecorator } from "../../../utility";

/**
 * Component represents payment form with beneficiary customer, beneficiary customer's acocunt,
 * amount , model ,reference number ,payment code and purpose of payment fields.
 * In case that invalid value is inserted proper error message will be shown to the user.
 * Checking of value correctness is done by compareing value with pattern. Form has button
 * for submiting.
 * @param {object} props
 */
const PaymentForm = (props) => {
  return (
    <form className={classes.PaymentForm} onSubmit={props.handleFormSubmit}>
      <div className={classes.InputContent}>
        <div className={classes.FirstColumn}>
          <label className={classes.Label}>
            {props.beneficiaryCustomerLabel}
            <textarea
              value={props.beneficiaryCustomerValue}
              onChange={props.handleOnBeneficiaryCustomerChanged}
              className={classes.BeneficiaryCustomer}
            />
            <div className={classes.Error}>
              {props.beneficiaryCustomerError !== ""
                ? props.beneficiaryCustomerError
                : null}
            </div>
          </label>
          <label className={classes.Label}>
            {props.beneficiaryCustomerAccountLabel}
            <input
              type="text"
              value={props.beneficiaryCustomerAccountValue}
              onChange={(event) => {
                formElementPatternCheckerDecorator(
                  event,
                  props.handleOnBeneficiaryCustomerAccountChanged,
                  /^[0-9-]{0,21}$/
                );
              }}
              className={classes.BeneficiaryCustomerAccount}
            />
            <div className={classes.Error}>
              {props.beneficiaryCustomerAccountError !== ""
                ? props.beneficiaryCustomerAccountError
                : null}
            </div>
          </label>
          <label className={classes.Label}>
            {props.paymentAmountLabel}
            <input
              type="number"
              placeholder={0.0}
              value={props.paymentAmountValue}
              onChange={props.handleOnPaymentAmountChanged}
              {...props.paymentAmountAdditionalAttrs}
              className={classes.PaymentAmount}
            />
            <div className={classes.Error}>
              {props.paymentAmountError !== ""
                ? props.paymentAmountError
                : null}
            </div>
          </label>
        </div>
        <div className={classes.SecondColumn}>
          <div className={classes.ModelReferenceWrapper}>
            <label className={classes.Label}>
              {props.modelLabel}
              <input
                type="number"
                value={props.modelValue}
                onChange={(event) => {
                  formElementPatternCheckerDecorator(
                    event,
                    props.handleOnModelChange,
                    /^[0-9]{0,2}$/
                  );
                }}
                className={classes.Model}
                {...props.modelAdditionalAttrs}
              />
            </label>
            <label className={classes.Label}>
              {props.referenceLabel}
              <input
                type="text"
                value={props.referenceValue}
                onChange={(event) => {
                  formElementPatternCheckerDecorator(
                    event,
                    props.handleOnReferenceChange,
                    /^[0-9-\\]{0,18}$/
                  );
                }}
                className={classes.Reference}
                {...props.referenceAdditionalAttrs}
              />
            </label>
            <div className={classes.Error}>
              {props.modelError !== "" ? props.modelError : null}
              {props.referenceError !== "" ? props.referenceError : null}
            </div>
          </div>
          <label className={classes.Label}>
            {props.paymentCodeLabel}
            <input
              type="number"
              value={props.paymentCodeValue}
              onChange={(event) => {
                formElementPatternCheckerDecorator(
                  event,
                  props.handleOnPaymentCodeChange,
                  /^[0-9]{0,3}$/
                );
              }}
              className={classes.PaymentCode}
              {...props.paymentCodeAdditionalAttrs}
            />
            <div className={classes.Error}>
              {props.paymentCodeError !== "" ? props.paymentCodeError : null}
            </div>
          </label>
          <label className={classes.Label}>
            {props.paymentPurposeLabel}
            <textarea
              value={props.paymentPurposeValue}
              onChange={props.handleOnPaymentPurposeChanged}
              className={classes.PaymentPurpose}
            />
            <div className={classes.Error}>
              {props.paymentPurposeError !== ""
                ? props.paymentPurposeError
                : null}
            </div>
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
  );
};

export default PaymentForm;
