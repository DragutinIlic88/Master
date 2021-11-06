import React from "react";

import classes from "./InsertOtpLayout.module.css";
import SubmitButton from "../../../../components/UI/Buttons/SubmitButton/SubmitButton";

/**
 * Generic component for displaying input field for inserting otp and submit button.
 * All options of input and button are passed via props object.
 * @param {object} props
 */
const InsertOtpLayout = (props) => {
  return (
    <div
      className={
        props.animate
          ? [classes.CenteredContent, classes.AnimateFromRight].join(" ")
          : [classes.CenteredContent, classes.AnimateFromLeft].join(" ")
      }
    >
      <div className={classes.InsertOtp}>{props.insertOtpLabel}</div>
      {props.error !== "" ? (
        <div className={classes.ErrorToken}>{props.invalidTokenMessage}</div>
      ) : null}
      <div className={classes.InsertOTPDiv}>
        <input
          className={classes.InsertOTPInput}
          type="text"
          maxLength={props.inputLength}
          placeholder={props.placeholder}
          value={props.otp}
          onChange={props.onValueChange}
        />
      </div>
      <div className={classes.SubmitWrapper}>
        <SubmitButton
          value={props.submitLabel}
          enabledSubmit={props.condition}
          clicked={props.handleOnClick}
        />
      </div>
    </div>
  );
};

export default InsertOtpLayout;
