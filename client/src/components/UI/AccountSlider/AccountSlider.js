import React from "react";
import Loader from "../Loader/Loader";
import { FETCH_DATA_STATUS } from "../../../constants";

import classes from "./AccountSlider.module.css";
import LeftArrow from "../../../assets/images/resize-left-arrow.png";
import RightArrow from "../../../assets/images/resize-right-arrow.png";

/**
 * Helper function used for formating amount in form suitable
 * for displaying. Adds commas after every three
 * digits and divides integral and decimal part with dot sign.
 * @param {string} amount string whch represents amount (eg 120000.56)
 * @returns amount as string in suitable format (eg. 120,000.56)
 */
const formatAmount = (amount) => {
  let amountString = amount.toString();
  let integerAndDecimalPart = amountString.split(".");
  let amountArray = integerAndDecimalPart[0].split("");
  let formatedArray = [];
  for (let i = amountArray.length - 1, j = 1; i >= 0; i--, j++) {
    if (j > 0 && j % 3 === 0 && j !== amountArray.length) {
      formatedArray.unshift("," + amountArray[i]);
    } else {
      formatedArray.unshift(amountArray[i]);
    }
  }

  if (integerAndDecimalPart.length > 1) formatedArray.push(".");
  formatedArray.push(integerAndDecimalPart[1]);

  return formatedArray.join("");
};

/**
 * Helper function which formats account string passed to it as parameter.
 * It adds - sign after every four digits.
 * @param {string} account string which represents account (112233445566778899)
 * @returns account as string in suitable format (eg. 1122-3344-5566-7788-99)
 */
const formatAccount = (account) => {
  let accountArray = account.split("");
  let formatedArray = [];
  for (let i = 0; i <= accountArray.length - 1; i++) {
    if ((i + 1) % 4 === 0) {
      formatedArray.push(accountArray[i] + "-");
    } else {
      formatedArray.push(accountArray[i]);
    }
  }

  return formatedArray.join("");
};

/**
 * Component for displaying all accounts passed to it by props object as slider.
 * Component first check status parameter of props object and if it has LOADIN value
 * it displayes Loader component. If status is failed , error parameter from props is
 * displayed as message in component. Otherwise acount slider is made based on the number
 * of length of accounts array parameter. If length of accounts is larger than one than
 * arrows are displayed as part of component so user can navigate to other accounts.
 * Component displayes amount , currency and account number in prpoer formats.
 * @param {object} props
 */
const AccountSlider = (props) => {
  let accountSliderContent = (
    <div className={classes.AccountSliderContent}>
      <Loader />
    </div>
  );

  if (props.status === FETCH_DATA_STATUS.LOADING) {
    accountSliderContent = (
      <div className={classes.AccountSliderContent}>
        <Loader />
      </div>
    );
  } else if (props.status === FETCH_DATA_STATUS.SUCEEDED) {
    accountSliderContent = (
      <div className={classes.AccountSliderContent}>
        <div className={classes.CurrencyAmount}>
          <div className={classes.Amount}>
            {formatAmount(props.displayedAccount.amount)}
          </div>
          <div className={classes.Currency}>
            {props.displayedAccount.currency.toUpperCase()}
          </div>
        </div>

        <div className={classes.AccountSlider}>
          {props.accounts.length > 1 ? (
            <div className={classes.LeftArrow}>
              <img
                src={LeftArrow}
                alt={props.leftArrowAlt}
                onClick={props.handleOnLeftArrowClick}
              />
            </div>
          ) : null}

          <div className={classes.Account}>
            {formatAccount(props.displayedAccount.accountNumber)}
          </div>
          {props.accounts.length > 1 ? (
            <div className={classes.RightArrow}>
              <img
                src={RightArrow}
                alt={props.rightArrowAlt}
                onClick={props.handleOnRightArrowClick}
              />
            </div>
          ) : null}
        </div>
      </div>
    );
  } else if (props.status === FETCH_DATA_STATUS.FAILED) {
    accountSliderContent = (
      <div className={classes.AccountSliderContentError}>{props.error}</div>
    );
  }

  return (
    <div className={classes.AccountSliderWrapper}>
      <div className={classes.AccountSliderHeader}>
        {" "}
        {props.balanceHeaderTitle}
      </div>
      {accountSliderContent}
    </div>
  );
};

export default AccountSlider;
