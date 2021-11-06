import React from "react";

import classes from "./LoanDetails.module.css";

/**
 * Component for displaying details of selected loan.
 * All information are passed from parrent component via props object.
 * @component
 * @param {object} props
 */
const LoanDetails = (props) => {
  return (
    <div className={classes.LoanContent}>
      <div className={classes.Summary}>
        <div className={classes.FirstColumn}>
          <div className={classes.Name}>
            <div className={classes.Label}>{props.nameLabel}</div>
            <div className={classes.NameValue}>{props.nameValue}</div>
          </div>
          <div className={classes.AccountNumber}>
            <div className={classes.AccountLabel}>
              {props.fromAccountNumberLabel}
            </div>
            <div className={classes.AccountNumberValue}>
              {props.fromAccountNumberValue}
            </div>
          </div>
          <div className={classes.AccountNumber}>
            <div className={classes.AccountLabel}>
              {props.toAccountNumberLabel}
            </div>
            <div className={classes.AccountNumberValue}>
              {props.toAccountNumberValue}
            </div>
          </div>
          <div className={classes.Type}>
            <div className={classes.Label}>{props.typeLabel}</div>
            <div className={classes.TypeValue}>{props.typeValue}</div>
          </div>
          <div className={classes.CreationDate}>
            <div className={classes.Label}>{props.creationDateLabel}</div>
            <div className={classes.CreationDateValue}>
              {props.creationDateValue}
            </div>
          </div>
          <div className={classes.EndingDate}>
            <div className={classes.Label}>{props.endingDateLabel}</div>
            <div className={classes.EndingDateValue}>
              {props.endingDateValue}
            </div>
          </div>
        </div>
        <div className={classes.SecondColumn}>
          <div className={classes.Status}>
            <div className={classes.Label}>{props.statusLabel}</div>
            <div className={classes.StatusValue}>{props.statusValue}</div>
          </div>
          <div className={classes.TotalAmount}>
            <div className={classes.Label}>{props.totalAmountLabel}</div>
            <div className={classes.TotalAmountValue}>
              <span>{props.totalAmount}</span>
              <span>{props.currency}</span>
            </div>
          </div>
          <div className={classes.TotalAmount}>
            <div className={classes.Label}>{props.purposeLabel}</div>
            <div className={classes.TotalAmountValue}>
              <span>{props.purposeValue}</span>
            </div>
          </div>
          {props.participationAmount ? (
            <div className={classes.ParticipationAmount}>
              <div className={classes.Label}>
                {props.participationAmountLabel}
              </div>
              <div className={classes.ParticipationAmountValue}>
                <span>{props.participationAmount}</span>
                <span>{props.currency}</span>
              </div>
            </div>
          ) : null}
          {props.remainingAmount ? (
            <div className={classes.RemainingAmount}>
              <div className={classes.Label}>{props.remainingAmountLabel}</div>
              <div className={classes.RemainingAmountValue}>
                <span>{props.remainingAmount}</span>
                <span>{props.currency}</span>
              </div>
            </div>
          ) : null}
          {props.debtAmount ? (
            <div className={classes.DebtAmount}>
              <div className={classes.Label}>{props.debtAmountLabel}</div>
              <div className={classes.DebtAmountValue}>
                <span>{props.debtAmount}</span>
                <span>{props.currency}</span>
              </div>
            </div>
          ) : null}
          {props.paymentDeadlineDateValue && (
            <div className={classes.PaymentDeadlineDate}>
              <div className={classes.Label}>
                {props.paymentDeadlineDateLabel}
              </div>
              <div className={classes.PaymentDeadlineDateValue}>
                {props.paymentDeadlineDateValue}
              </div>
            </div>
          )}
        </div>
      </div>
      <div className={classes.PositionButton}>
        <div className={classes.HideContent} onClick={props.hideContent}>
          {props.buttonLabel}
        </div>
      </div>
    </div>
  );
};

export default LoanDetails;
