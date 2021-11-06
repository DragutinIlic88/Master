import React, { useState, useEffect, useMemo } from "react";
import { useTranslation } from "react-i18next";
import { useSelector, useDispatch } from "react-redux";

import {
  selectAccounts,
  selectStatus,
  selectAccountError,
  getAccounts,
  setAccountStatus,
} from "../home/homeSlice";
import { selectToken } from "../login/loginSlice";

import {
  selectExchangeError,
  selectCurrenciesStatus,
  selectCurrencies,
  selectFee,
  selectFeeStatus,
  selectExchangeStatus,
  getCurrencies,
  getFeeInfo,
  confirmExchange,
  resetFeeInformation,
  resetExchangeInformation,
} from "./exchangeSlice";

import { numberPattern } from "../../utility";

import { FETCH_DATA_STATUS } from "../../constants";
import { useErrorTranslations } from "../../useErrorTranslations";
import SelectButton from "../../components/UI/SelectButton/SelectButton";
import CommonInput from "../../components/UI/CommonInput/CommonInput";
import CommonButton from "../../components/UI/Buttons/CommonButton/CommonButton";
import Loader from "../../components/UI/Loader/Loader";
import Modal from "../../components/UI/Modal/Modal";

import classes from "./Exchange.module.css";

/**
 * Component is used for presenting user with exchange service.
 * First user can choose account from which it wants to exchange money in select input,
 * can insert amount which it wants to exchange and can choose currency from select input.
 * Available accounts and currencies are retreived from server when component is mounted.
 * After user choose account, currency and amount , it can click continue button and
 * exchange fee will be retrieved from server and displayed to the user. Also input filed
 * where user can type account where exchaged funds will be sent is displayed. User can go
 * back to initial form or can preform exchange by clicking exchange button. Modal with
 * summary will be displayed to the user in case of successful exchange.
 * @component
 */
const Exchange = () => {
  //#region init data
  let selectedAccount = null;
  let feeInfoContent = null;
  let exchangeConfirmContent = null;
  let form = <Loader />;
  const { t } = useTranslation();
  const accounts = useSelector(selectAccounts);
  const status = useSelector(selectStatus);
  const error = useSelector(selectAccountError);
  const token = useSelector(selectToken);
  const currencies = useSelector(selectCurrencies);
  const currenciesStatus = useSelector(selectCurrenciesStatus);
  const exchangeStatus = useSelector(selectExchangeStatus);
  const exchangeError = useSelector(selectExchangeError);
  const fee = useSelector(selectFee);
  const feeStatus = useSelector(selectFeeStatus);
  const [fromAccount, setFromAccount] = useState("");
  const [toAccount, setToAccount] = useState("");
  const [inputAmount, setInputAmount] = useState(0);
  const [amountInputError, setAmountInputError] = useState(null);
  const [selectedCurrency, setSelectedCurrency] = useState(null);
  const [fromCurrency, setFromCurrency] = useState(null);
  const [formDisabled, setFormDisabled] = useState(false);
  const [showConfrimExchange, setShowConfirmExchange] = useState(false);
  const dispatch = useDispatch();

  const { translateErrors } = useErrorTranslations();

  const exchangeErrorTranslations = useMemo(
    () => translateErrors(exchangeError),
    [exchangeError, translateErrors]
  );

  const accountErrorTranslations = useMemo(
    () => translateErrors(error),
    [error, translateErrors]
  );
  //#endregion

  //#region  lifecycle methods
  /**
   * Lifecycle method triggered every time when user, dispatch , currenciesStatus or status
   * variables are updated. Gets all accounts and currencies available for loged in user.
   * @lifecycle useEffect
   *
   */
  useEffect(() => {
    setFromAccount("");
    dispatch(
      getAccounts({
        userToken: token,
      })
    );

    setSelectedCurrency(null);
    dispatch(
      getCurrencies({
        userToken: token,
      })
    );
  }, [token, dispatch]);
  //#endregion

  //#region event handlers and helper methods
  /**
   * Event handler is triggered when user change account from which
   * funds will be taken for exchange. If funds on that amount are less than
   * inserted amount proper error will be displayed to the user.
   * @param {object} e
   */
  const onAccountChange = (e) => {
    setFromAccount(e.target.value);
    const currentAccount = getAccountFromAccountNumber(
      e.target.value,
      accounts
    );
    setFromCurrency(currentAccount.currency);
    if (inputAmount > currentAccount.amount) {
      setAmountInputError(t("NotEnoughFunds"));
    } else {
      setAmountInputError(null);
    }
  };

  /**
   * Helper function returns first account from list of accounts where account number passed as
   * first parameter is equal as accountNumber of list element.
   * @param {string} accNumber
   * @param {Array} accList
   */
  const getAccountFromAccountNumber = (accNumber, accList) => {
    const acc = accList.filter(
      (account) => account.accountNumber === accNumber
    )[0];
    return acc;
  };

  /**
   * Event handler is triggered when user inserts amount which it wants to exchange.
   * Inserted amount is set as current state and checking is performed if amount is negative or
   * if amount exceed funds on choosen account. If that is the case proper error is displayed to
   * the user.
   * @param {object} e
   */
  const handleOnAmountInputChange = (e) => {
    if (e.target.value !== "") {
      const amount = parseFloat(e.target.value);
      if (amount < 0) {
        setInputAmount(amount);
        setAmountInputError(t("NegativeAmountInputError"));
      } else if (amount > selectedAccount.amount) {
        setInputAmount(amount);
        setAmountInputError(t("NotEnoughFunds"));
      } else {
        setAmountInputError(null);
        setInputAmount(amount);
      }
    }
  };

  /**
   * Event handler is triggered when account where excahnged funds will be transfered is changed.
   * That value is set as current state.
   * @param {object} e
   */
  const handleOnToAccountChanged = (e) => {
    setToAccount(e.target.value);
  };

  /**
   * Event handler is triggered when currency is changed.
   * That value is set as current state.
   * @param {object} e
   */
  const handleOnCurrencyChange = (e) => {
    setSelectedCurrency(e.target.value);
  };

  /**
   * Event handler is triggered when user whants to continue
   * with exchanging process. Dispatch method is called so it
   * can retreive fee information based of currency of account
   * and selected currency.
   */
  const handleOnContinueExchange = () => {
    dispatch(
      getFeeInfo({
        userToken: token,
        fromCurrency: selectedAccount.currency,
        toCurrency: selectedCurrency,
      })
    );
  };

  /**
   * Event handler is triggered when user confirms exchange.
   * Informations about user, accounts ,currency and rate are
   * dispatched to redux so it can send it to server.
   */
  const handleOnFinishExchange = () => {
    dispatch(
      confirmExchange({
        userToken: token,
        fromAccount,
        toAccount,
        toCurrency: selectedCurrency,
        rate: parseFloat(fee.exchangeRate),
        fromCurrency,
        amount: inputAmount,
      })
    );
  };

  /**
   * Event handler is triggered when user whants to go back
   * to the first form of exchange process. Form is again enabled
   * for updateing and fee information are rested on redux state.
   */
  const handleOnBackClicked = () => {
    setFormDisabled(false);
    dispatch(resetFeeInformation());
  };

  /**
   * Event handler is triggered when user clicks ok button on
   * modal which is pressented for user as summary afger exchange process
   * is finished. React and redux state is reseted to default values.
   */
  const handleOnConfirmExchangeClosed = () => {
    dispatch(resetExchangeInformation());
    dispatch(setAccountStatus(FETCH_DATA_STATUS.IDLE));
    setShowConfirmExchange(false);
    setFormDisabled(false);
    setToAccount("");
    setInputAmount(0);
    dispatch(
      getAccounts({
        userToken: token,
      })
    );
    dispatch(
      getCurrencies({
        userToken: token,
      })
    );
  };
  //#endregion

  //account and amount form rendering
  /**
   * Part of code which reffers to rendering of initial exchange form.
   * If accounts or currencies are not still retrieved from server Loader
   * component will be displayed. Otherwise logic for transforming account
   * and currencies list to format suitable for select input is performed
   * and initial values are allocated to state.
   * As last step form content is made.
   * In case there was error while retrieving accounts or currencies proper
   * error message is displayed.
   */
  if (
    status === FETCH_DATA_STATUS.LOADING ||
    currenciesStatus === FETCH_DATA_STATUS.LOADING
  ) {
    form = <Loader />;
  } else if (
    status === FETCH_DATA_STATUS.SUCEEDED &&
    currenciesStatus === FETCH_DATA_STATUS.SUCEEDED
  ) {
    if (fromAccount === "") {
      setFromAccount(accounts[0].accountNumber);
      setFromCurrency(accounts[0].currency);
    } else {
      if (selectedCurrency === null) {
        setSelectedCurrency(currencies[0].code);
      } else {
        const accountItems = accounts.map((acc) => {
          return { value: acc.accountNumber, text: acc.accountNumber };
        });

        selectedAccount = getAccountFromAccountNumber(fromAccount, accounts);

        const currencyItems = currencies.map((curr) => {
          return { value: curr.code, text: curr.code };
        });

        form = (
          <div className={classes.ExchangeForm}>
            <SelectButton
              labelText={t("SelectAccount")}
              selectValue={fromAccount}
              onSelectChange={onAccountChange}
              items={accountItems}
              style={{ width: "60%" }}
              disabled={formDisabled}
            />
            <div className={classes.Resource}>
              {t("CurrentResource")}
              <span>{selectedAccount.amount}</span>
              <span>{selectedAccount.currency}</span>
            </div>
            <div className={classes.AmountCurrencyDiv}>
              <div className={classes.CommonInputWrapper}>
                <CommonInput
                  labelText={t("ExchangeAmount")}
                  inputType="number"
                  additionalProps={{ min: 0, max: selectedAccount.amount }}
                  inputValue={inputAmount.toString()}
                  onInputChange={handleOnAmountInputChange}
                  error={amountInputError}
                  disabled={formDisabled}
                />
              </div>
              <SelectButton
                labelText={t("Currency")}
                items={currencyItems}
                selectValue={selectedCurrency}
                onSelectChange={handleOnCurrencyChange}
                style={{ width: "30%", height: "145px" }}
                disabled={formDisabled}
              />
            </div>
            <div className={classes.Continue}>
              <CommonButton
                buttonLabel={t("Continue")}
                buttonClicked={handleOnContinueExchange}
                disabled={formDisabled}
              />
            </div>
          </div>
        );
      }
    }
  } else if (
    status === FETCH_DATA_STATUS.FAILED ||
    currenciesStatus === FETCH_DATA_STATUS.FAILED
  ) {
    form = (
      <div className={classes.ErrorWrapper}>
        {error != null ? (
          <div className={classes.AccountsError}>
            {accountErrorTranslations}
          </div>
        ) : null}
        {exchangeError !== null ? (
          <div className={classes.CurrenciesError}>
            {t("ExchangeCanNotBePerformed")}
          </div>
        ) : null}
      </div>
    );
  }

  //feeInfo content rendering part
  /**
   * Part of code which renders fee information, input field for account
   * where exchange funds will be sent, and buttons for exchanging or going back.
   * While fee info are retrieved from server Loader component is displayed.
   * If some error occures it will be displayed to the user. Otherwise first form
   * is disabled for editing and next form is presented to the user.
   */
  if (feeStatus === FETCH_DATA_STATUS.LOADING) {
    feeInfoContent = <Loader />;
  } else if (feeStatus === FETCH_DATA_STATUS.FAILED) {
    feeInfoContent = (
      <div className={classes.FeeInfoErrorWrapper}>
        {exchangeErrorTranslations}
      </div>
    );
  } else if (feeStatus === FETCH_DATA_STATUS.SUCEEDED) {
    if (!formDisabled) {
      setFormDisabled(true);
    }

    feeInfoContent = (
      <div className={classes.FeeInfoContent}>
        <div className={classes.FeeInfo}>
          <div className={classes.Info}>
            <div>
              {t("RateOn")}
              {fee.dateOfInsert}
              {t("DateFrom")}
              {fee.fromCurrency}
              {t("To")}
              {fee.toCurrency}
              {t("Is")}
              {fee.exchangeRate}
            </div>
            <div>
              {t("ForAmount")}
              {inputAmount} {fee.fromCurrency}
              {t("WillGet")}
              {parseFloat(inputAmount) * parseFloat(fee.exchangeRate)}{" "}
              {fee.toCurrency}
            </div>
          </div>
          <div className={classes.ToAccount}>
            <CommonInput
              labelText={t("ToAccount")}
              inputType="text"
              pattern={numberPattern}
              inputValue={toAccount}
              onInputChange={handleOnToAccountChanged}
            />
          </div>
        </div>
        <div className={classes.NavigationPart}>
          <div className={classes.Continue}>
            <CommonButton
              buttonLabel={t("Exchange")}
              buttonClicked={handleOnFinishExchange}
            />
          </div>
          <div className={classes.Back}>
            <CommonButton
              buttonLabel={t("Back")}
              buttonClicked={handleOnBackClicked}
            />
          </div>
        </div>
      </div>
    );
  }

  /**
   * When user press exchange button modal with summary will be shown in case of success or
   * proper error message in case that something goes wrong.
   */
  if (exchangeStatus === FETCH_DATA_STATUS.LOADING) {
    exchangeConfirmContent = <Loader />;
  } else if (exchangeStatus === FETCH_DATA_STATUS.FAILED) {
    exchangeConfirmContent = (
      <div className={classes.ExchangeError}>
        {" "}
        {t("ExchangeCanNotBePerformed")}
      </div>
    );
    if (!showConfrimExchange) {
      setShowConfirmExchange(true);
    }
  } else if (exchangeStatus === FETCH_DATA_STATUS.SUCEEDED) {
    if (!showConfrimExchange) {
      setShowConfirmExchange(true);
    }

    exchangeConfirmContent = (
      <div className={classes.ConfirmExchange}>
        <div className={classes.ExchangeSuccess}>{t("ExchangeSuccess")}</div>
        <div className={classes.ExchangeConfirmInfo}>
          <div className={classes.ExchangeConfirmAccount}>
            {t("FromAccount")}
            {fromAccount}{" "}
          </div>
          <div className={classes.ExchangeConfirmFundsRemoved}>
            {t("FundsRemoved")}
            <span>{inputAmount}</span> <span>{selectedAccount.currency}</span>
          </div>
          <div className={classes.ExchangeConfirmAccountTo}>
            {t("AccountSendTo")}
            {toAccount}
          </div>
          <div className={classes.ExchangeConfirmFundsAdded}>
            {t("FundsAdded")}
            {parseFloat(inputAmount) * parseFloat(fee.exchangeRate)}{" "}
            {fee.toCurrency}
          </div>

          <div className={classes.ExchangeConfirmRate}>
            {t("UsedRate")} {fee.exchangeRate}
          </div>
        </div>
        <div className={classes.PositionButton}>
          <div
            className={classes.HideConfirmExchange}
            onClick={handleOnConfirmExchangeClosed}
          >
            {t("OK")}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className={classes.Exchange}>
      <Modal
        show={showConfrimExchange}
        modalClosed={handleOnConfirmExchangeClosed}
      >
        {exchangeConfirmContent}
      </Modal>
      <div className={classes.ExchangeHeader}>{t("ExchangeForm")}</div>
      {form}
      {feeInfoContent}
    </div>
  );
};

export default Exchange;
