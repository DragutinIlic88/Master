import React, { useState, useEffect, useMemo } from "react";
import { ToastContainer, toast } from "react-toastify";
import { useTranslation } from "react-i18next";
import { Redirect } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";

import {
  auth,
  signUp,
  selectStatus,
  selectError,
  selectSignUp,
  signUpSwitched,
  selectRegistrationStatus,
  setRegistrationStatus,
  setLoginStatus,
  setError,
} from "./loginSlice";

import LanguageList from "../../components/UI/LanguageList/LanguageList";
import Title from "../../components/UI/Title/Title";
import Input from "../../components/UI/Input/Input";
import SubmitButton from "../../components/UI/Buttons/SubmitButton/SubmitButton";
import Loader from "../../components/UI/Loader/Loader";
import SignUpSwitcher from "./SignUpSwitcher/SignUpSwitcher";

import {
  emailPattern,
  passwordPattern,
  bankIdPattern,
  numberPattern,
  mobilePattern,
  mobileSignsOnly,
  isValueValid,
} from "../../utility";

import { FETCH_DATA_STATUS } from "../../constants";

import classes from "./Login.module.css";
import "react-toastify/dist/ReactToastify.css";

/**
 * Component for renderning login and sign up page. This is the first component which is
 * rendered when user navigate to this application. It has options for changing language
 * on which application will work , for logging of user and for signing up new user.
 * User will be presented with log in fields (username and password) and button which is
 * switcher between login and sign up page. If user choose that it wants to sign up, component
 * will display form with sign up fileds.
 * @component
 */
const Login = () => {
  //#region init data
  const { t, i18n } = useTranslation();
  const languages = Object.keys(i18n.services.resourceStore.data);
  const dispatch = useDispatch();
  const loginStatus = useSelector(selectStatus);
  const registrationStatus = useSelector(selectRegistrationStatus);
  const errorMessage = useSelector(selectError);
  const isSignUpPage = useSelector(selectSignUp);
  const [emailValue, setEmailValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");
  const [enableSubmit, setEnableSubmit] = useState(false);
  const [bankIdValue, setBankIdValue] = useState("");
  const [mobileNumberValue, setMobileNumber] = useState("");
  const [userNameValue, setUserNameValue] = useState("");
  const [addressValue, setAddressValue] = useState("");
  const [homePhoneValue, setHomePhoneValue] = useState("");
  const [confirmPasswordValue, setConfirmPasswordValue] = useState("");
  //#endregion

  //#region event handlers and helper methods

  const errorTranslations = useMemo(() => {
    if (typeof errorMessage === "string") {
      const errorLabels = errorMessage.split("\n");
      if (errorLabels.length > 1) {
        const errorTranslations = errorLabels.map((l) => t(l)).join("\n");
        return errorTranslations;
      }
      return t(errorMessage);
    }
    return "";
  }, [errorMessage, t]);

  /**
   * Event handler triggered when user press button for switching language
   * @param {string} lang
   */
  const handleLanguageChange = (lang) => {
    if (i18n.language !== lang) i18n.changeLanguage(lang);
  };

  /**
   *
   * Event handler triggered when user wants to login or sign up.
   * Information in infoObject will be dispatched to the redux which
   * will perform request to the server.
   * @param {object} infoObject
   * @param {string} status
   * @param {boolean} isSignUpPage
   */
  const handleSubmitClick = (infoObject, isSignUpPage) => {
    if (!isSignUpPage) {
      dispatch(
        auth({ email: infoObject.email, password: infoObject.password })
      );
    } else {
      dispatch(signUp(infoObject));
    }
  };

  /**
   * Event handler triggered when user press login/signup switch button.
   * Information is dispatched to the redux and proper form is presented to
   * the user.
   */
  const handleToggleSignUp = () => {
    if (isSignUpPage) {
      setEmailValue("");
      setPasswordValue("");
      setConfirmPasswordValue("");
      setBankIdValue("");
      setMobileNumber("");
      setUserNameValue("");
      setAddressValue("");
      setHomePhoneValue("");
    } else {
      setEmailValue("");
      setPasswordValue("");
    }

    dispatch(setError(null));
    dispatch(signUpSwitched({ signUp: !isSignUpPage }));
  };
  //#endregion

  //#region  lifecycle methods
  /**
   * Lifecycle method is called every time when some variable from array passed as
   * second argument is changed. If inserted vaules for loging/signing up are valid
   * submit button will be shown to the user.
   */
  useEffect(() => {
    // for every parameter
    if (!isSignUpPage) {
      setEnableSubmit(
        isValueValid(emailValue, emailPattern) &&
          isValueValid(passwordValue, passwordPattern)
      );
    } else {
      setEnableSubmit(
        isValueValid(bankIdValue, bankIdPattern) &&
          isValueValid(mobileNumberValue, mobilePattern) &&
          isValueValid(emailValue, emailPattern) &&
          isValueValid(userNameValue, ".+") &&
          isValueValid(passwordValue, passwordPattern) &&
          isValueValid(confirmPasswordValue, passwordPattern)
      );
    }
  }, [
    isSignUpPage,
    emailValue,
    passwordValue,
    bankIdValue,
    mobileNumberValue,
    userNameValue,
    confirmPasswordValue,
  ]);
  //#endregion

  /**
   * In case that user just wants to log in to application
   * content will have logic for language switching , email
   * input , password input and sign up switcher button
   */
  let content = (
    <div className={classes.LoginInfo}>
      <div className={classes.LanguageListPosition}>
        <LanguageList
          className={classes.LanguageListPosition}
          languages={languages}
          labelText={t("SwitchLanguage")}
          changeLanguage={handleLanguageChange}
        />
      </div>

      <div className={classes.EmailWrapper}>
        <Input
          inputName="email"
          inputId="eml"
          isRequired={true}
          inputType="email"
          spanText={t("EmailAddressInput")}
          inputValue={emailValue}
          onValueChange={(value) => {
            setEmailValue(value);
          }}
          inputPattern={emailPattern}
        />
      </div>
      <div className={classes.PasswordWrapper}>
        <Input
          inputName="password"
          inputId="pwd"
          isRequired={true}
          inputType="password"
          spanText={t("PasswordInput")}
          inputPattern={passwordPattern}
          inputValue={passwordValue}
          onValueChange={(value) => {
            setPasswordValue(value);
          }}
        />
      </div>
      {errorMessage !== null ? (
        <div className={classes.Error}>{errorTranslations}!</div>
      ) : null}
      <div className={classes.SubmitWrapper}>
        <SubmitButton
          value={t("Login")}
          enabledSubmit={enableSubmit}
          clicked={() => {
            handleSubmitClick(
              { email: emailValue, password: passwordValue },
              isSignUpPage
            );
          }}
        />
      </div>
      <div className={classes.SignUpSwitcherWrapper}>
        <SignUpSwitcher
          clicked={handleToggleSignUp}
          labelText={t("SignUpLabel")}
          linkText={t("SignUp")}
        />
      </div>
    </div>
  );

  /**
   * If user press signUp switch button
   * content will became form for signing up
   * which has language selection element,
   * inputs for email, mobile, acount or
   * credit card number, username, address,
   * phone , password and confirmation password
   */
  if (isSignUpPage) {
    content = (
      <div className={classes.SignUpInfo}>
        <div className={classes.LanguageListPosition}>
          <LanguageList
            className={classes.LanguageListPosition}
            languages={languages}
            labelText={t("SwitchLanguage")}
            changeLanguage={handleLanguageChange}
          />
        </div>
        <div className={classes.TwoRows}>
          <div className={classes.FirstRow}>
            <div className={classes.EmailWrapper}>
              <Input
                inputName="bankId"
                inputId="bnkId"
                isRequired={true}
                pattern={numberPattern}
                inputType="text"
                spanText={t("AccountCard")}
                inputValue={bankIdValue}
                onValueChange={(value) => {
                  setBankIdValue(value);
                }}
                inputPattern={bankIdPattern}
              />
            </div>
            <div className={classes.EmailWrapper}>
              <Input
                inputName="mobile"
                inputId="mob"
                isRequired={true}
                pattern={mobileSignsOnly}
                inputType="text"
                spanText={t("MobileNumber")}
                inputValue={mobileNumberValue}
                onValueChange={(value) => {
                  setMobileNumber(value);
                }}
                inputPattern={mobilePattern}
              />
            </div>
            <div className={classes.EmailWrapper}>
              <Input
                inputName="email"
                inputId="eml"
                isRequired={true}
                inputType="email"
                spanText={t("EmailAddressInput")}
                inputValue={emailValue}
                onValueChange={(value) => {
                  setEmailValue(value);
                }}
                inputPattern={emailPattern}
              />
            </div>
            <div className={classes.EmailWrapper}>
              <Input
                inputName="userName"
                inputId="usName"
                isRequired={true}
                inputType="text"
                spanText={t("UserName")}
                inputValue={userNameValue}
                onValueChange={(value) => {
                  setUserNameValue(value);
                }}
              />
            </div>
            <div className={classes.EmailWrapper}>
              <Input
                inputName="address"
                inputId="adr"
                inputType="text"
                spanText={t("Address")}
                inputValue={addressValue}
                onValueChange={(value) => {
                  setAddressValue(value);
                }}
              />
            </div>
          </div>
          <div className={classes.SecondRow}>
            <div className={classes.EmailWrapper}>
              <Input
                inputName="homePhone"
                inputId="hPhone"
                pattern={mobileSignsOnly}
                inputType="text"
                spanText={t("HomePhone")}
                inputValue={homePhoneValue}
                onValueChange={(value) => {
                  setHomePhoneValue(value);
                }}
              />
            </div>
            <div className={classes.PasswordWrapper}>
              <Input
                inputName="password"
                inputId="pwd"
                isRequired={true}
                inputType="password"
                spanText={t("PasswordInput")}
                inputPattern={passwordPattern}
                inputValue={passwordValue}
                onValueChange={(value) => {
                  setPasswordValue(value);
                }}
              />
            </div>
            <div className={classes.PasswordWrapper}>
              <Input
                inputName="confirmPassword"
                inputId="cpwd"
                isRequired={true}
                inputType="password"
                spanText={t("ConfirmPasswordInput")}
                inputPattern={passwordPattern}
                inputValue={confirmPasswordValue}
                onValueChange={(value) => {
                  setConfirmPasswordValue(value);
                }}
              />
            </div>
            <div className={classes.SubmitWrapper}>
              <SubmitButton
                value={t("SignUpSubmit")}
                enabledSubmit={enableSubmit}
                clicked={() => {
                  handleSubmitClick(
                    {
                      email: emailValue,
                      password: passwordValue,
                      confirmPassword: confirmPasswordValue,
                      bankId: bankIdValue,
                      mobile: mobileNumberValue,
                      userName: userNameValue,
                      address: addressValue,
                      homePhone: homePhoneValue,
                    },
                    isSignUpPage
                  );
                }}
              />
            </div>
            {errorMessage !== null ? (
              <div className={classes.SignUpError}>{errorTranslations}!</div>
            ) : null}
          </div>
        </div>
        <div className={classes.SignInSwitcherWrapper}>
          <SignUpSwitcher
            clicked={handleToggleSignUp}
            labelText={t("SignInLabel")}
            linkText={t("Login")}
          />
        </div>
      </div>
    );
  }

  /**
   * When user trys to login/signup there is period of waiting
   * for response from server. For that time Loader component will
   * be displayed. When response arrives and if it has success status
   * user will be redirected to the next page.
   */
  if (
    loginStatus === FETCH_DATA_STATUS.LOADING ||
    registrationStatus === FETCH_DATA_STATUS.LOADING
  ) {
    content = (
      <div className={classes.LoaderWrapper}>
        <Loader />
      </div>
    );
  } else if (loginStatus === FETCH_DATA_STATUS.FAILED) {
    toast.error(t("LoginFailed"), {
      autoClose: 5000,
    });
    dispatch(setLoginStatus(FETCH_DATA_STATUS.IDLE));
  } else if (registrationStatus === FETCH_DATA_STATUS.FAILED) {
    toast.error(t("SignUpFailed"), {
      autoClose: 5000,
    });
    dispatch(setRegistrationStatus(FETCH_DATA_STATUS.IDLE));
  } else if (loginStatus === FETCH_DATA_STATUS.SUCEEDED) {
    content = <Redirect push to="/home" />;
  } else if (registrationStatus === FETCH_DATA_STATUS.SUCEEDED) {
    toast.success(t("SignUpSuccess"), { autoClose: 5000 });
    dispatch(signUpSwitched({ signUp: !isSignUpPage }));
    dispatch(setRegistrationStatus(FETCH_DATA_STATUS.IDLE));
  }

  return (
    <div className={classes.LoginContent}>
      <div className={classes.TitleWrapper}>
        <Title titleName={t("OnlineBanking")} />
      </div>
      {content}
      <ToastContainer />
    </div>
  );
};

export default Login;
