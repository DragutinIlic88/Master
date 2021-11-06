import React, { useState, useEffect, useMemo } from "react";
import moment from "moment";
import _ from "lodash";
import { useTranslation } from "react-i18next";
import { useSelector, useDispatch } from "react-redux";
import { Link, useRouteMatch, Redirect } from "react-router-dom";

import {
  getLoans,
  postLoan,
  selectLoans,
  selectLoanStatus,
  selectPostLoanStatus,
  selectError,
  resetLoansState,
} from "./loanSlice";

import {
  getCurrencies,
  selectCurrenciesStatus,
  selectCurrencies,
} from "../exchange/exchangeSlice";
import { useErrorTranslations } from "../../useErrorTranslations";
import { selectToken } from "../login/loginSlice";
import LoanList from "./LoanList/LoanList";
import LoanDetails from "./LoanDetails/LoanDetails";
import LoanForm from "./LoanForm/LoanForm";
import Modal from "../../components/UI/Modal/Modal";
import { FETCH_DATA_STATUS, SERVICES_PATH } from "../../constants";
import classes from "./Loan.module.css";
import LoanConfirmation from "./LoanConfirmation/LoanConfirmation";

/**
 * @component
 * Componennt for handling logic and displaying information of Loan service.
 * Component also handles routing between displaying all loans page for user and creating new loan page.
 * If user clicks on some loan from loan list , LoanDetails component will show detailed information about that loan.
 * On new loan page after user inserts all neccassery information and presses enter LoanConfirmation component will
 * show summary of loan information.
 */
const Loan = () => {
  //#region initialization
  const { t } = useTranslation();
  const token = useSelector(selectToken);
  const loans = useSelector(selectLoans);
  const loanStatus = useSelector(selectLoanStatus);
  const postLoanStatus = useSelector(selectPostLoanStatus);
  const error = useSelector(selectError);
  const currencies = useSelector(selectCurrencies);
  const currenciesStatus = useSelector(selectCurrenciesStatus);
  const dispatch = useDispatch();
  let loanDetailsContent = null;
  let labels = null;
  let today = new Date();
  let maxLoanDate = new Date();
  maxLoanDate.setFullYear(maxLoanDate.getFullYear() + 30);

  const initialFormData = {
    fromAccount: "",
    receiveAccount: "",
    totalAmount: "",
    participation: null,
    currency: "",
    startDate: "",
    endDate: "",
    purpose: "",
    collateral: null,
  };

  const initialFormErrorsData = {
    fromAccountError: "",
    receiveAccountError: "",
    totalAmountError: "",
    participationError: "",
    startDateError: "",
    endDateError: "",
    purposeError: "",
  };

  const [loanDetails, setLoanDetails] = useState(null);
  const [formData, setFormData] = useState(initialFormData);
  const [showConfirmationPage, setShowConfirmationPage] = useState(false);

  const [formErrors, setFormErrors] = useState(initialFormErrorsData);
  const { path } = useRouteMatch();

  const { translateErrors } = useErrorTranslations();

  const loanErrorTranslations = useMemo(
    () => translateErrors(error),
    [error, translateErrors]
  );

  //#endregion

  //#region  methods and event handlers

  /**
   * Method for seting formData , formErrors and showConfirmationPage on initial state
   */
  const resetPostForms = () => {
    setFormData(initialFormData);
    setFormErrors(initialFormErrorsData);
    setShowConfirmationPage(false);
  };

  /**
   * Event handler is triggered when user clicks on some loan to see detailed information
   * of the loan. Choosen loan is set as current state and modal is show with proper information.
   * @param {object} loan
   */
  const handleOnShowLoanDetails = (loan) => {
    setLoanDetails(loan);
  };

  /**
   * Event handler is triggered when user wants to close detailed information of
   * some loan. It sets current loan state to null and with that modal is closed.
   */
  const handleOnLoanDetailsClose = () => {
    setLoanDetails(null);
  };

  /**
   * Event handler is triggered when user wants to close confirmation page.
   * For some reason user doesn't want to proceed with new loan creation.
   * Modal containing summery of new loan will be closed.
   */
  const handleOnConfirmationPageClosed = () => {
    setShowConfirmationPage(false);
  };

  /**
   * Event handler is triggered when user whants to confirm new loan creation.
   * Information about user and new loan are dispatched to redux, so it can send it to
   * the server.
   */
  const handleOnConfirmationPageSubmit = () => {
    dispatch(
      postLoan({
        userToken: token,
        ...formData,
      })
    );
  };

  /**
   * Helper constant which contains currencies in form suitable for select input
   */
  const currencyItems = useMemo(
    () =>
      currencies.map((curr) => {
        return { value: curr.code, text: curr.code };
      }),
    [currencies]
  );

  /**
   * Helper method for form validation.
   * If some field of form is invalid , method add proper error message
   * and returns false
   */
  const validateForm = () => {
    let valid = true;
    let errorObject = initialFormErrorsData;

    if (formData.fromAccount === "") {
      valid = false;
      errorObject.fromAccountError = t("FromAccountRequiredError");
    }

    if (formData.receiveAccount === "") {
      valid = false;
      errorObject.receiveAccountError = t("ReceiveAccountRequiredError");
    }

    if (formData.totalAmount === "") {
      valid = false;
      errorObject.totalAmountError = t("TotalAmountRequiredError");
    }

    if (
      formData.totalAmount !== "" &&
      formData.participation !== "" &&
      parseFloat(formData.participation) > parseFloat(formData.totalAmount)
    ) {
      valid = false;
      errorObject.participationError = t("ParticipationTooLargeError");
    }

    if (formData.startDate === "") {
      valid = false;
      errorObject.startDateError = t("StartDateRequiredError");
    }

    if (formData.endDate === "") {
      valid = false;
      errorObject.endDateError = t("EndDateRequiredError");
    }

    if (formData.startDate !== "" && formData.endDate !== "") {
      const sd = new Date(formData.startDate);
      const ed = new Date(formData.endDate);
      if (sd.getTime() > ed.getTime()) {
        valid = false;
        errorObject.endDateError = t("EndDateBeforeStartDateError");
      }
    }

    setFormErrors(errorObject);
    return valid;
  };

  /**
   * Event handler is triggered when user fill form for new loan and clicks submit button.
   * Form is validated and if it is valid confirmation page is shown.
   * Otherwise errors for invalid fields are shown to the user.
   * @param {object} event
   */
  const handleFormSubmit = (event) => {
    event.preventDefault();
    if (validateForm()) {
      setShowConfirmationPage(true);
    }
  };

  /**
   * Event handler is triggered when from account field is changed.
   * @param {object} event
   */
  const handleOnFromAccountChanged = (event) => {
    setFormData({
      ...formData,
      fromAccount: event.target.value,
    });
  };

  /**
   * Event handler is triggered when receive account field is changed.
   * @param {object} event
   */
  const handleOnReceiveAccountChanged = (event) => {
    setFormData({
      ...formData,
      receiveAccount: event.target.value,
    });
  };

  /**
   * Event handler is triggered when total amount field is changed.
   * @param {object} event
   */
  const handleOnTotalAmountChanged = (event) => {
    setFormData({
      ...formData,
      totalAmount: event.target.value,
    });
  };

  /**
   * Event handler is triggered when participation field is changed.
   * @param {object} event
   */
  const handleOnParticipationChanged = (event) => {
    setFormData({
      ...formData,
      participation: event.target.value,
    });
  };

  /**
   * Event handler is triggered when currency select input is changed.
   * @param {object} event
   */
  const handleOnCurrencyChange = (event) => {
    setFormData({ ...formData, currency: event.target.value });
  };

  /**
   * Event handler is triggered when start date field is changed.
   * @param {object} event
   */
  const handleOnStartDateChanged = (event) => {
    setFormData({ ...formData, startDate: event.target.value });
  };

  /**
   * Event handler is triggered when end date field is changed.
   * @param {object} event
   */
  const handleOnEndDateChanged = (event) => {
    setFormData({ ...formData, endDate: event.target.value });
  };

  /**
   * Event handler is triggered when purpose of loan field is changed.
   * @param {object} event
   */
  const handleOnPPurposeChanged = (event) => {
    setFormData({ ...formData, purpose: event.target.value });
  };

  /**
   * Event handler is triggered when collateral field is changed.
   * @param {object} event
   */
  const handleOnCollateralChanged = (event) => {
    setFormData({ ...formData, collateral: event.target.value });
  };

  //#endregion

  //#region  effect lifecycle methods
  /**
   * Lifecycle method which gets all loans for logged in user.
   */
  useEffect(() => {
    if (path === SERVICES_PATH.LOAN) {
      dispatch(
        getLoans({
          userToken: token,
        })
      );
    }
  }, [dispatch, token, path]);

  /**
   * Lifecycle method which gets all available currencies for logged in user.
   */
  useEffect(() => {
    dispatch(
      getCurrencies({
        userToken: token,
      })
    );
  }, [dispatch, token]);

  //#endregion

  /**
   * In case that current loan is not null
   * content for that loan is created as LoanDetails component.
   */
  if (loanDetails) {
    loanDetailsContent = (
      <LoanDetails
        nameLabel={t("LoanName")}
        nameValue={loanDetails.loanName}
        fromAccountNumberLabel={t("LoanFromAccount")}
        fromAccountNumberValue={loanDetails.fromAccountNumber}
        toAccountNumberLabel={t("LoanToAccount")}
        toAccountNumberValue={loanDetails.toAccountNumber}
        typeLabel={t("LoanTypeLabel")}
        typeValue={loanDetails.loanType}
        creationDateLabel={t("LoanCretionDate")}
        creationDateValue={loanDetails.creationDate}
        endingDateLabel={t("LoanEndingDate")}
        endingDateValue={loanDetails.endingDate}
        statusLabel={t("LoanStatus")}
        statusValue={loanDetails.loanStatus}
        totalAmountLabel={t("LoanTotalAmount")}
        totalAmount={loanDetails.totalAmount}
        currency={loanDetails.currency}
        participationAmountLabel={t("LoanParticipationAmount")}
        participationAmount={loanDetails.participationAmount}
        collateralLabel={t("LoanCollateral")}
        collateralValue={loanDetails.collateral}
        purposeLabel={t("LoanPurposeSpecified")}
        purposeValue={loanDetails.purpose}
        remainingAmountLabel={t("LoanRemainingAmount")}
        remainingAmount={loanDetails.remainingAmount}
        debtAmountLabel={t("LoanDebtAmount")}
        debtAmount={loanDetails.debtAmount}
        paymentDeadlineDateLabel={t("LoanPaymentDeadlineDate")}
        paymentDeadlineDateValue={loanDetails.paymentDeadlineDate}
        hideContent={handleOnLoanDetailsClose}
        buttonLabel={t("Close")}
      />
    );
  }

  if (showConfirmationPage) {
    labels = {
      fromAccountLabel: t("LoanFromAccountConfirmationLabel"),
      receiveAccountLabel: t("LoanReceiveAccountConfirmationLabel"),
      totalAmountLabel: t("LoanTotalAmountConfirmationLabel"),
      participationLabel: t("LoanParticipationConifrmationLabel"),
      currencyLabel: t("LoanCurrencyConfirmationLabel"),
      startDateLabel: t("LoanStartDateConfirmationLabel"),
      endDateLabel: t("LoanEndDateConfirmationLabel"),
      purposeLabel: t("LoanPurposeConfirmationLabel"),
      collateralLabel: t("LoanCollateralConfirmationLabel"),
      submitLabel: t("Submit"),
      cancelLabel: t("Cancel"),
    };
  }

  let loanClasses = [classes.NavItem],
    newLoanClasses = [classes.NavItem];

  /**
   * If user wants to be on loans page, where list of all loans are displayed
   * and if it previously was on new loan page state needs to be initialize again
   * and proper css class is set
   */
  if (path === SERVICES_PATH.LOAN) {
    if (postLoanStatus !== FETCH_DATA_STATUS.IDLE) {
      dispatch(resetLoansState());
    }
    if (
      !_.isEqual(formData, initialFormData) ||
      !_.isEqual(formErrors, initialFormErrorsData)
    ) {
      resetPostForms();
    }

    loanClasses.push(classes.Active);
  } else {
    /**
     *if user whants to be on new loan page default currency value is set as first from
     *currencies array and proper class is set. After user submited new loan to the server
     *it is navigated on loans page where new loan is displayed with in progress status.
     */
    if (
      formData &&
      formData.currency === "" &&
      currenciesStatus === FETCH_DATA_STATUS.SUCEEDED
    ) {
      setFormData({ ...formData, currency: currencies[0].id });
    }

    if (postLoanStatus === FETCH_DATA_STATUS.SUCEEDED) {
      return <Redirect to={SERVICES_PATH.LOAN} push />;
    }

    newLoanClasses.push(classes.Active);
  }

  return (
    <div className={classes.Contariner}>
      <div className={classes.Navigation}>
        <ul>
          <li className={loanClasses.join(" ")}>
            <Link to={SERVICES_PATH.LOAN} className={classes.Link}>
              {t("LoanHistory")}
            </Link>
          </li>
          {loanStatus !== FETCH_DATA_STATUS.FAILED &&
            Array.isArray(currencies) &&
            currencies.length > 0 && (
              <li className={newLoanClasses.join(" ")}>
                <Link to={SERVICES_PATH.NEW_LOAN} className={classes.Link}>
                  {t("LoanNew")}
                </Link>
              </li>
            )}
        </ul>
      </div>
      {path === SERVICES_PATH.LOAN ? (
        <div className={classes.Loan}>
          <Modal
            show={loanDetails !== null}
            modalClosed={handleOnLoanDetailsClose}
          >
            {loanDetailsContent}
          </Modal>
          <LoanList
            headerTitle={t("LoanHistoryHeader")}
            noPreviousLoansMessage={t("NoLoan")}
            loansList={loans}
            loadingStatus={loanStatus}
            error={loanErrorTranslations}
            showLoanDetails={handleOnShowLoanDetails}
          />
        </div>
      ) : (
        <div className={classes.NewLoan}>
          {showConfirmationPage ? (
            <Modal
              show={showConfirmationPage}
              modalClosed={handleOnConfirmationPageClosed}
            >
              <LoanConfirmation
                data={formData}
                labels={labels}
                handleOnConfirmationPageClosed={handleOnConfirmationPageClosed}
                handleOnConfirmationPageSubmit={handleOnConfirmationPageSubmit}
                status={postLoanStatus}
                error={loanErrorTranslations}
              />
            </Modal>
          ) : null}
          <LoanForm
            headerTitle={t("LoanFormHeader")}
            fromAccountLabel={t("LoanFromAccountInsert")}
            fromAccountValue={formData.fromAccount}
            handleOnFromAccountChanged={handleOnFromAccountChanged}
            fromAccountError={formErrors.fromAccountError}
            receiveAccountLabel={t("LoanReceiveAccountInsert")}
            receiveAccountValue={formData.receiveAccount}
            handleOnReceiveAccountChanged={handleOnReceiveAccountChanged}
            receiveAccountError={formErrors.receiveAccountError}
            totalAmountLabel={t("LoanTotalAmount")}
            totalAmountValue={formData.totalAmount}
            handleOnTotalAmountChanged={handleOnTotalAmountChanged}
            paymentAmountAdditionalAttrs={{ min: "0", step: "0.01" }}
            totalAmountError={formErrors.totalAmountError}
            participationLabel={t("LoanParticipation")}
            participationValue={formData.participation}
            handleOnParticipationChanged={handleOnParticipationChanged}
            participationAdditionalAttrs={{ min: "0", step: "0.01" }}
            participationError={formErrors.participationError}
            currencyLabel={t("LoanCurrency")}
            currencyItems={currencyItems}
            handleOnCurrencyChange={handleOnCurrencyChange}
            startDateLabel={t("LoanStartDate")}
            startDate={formData.startDate}
            handleOnStartDateChanged={handleOnStartDateChanged}
            startDateAdditionalAttrs={{
              min: moment(today).format("YYYY-MM-DD"),
              max: moment(maxLoanDate).format("YYYY-MM-DD"),
            }}
            startDateError={formErrors.startDateError}
            endDateLabel={t("LoanEndDate")}
            endDate={formData.endDate}
            handleOnEndDateChanged={handleOnEndDateChanged}
            endDateAdditionalAttrs={{
              min: moment(today).format("YYYY-MM-DD"),
              max: moment(maxLoanDate).format("YYYY-MM-DD"),
            }}
            endDateError={formErrors.endDateError}
            purposeLabel={t("LoanPurpose")}
            purposeValue={formData.purpose}
            handleOnPPurposeChanged={handleOnPPurposeChanged}
            purposeError={formErrors.purposeError}
            collateralLabel={t("LoanCollateral")}
            colateralValue={formData.collateral}
            handleOnCollateralChanged={handleOnCollateralChanged}
            submitButtonValue={t("Submit")}
            handleFormSubmit={handleFormSubmit}
          />
        </div>
      )}
    </div>
  );
};

export default Loan;
