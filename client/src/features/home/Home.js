import React, { useEffect, useState, useMemo } from "react";
import { useTranslation } from "react-i18next";
import { useSelector, useDispatch } from "react-redux";

import {
  selectAccounts,
  selectStatus,
  selectTransactionStatus,
  selectTransactions,
  selectTransactionError,
  selectAccountError,
  getAccounts,
  getTransactions,
} from "./homeSlice";
import { selectToken } from "../login/loginSlice";
import AccountSlider from "../../components/UI/AccountSlider/AccountSlider";
import TransactionList from "../../components/UI/TransactionList/TransactionList";
import { FETCH_DATA_STATUS } from "../../constants";

import classes from "./Home.module.css";
import { useErrorTranslations } from "../../useErrorTranslations";

/**
 * Component is rendered when user logs on application.
 * Component displays information about balances of  all accounts available to the user
 * and list of last five performed transactions.
 * @component
 */
const Home = () => {
  //#region init data
  const { t } = useTranslation();
  const accounts = useSelector(selectAccounts);
  const status = useSelector(selectStatus);
  const token = useSelector(selectToken);
  const transactionError = useSelector(selectTransactionError);
  const accountError = useSelector(selectAccountError);
  const transactions = useSelector(selectTransactions);
  const transactionStatus = useSelector(selectTransactionStatus);
  const dispatch = useDispatch();

  const [displayedAccount, setDisplayedAccount] = useState(null);

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

  //#region lifecycle methods
  /**
   * lifecycle methods dispatches informations to the redus so it can get
   * accounts and transactions performed by logged in user.
   */
  useEffect(() => {
    dispatch(
      getAccounts({
        userToken: token,
      })
    );

    dispatch(
      getTransactions({
        transactionsNumber: 5,
        userToken: token,
      })
    );
  }, [token, dispatch]);
  //#endregion

  //#region  event handlers
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
  //#endregion

  /**
   * First time when service is loaded displayed account will be set as first element of array.
   */
  if (status === FETCH_DATA_STATUS.SUCEEDED) {
    if (displayedAccount === null) {
      setDisplayedAccount(accounts[0]);
    }
  }

  return (
    <div className={classes.Home}>
      <AccountSlider
        status={status}
        displayedAccount={displayedAccount}
        accounts={accounts}
        handleOnLeftArrowClick={handleOnLeftArrowClick}
        handleOnRightArrowClick={handleOnRightArrowClick}
        leftArrowAlt={t("LeftArrow")}
        rightArrowAlt={t("RightArrow")}
        balanceHeaderTitle={t("Balance")}
        error={accountErrorTranslations}
      />
      <div className={classes.TransactionHistory}>
        <div className={classes.LastTransactionsHistoryHeader}>
          {t("LastTransactions")}
        </div>
        <div>
          <TransactionList
            transactions={transactions}
            status={transactionStatus}
            error={transactionErrorTranslations}
            noTransactionsMessage={t("NoneTransactions")}
          />
        </div>
      </div>
    </div>
  );
};

export default Home;
