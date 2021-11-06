import React, { useState } from "react";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";
import axios from "axios";

import ConfirmButton from "./SignUpConfirmationButton/SignUpConfirmationButton";
import InsertOtpLayout from "./InsertOtpLayout/InsertOtpLayout";
import Title from "../../../components/UI/Title/Title";
import { API_URL, ENDPOINT_PATHS } from "../../../constants";

import classes from "./SignUpConfirmation.module.css";

/**
 * Component for displaying verification fields after user trys to sign up to the application.
 * First user can choose verification method which can be via sms or email. After user choose
 * verification method and confirmation arives from server , component will display otp input
 * filed.User can type otp which will arive on email or phone and press submit button.
 */
const SignUpConfirmation = () => {
  //#region init data
  const { t } = useTranslation();
  const [type, setType] = useState("");
  const [otp, setOtp] = useState("");
  const [animate, setAnimate] = useState(false);
  const [error, setError] = useState("");
  const history = useHistory();

  let content = (
    <div
      className={[classes.CenteredContent, classes.AnimateFromLeft].join(" ")}
    >
      <div className={classes.Label}>{t("ChoosVerificationMethod")}</div>
      {error !== "" ? (
        <div className={classes.ErrorToken}>{t("TokenError")}</div>
      ) : null}
      <div className={classes.ButtonsPostion}>
        <ConfirmButton
          text={t("SMS")}
          clicked={() => {
            handleOnVerificationClicked("SMS");
          }}
        />
        <ConfirmButton
          text={t("EMAIL")}
          clicked={() => {
            handleOnVerificationClicked("EMAIL");
          }}
        />
      </div>
    </div>
  );
  //#endregion

  //#region  event handlers and helper methods
  /**
   * Event handler is triggered when user chooses verification method.
   * Choosen type will be posted to the server with help of axios library.
   * If successful response arrives type is set and animation for new display
   * begins. Otherwise error is displayed.
   * @param {string} type string which can be SMS or EMAIL
   */
  const handleOnVerificationClicked = (type) => {
    axios
      .post(API_URL + ENDPOINT_PATHS.SIGNUP_VERIFICATION, {
        verificationType: type,
      })
      .then((res) => {
        setType(type);
        setAnimate(true);
        setError("");
      })
      .catch((err) => {
        setError(err.response.data.errorMessage);
      });
  };

  /**
   * Event handler is triggered when user clicks submit button
   * after it inserts otp. Otp is sent to the server and if it
   * pass user is navigated to the home page.Otherwise error message
   * is displayed.
   */
  const handleOnProceedClicked = () => {
    axios
      .post(API_URL + "/signUp/verifyToken", {
        otp: otp,
      })
      .then((res) => {
        history.push("/home");
      })
      .catch((err) => {
        setError(err.response.data.errorMessage);
      });
  };
  //#endregion

  /**
   * Based on which type of verification user chooses
   * proper content will be displayed. For displaying
   * filed where user can type otp InsertOtpLayout custom
   * component is used.
   */
  if (type === "SMS") {
    content = (
      <InsertOtpLayout
        animate={animate}
        insertOtpLabel={t("InsertOtp")}
        error={error}
        invalidTokenMessage={t("InvalidToken")}
        inputLength="5"
        placeholder="*****"
        otp={otp}
        onValueChange={(e) => {
          setOtp(e.target.value);
        }}
        submitLabel={t("Proceed")}
        condition={otp.length === 5}
        handleOnClick={() => {
          handleOnProceedClicked();
        }}
      />
    );
  } else if (type === "EMAIL") {
    content = (
      <InsertOtpLayout
        animate={animate}
        insertOtpLabel={t("InsertEmailOtp")}
        error={error}
        invalidTokenMessage={t("InvalidToken")}
        inputLength="5"
        placeholder="*****"
        otp={otp}
        onValueChange={(e) => {
          setOtp(e.target.value);
        }}
        submitLabel={t("Proceed")}
        condition={otp.length === 5}
        handleOnClick={() => {
          handleOnProceedClicked();
        }}
      />
    );
  }

  return (
    <div className={classes.BackgroundContainer}>
      <div
        className={
          animate
            ? [classes.TitleWrapper, classes.MoveFromTop].join(" ")
            : classes.TitleWrapper
        }
      >
        <Title titleName={t("OnlineBanking")} />
      </div>
      {content}
    </div>
  );
};

export default SignUpConfirmation;
