import React from "react";

import { formElementPatternCheckerDecorator } from "../../../utility";
import Loader from "../../../components/UI/Loader/Loader";
import { FETCH_DATA_STATUS } from "../../../constants";

import classes from "./EditProfilePage.module.css";

/**
 * Component for editing some user information.
 * Component shows selected information of user in input, checks if
 * updated value is valid and has ability to cancel editing or to submit it.
 * @component
 * @param {object} props
 */
const EditProfilePage = (props) => {
  if (props.editStatus === FETCH_DATA_STATUS.LOADING) {
    return <Loader />;
  }

  return (
    <div className={classes.EditProfilePage}>
      <div className={classes.Header}>{props.title}</div>
      <div className={classes.Content}>
        <div className={classes.Label}>{props.labelText}</div>
        <div className={classes.InputWrapper}>
          <input
            value={props.inputValue}
            onChange={(event) => {
              formElementPatternCheckerDecorator(
                event,
                props.inputChanged,
                props.inputPattern
              );
            }}
            type={props.inputType}
            className={classes.Input}
          />
        </div>
        <div className={classes.Error}>
          {props.editErrorMessage ? props.editErrorMessage : null}
        </div>
        <div className={classes.ProceedDiv}>
          <div className={classes.Button} onClick={props.close}>
            {props.closeLabel}
          </div>
          <div className={classes.Button} onClick={props.change}>
            {props.changeLabel}
          </div>
        </div>
      </div>
    </div>
  );
};

export default EditProfilePage;
