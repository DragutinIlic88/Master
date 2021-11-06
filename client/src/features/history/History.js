import React, { useState, useEffect, useMemo } from "react";
import { useTranslation } from "react-i18next";
import { useSelector, useDispatch } from "react-redux";

import {
  selectAccounts,
  selectStatus,
  setTransactionStatus,
  selectTransactionStatus,
  setTransactionError,
  selectTransactions,
  selectAccountError,
  selectTransactionError,
  getAccounts,
  getTransactions,
  setLoadMore,
} from "../home/homeSlice";
import { selectToken } from "../login/loginSlice";

import Accounts from "./Accounts/Accounts";
import TransactionList from "../../components/UI/TransactionList/TransactionList";
import AccountDetails from "./Accounts/AcountDetails/AccountDetails";
import TransactionDetails from "../../components/UI/TransactionList/TransactionDetails/TransactionDetails";
import Modal from "../../components/UI/Modal/Modal";
import { FETCH_DATA_STATUS } from "../../constants";
import { useErrorTranslations } from "../../useErrorTranslations";

import classes from "./History.module.css";

/**
 * Component for rendering information about accounts and transactions about logged in user.
 * All accounts which user have will be displayed in list with available balance and possibility
 * for displaying full detailed information for that account if user clicks on it.
 * Last ten transactions performed by user of selected account will be shown. When clicks on some,
 * full details for that transaction will be shown. User can load more earlier transactions by clicking
 * load more button.
 * @component
 */
const History = () => {
  //#region  init data
  const { t } = useTranslation();
  const accounts = useSelector(selectAccounts);
  const status = useSelector(selectStatus);
  const token = useSelector(selectToken);
  const transactionError = useSelector(selectTransactionError);
  const accountError = useSelector(selectAccountError);
  const transactions = useSelector(selectTransactions);
  const transactionStatus = useSelector(selectTransactionStatus);
  const dispatch = useDispatch();

  const [selectedAccount, setSelectedAccount] = useState(null);
  const [accountDetails, setAccountDetails] = useState(null);
  const [transactionDetails, setTransactionDetails] = useState(null);
  let accountDetailsContent = null;
  let transactionDetailsContent = null;
  const { translateErrors } = useErrorTranslations();

  const transactionErrorTranslations = useMemo(
    () => translateErrors(transactionError),
    [transactionError, translateErrors]
  );

  const accountErrorTranslations = useMemo(
    () => translateErrors(accountError),
    [accountError, translateErrors]
  );
  //#endregion

  //#region event handlers and helper methods
  /**
   * Event handler is triggered when user wants to close modal showing
   * account details.
   */
  const handleOnAccountDetailsClosed = () => {
    setAccountDetails(null);
  };

  /**
   * Event handler is triggered when user wants to close modal showing
   * transaction details.
   */
  const handleOnTransactionDetailsClosed = () => {
    setTransactionDetails(null);
  };

  /**
   * Event handler is triggered when details for clicked account need to be shown.
   * stopPropagation is called because we want that this event is triggered only
   * when button is pressed and we don't want to change selected account by that action.
   * @param {object} e event object
   * @param {object} acc  account object
   */
  const handleOnShowAccountDetails = (e, acc) => {
    e.stopPropagation();
    setAccountDetails(acc);
  };

  /**
   * Event handler is triggered when user clicks on some transaction.
   * That transaction will be set as current state for modal showing transaction details.
   * @param {object} tr transaction object
   */
  const handleOnShowTransactionDetails = (tr) => {
    setTransactionDetails(tr);
  };

  /**
   * Event handler is triggered when user clicks on some account.
   * That account will become selected ,and transaction history for that transaction
   * will be shown to the user.
   * @param {object} acc account object
   */
  const handleOnSelectAccountClick = (acc) => {
    setSelectedAccount(acc);
  };

  /**
   * Event handler is triggered when user wants to load more transactions for selected account.
   * Information is dispateched to redux ,which contact server and gets next ten transactions if
   * available.
   */
  const handleOnLoadMoreClicked = () => {
    dispatch(setLoadMore(true));
    dispatch(
      getTransactions({
        transactionsNumber: 10,
        userToken: token,
        accountNumber: selectedAccount.accountNumber,
        beginning: transactions ? transactions.length : 0,
      })
    );
  };
  //#endregion

  //#region  lifecycle methods
  /**
   * Lifecycle method gets all account available for logged in user
   */
  useEffect(() => {
    dispatch(
      getAccounts({
        userToken: token,
      })
    );
  }, [token, dispatch]);

  /**
   * Lifecycle method gets first ten transactions for selected account and logged in user.
   */
  useEffect(() => {
    if (selectedAccount !== null) {
      dispatch(
        getTransactions({
          transactionsNumber: 10,
          userToken: token,
          accountNumber: selectedAccount.accountNumber,
        })
      );
    }
  }, [token, selectedAccount, dispatch]);
  //#endregion

  if (status === FETCH_DATA_STATUS.SUCEEDED) {
    if (selectedAccount === null) {
      setSelectedAccount(accounts[0]);
    }
  }

  if (status === FETCH_DATA_STATUS.FAILED) {
    dispatch(setTransactionStatus(FETCH_DATA_STATUS.FAILED));
    dispatch(setTransactionError(accountError));
  }

  /**
   * Part of code renders AccountDetails component
   * with all neccessary information passed to it
   */
  if (accountDetails) {
    accountDetailsContent = (
      <AccountDetails
        accountNumberLabel={t("AccountNumber")}
        accountNumberValue={accountDetails.accountNumber}
        accountTypeLabel={t("AccountType")}
        accountTypeValue={accountDetails.accountType}
        accountDetailsLabel={t("AccountDetails")}
        accountDetailsValue={accountDetails.accountDetails}
        accountStateLabel={t("AccountState")}
        accountStateAmount={accountDetails.amount}
        accountStateCurrency={accountDetails.currency}
        accountDateLabel={t("AccountDate")}
        accountDateValue={accountDetails.creationDate}
        hideContent={handleOnAccountDetailsClosed}
        buttonLabel={t("Close")}
      />
    );
  }

  /**
   * Part of code renders TransactionDetails component
   * with all neccessary information passed to it
   */
  if (transactionDetails) {
    transactionDetailsContent = (
      <TransactionDetails
        transactionNameLabel={t("TransactionName")}
        transactionNameValue={transactionDetails.transactionName}
        accountNumberLabel={t("AccountNumber")}
        accountNumberValue={transactionDetails.accountNumber}
        transactionTypeLabel={t("TransactionType")}
        transactionTypeValue={transactionDetails.transactionType}
        transactionDateLabel={t("TransactionDate")}
        transactionDateValue={transactionDetails.creationTime}
        transactionDetailsLabel={t("TransactionDetails")}
        transactionDetailsValue={transactionDetails.transactionDetails}
        transactionStatusLabel={t("TransactionStatus")}
        transactionStatusValue={transactionDetails.transactionStatus}
        transactionAmountLabel={t("TransacitonAmount")}
        transactionAmount={transactionDetails.transactionAmount.total}
        transactionCurrency={transactionDetails.transactionAmount.currency}
        hideContent={handleOnTransactionDetailsClosed}
        buttonLabel={t("Close")}
      />
    );
  }

  return (
    <div className={classes.History}>
      <Modal
        show={accountDetails != null}
        modalClosed={handleOnAccountDetailsClosed}
      >
        {accountDetailsContent}
      </Modal>
      <Accounts
        accountsHeaderTitle={t("Accounts")}
        detailsButtonLabel={t("DetailsButtonLabel")}
        status={status}
        accounts={accounts}
        selectedAccount={selectedAccount}
        error={accountErrorTranslations}
        showTransactionDetails={handleOnShowAccountDetails}
        selectClickedAccount={handleOnSelectAccountClick}
      />
      <Modal
        show={transactionDetails != null}
        modalClosed={handleOnTransactionDetailsClosed}
      >
        {transactionDetailsContent}
      </Modal>
      <div className={classes.TransactionHistory}>
        <div className={classes.TransactionsHistoryHeader}>
          {t("TransactionsHistory")}
        </div>
        <div>
          <TransactionList
            transactions={transactions}
            status={transactionStatus}
            error={transactionErrorTranslations}
            noTransactionsMessage={t("NoneTransactionsForAccount")}
            showTransactionDetails={handleOnShowTransactionDetails}
          />
        </div>
        {transactions &&
        transactions.length !== 0 &&
        transactions.length % 10 === 0 ? (
          <div className={classes.LoadMoreWrapper}>
            <div
              className={classes.LoadMoreButton}
              onClick={handleOnLoadMoreClicked}
            >
              {t("LoadMore")}
            </div>
          </div>
        ) : null}
      </div>
    </div>
  );
};

export default History;
