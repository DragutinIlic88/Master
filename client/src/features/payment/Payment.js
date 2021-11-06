import React, { useEffect, useState, useMemo } from "react";
import { useTranslation } from "react-i18next";
import { useSelector, useDispatch } from "react-redux";
import { useErrorTranslations } from "../../useErrorTranslations";

import {
  selectAccounts,
  selectStatus,
  selectAccountError,
  getAccounts,
} from "../home/homeSlice";

import {
  selectPaymentError,
  selectPaymentStatus,
  sendPaymentInfo,
  resetPaymentInformation,
} from "./paymentSlice";

import { selectToken } from "../login/loginSlice";
import { FETCH_DATA_STATUS } from "../../constants";
import AccountSlider from "../../components/UI/AccountSlider/AccountSlider";
import PaymentForm from "./PaymentForm/PaymentForm";
import PaymentSummary from "./PaymentSummary/PaymentSummary";
import Modal from "../../components/UI/Modal/Modal";
import classes from "./Payment.module.css";

/**
 * Component for rendering information regarding payment service to the logged in user.
 * Component has AccountSlider component where user can choose account from which payment
 * would be carried out.
 * Component also has PaymentForm component where user can insert all details regarding payment.
 * When user wants to submit payment summary will be displayed where user can confirm payment.
 * After that informations are sent to redux and server and payment is done.
 */
const Payment = () => {
  //#region init data
  const { t } = useTranslation();
  const accounts = useSelector(selectAccounts);
  const status = useSelector(selectStatus);
  const token = useSelector(selectToken);
  const error = useSelector(selectAccountError);
  const paymentError = useSelector(selectPaymentError);
  const paymentStatus = useSelector(selectPaymentStatus);

  const initalPaymentForm = {
    beneficiaryCustomer: "",
    beneficiaryCustomerAccount: "",
    paymentAmount: "",
    model: "",
    reference: "",
    paymentCode: "",
    paymentPurpose: "",
  };

  const [displayedAccount, setDisplayedAccount] = useState(null);
  const [paymentForm, setPaymentForm] = useState(initalPaymentForm);

  const [paymentFormErrors, setPaymentFormErrors] = useState({
    beneficiaryCustomerError: "",
    beneficiaryCustomerAccountError: "",
    paymentAmountError: "",
    modelError: "",
    referenceError: "",
    paymentCodeError: "",
    paymentPurposeError: "",
  });

  const [showPaymentSummary, setShowPaymentSummary] = useState(false);

  const dispatch = useDispatch();

  const { translateErrors } = useErrorTranslations();

  const paymentErrorTranslations = useMemo(
    () => translateErrors(paymentError),
    [paymentError, translateErrors]
  );

  const accountErrorTranslations = useMemo(
    () => translateErrors(error),
    [error, translateErrors]
  );
  //#endregion

  //#region  event handlers and helper methods
  /**
   * Event handler is triggered when user clicks on account slider left arrow.
   * Previous account from array will be shown , or last one in case that current was first one in array.
   */
  const handleOnLeftArrowClick = () => {
    const index = accounts
      .map((acc) => {
        return acc.accountNumber;
      })
      .indexOf(displayedAccount.accountNumber);
    if (index === 0) {
      setDisplayedAccount(accounts[accounts.length - 1]);
    } else {
      setDisplayedAccount(accounts[index - 1]);
    }
  };

  /**
   * Event handler is triggered when user clicks on account slider right arrow.
   * Next account from array will be shown , or first one in case that current was last one in array.
   */
  const handleOnRightArrowClick = () => {
    const index = accounts
      .map((acc) => {
        return acc.accountNumber;
      })
      .indexOf(displayedAccount.accountNumber);

    if (index === accounts.length - 1) {
      setDisplayedAccount(accounts[0]);
    } else {
      setDisplayedAccount(accounts[index + 1]);
    }
  };

  /**
   * Event handler is triggered when user wants to close payment summary without proceeding.
   * All typed in information will be reseted to default and summary page will be closed
   */
  const handleClosePaymentSummary = () => {
    if (paymentStatus === FETCH_DATA_STATUS.SUCEEDED) {
      setPaymentForm(initalPaymentForm);
      dispatch(resetPaymentInformation());
    }
    setShowPaymentSummary(false);
  };

  /**
   * Helper method for validating all fields in form.
   * In case that some field is invalid proper error message will
   * be displayed.
   */
  const validateForm = () => {
    let valid = true;
    let errorObject = {
      beneficiaryCustomerError: "",
      beneficiaryCustomerAccountError: "",
      paymentAmountError: "",
      modelError: "",
      referenceError: "",
      paymentCodeError: "",
      paymentPurposeError: "",
    };

    if (paymentForm.beneficiaryCustomer === "") {
      valid = false;
      errorObject.beneficiaryCustomerError = t(
        "BeneficiaryCustomerRequiredError"
      );
    }
    if (paymentForm.beneficiaryCustomerAccount === "") {
      valid = false;
      errorObject.beneficiaryCustomerAccountError = t(
        "BeneficiaryCustomerAccountRequiredError"
      );
    }
    if (
      paymentForm.paymentAmount === "" ||
      parseFloat(paymentForm.paymentAmount) === 0
    ) {
      valid = false;
      errorObject.paymentAmountError = t("PaymentAmountRequiredError");
    }
    if (paymentForm.paymentPurpose === "") {
      valid = false;
      errorObject.paymentPurposeError = t("PaymentPurposeRequiredError");
    }

    setPaymentFormErrors(errorObject);
    return valid;
  };

  /**
   * Event handler is triggered when user clicks on submit button on form component.
   * Form validation is prefformed and summary page is shown to the user.
   * @param {object} event
   */
  const handleOnPaymentFormSubmit = (event) => {
    event.preventDefault();
    if (validateForm()) {
      setShowPaymentSummary(true);
    }
  };

  /**
   * Event handler is triggered when user press submit on summary page.
   * Information is dispatched to the redux which sends request to the server.
   */
  const handleOnPaymentSubmit = () => {
    const requestData = {
      userToken: token,
      accountNumber: displayedAccount.accountNumber,
      beneficiaryCustomer: paymentForm.beneficiaryCustomer,
      beneficiaryCustomerAccount: paymentForm.beneficiaryCustomerAccount,
      amount: paymentForm.paymentAmount,
      currency: displayedAccount.currency,
      model: paymentForm.model,
      reference: paymentForm.reference,
      paymentCode: paymentForm.paymentCode,
      paymentPurpose: paymentForm.paymentPurpose,
    };

    dispatch(sendPaymentInfo(requestData));
  };

  /**
   * Event handler is triggered when user changes beneficiary customer field
   * @param {object} event
   */
  const handleOnBeneficiaryCustomerChanged = (event) => {
    setPaymentForm({ ...paymentForm, beneficiaryCustomer: event.target.value });
  };

  /**
   * Event handler is triggered when user changes beneficiary customer account field
   * @param {object} event
   */
  const handleOnBeneficiaryCustomerAccountChanged = (event) => {
    setPaymentForm({
      ...paymentForm,
      beneficiaryCustomerAccount: event.target.value,
    });
  };

  /**
   * Event handler is triggered when user changes amount filed
   * @param {object} event
   */
  const handleOnPaymentAmountChanged = (event) => {
    setPaymentForm({
      ...paymentForm,
      paymentAmount: event.target.value,
    });
  };

  /**
   * Event handler is triggered when user changes model filed
   * @param {object} event
   */
  const handleOnModelChange = (event) => {
    setPaymentForm({
      ...paymentForm,
      model: event.target.value,
    });
  };

  /**
   * Event handler is triggered when user changes reference number field
   * @param {object} event
   */
  const handleOnReferenceChange = (event) => {
    setPaymentForm({
      ...paymentForm,
      reference: event.target.value,
    });
  };

  /**
   * Event handler is triggered when user changes payment code field
   * @param {object} event
   */
  const handleOnPaymentCodeChange = (event) => {
    setPaymentForm({
      ...paymentForm,
      paymentCode: event.target.value,
    });
  };

  /**
   * Event handler is triggered when user changes purpose of payment field
   * @param {object} event
   */
  const handleOnPaymentPurposeChanged = (event) => {
    setPaymentForm({
      ...paymentForm,
      paymentPurpose: event.target.value,
    });
  };
  //#endregion

  //#region lifecycle methods
  /**
   * Lifecycle method is called to gets accounts from server.
   * Information is dispatched to the redux so it can retreive it from server.
   */
  useEffect(() => {
    dispatch(
      getAccounts({
        userToken: token,
      })
    );
  }, [token, dispatch]);
  //#endregion

  if (status === FETCH_DATA_STATUS.SUCEEDED) {
    if (displayedAccount === null) {
      setDisplayedAccount(accounts[0]);
    }
  }

  return (
    <div className={classes.Payment}>
      <Modal show={showPaymentSummary} modalClosed={handleClosePaymentSummary}>
        {showPaymentSummary ? (
          <PaymentSummary
            paymentSuceededMessage={t("PaymentSuceed")}
            paymentStatus={paymentStatus}
            paymentError={paymentErrorTranslations}
            headerLabel={t("PaymentSummary")}
            beneficiaryCustomerLabel={t("BeneficiaryCustomer")}
            beneficiaryCustomerValue={paymentForm.beneficiaryCustomer}
            beneficiaryCustomerAccountLabel={t("BeneficiaryCustomerAccount")}
            beneficiaryCustomerAccountValue={
              paymentForm.beneficiaryCustomerAccount
            }
            paymentAmountLabel={t("PaymentAmount")}
            paymentAmountValue={paymentForm.paymentAmount}
            modelLabel={t("Model")}
            modelValue={paymentForm.model}
            referenceLabel={t("Reference")}
            referenceValue={paymentForm.reference}
            paymentCodeLabel={t("PaymentCode")}
            paymentCodeValue={paymentForm.paymentCode}
            paymentPurposeLabel={t("PaymentPurpose")}
            paymentPurposeValue={paymentForm.paymentPurpose}
            closeLabel={t("Close")}
            handleOnClose={handleClosePaymentSummary}
            submitLabel={t("Submit")}
            handleOnSubmit={handleOnPaymentSubmit}
          />
        ) : null}
      </Modal>
      <AccountSlider
        status={status}
        displayedAccount={displayedAccount}
        accounts={accounts}
        handleOnLeftArrowClick={handleOnLeftArrowClick}
        handleOnRightArrowClick={handleOnRightArrowClick}
        leftArrowAlt={t("LeftArrow")}
        rightArrowAlt={t("RightArrow")}
        balanceHeaderTitle={t("PaymentAccount")}
        error={accountErrorTranslations}
      />
      {status !== FETCH_DATA_STATUS.FAILED &&
        Array.isArray(accounts) &&
        accounts.length > 0 && (
          <PaymentForm
            handleFormSubmit={handleOnPaymentFormSubmit}
            beneficiaryCustomerLabel={t("BeneficiaryCustomer")}
            beneficiaryCustomerValue={paymentForm.beneficiaryCustomer}
            beneficiaryCustomerError={
              paymentFormErrors.beneficiaryCustomerError
            }
            handleOnBeneficiaryCustomerChanged={
              handleOnBeneficiaryCustomerChanged
            }
            beneficiaryCustomerAccountLabel={t("BeneficiaryCustomerAccount")}
            beneficiaryCustomerAccountValue={
              paymentForm.beneficiaryCustomerAccount
            }
            handleOnBeneficiaryCustomerAccountChanged={
              handleOnBeneficiaryCustomerAccountChanged
            }
            beneficiaryCustomerAccountError={
              paymentFormErrors.beneficiaryCustomerAccountError
            }
            paymentAmountLabel={t("PaymentAmount")}
            paymentAmountValue={paymentForm.paymentAmount}
            handleOnPaymentAmountChanged={handleOnPaymentAmountChanged}
            paymentAmountAdditionalAttrs={{
              min: "0",
              max: displayedAccount !== null ? displayedAccount.amount : "1000",
              step: "0.01",
            }}
            paymentAmountError={paymentFormErrors.paymentAmountError}
            modelLabel={t("Model")}
            modelValue={paymentForm.model}
            handleOnModelChange={handleOnModelChange}
            modelAdditionalAttrs={{ min: "0", max: "100" }}
            modelError={paymentFormErrors.modelError}
            referenceLabel={t("Reference")}
            referenceValue={paymentForm.reference}
            referenceError={paymentFormErrors.referenceError}
            handleOnReferenceChange={handleOnReferenceChange}
            paymentCodeLabel={t("PaymentCode")}
            paymentCodeValue={paymentForm.paymentCode}
            handleOnPaymentCodeChange={handleOnPaymentCodeChange}
            paymentCodeAdditionalAttrs={{ min: "0", step: "1" }}
            paymentCodeError={paymentFormErrors.paymentCodeError}
            paymentPurposeLabel={t("PaymentPurpose")}
            paymentPurposeError={paymentFormErrors.paymentPurposeError}
            paymentPurposeValue={paymentForm.paymentPurpose}
            handleOnPaymentPurposeChanged={handleOnPaymentPurposeChanged}
            submitButtonValue={t("Submit")}
          />
        )}
    </div>
  );
};

export default Payment;
