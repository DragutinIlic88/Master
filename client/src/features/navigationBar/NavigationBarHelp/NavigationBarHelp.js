import React, { useState } from "react";

import InfoSegment from "../../../components/UI/InfoSegment/InfoSegment";

import classes from "./NavigationBarHelp.module.css";
import phone from "../../../assets/images/phone-help.png";
import letter from "../../../assets/images/letter-help.png";

/**
 * Component is displayed as part of NavigationBar component
 * and provides user with phone and email which can be contacted
 * in case that user needs help
 * @component
 * @param {object} props
 */
const NavigationBarHelp = (props) => {
  const [showContactInfo, setShowContactInfo] = useState(false);
  const [showEmailInfo, setShowEmailInfo] = useState(false);

  /**
   * Event handler is triggered when user clicks on phone help button
   * @event
   */
  const handleOnContactClick = () => {
    setShowContactInfo(true);
  };

  /**
   * Event handler is triggered when user clicks on email help button
   * @event
   */
  const handleOnLetterClick = () => {
    setShowEmailInfo(true);
  };

  return (
    <div className={classes.NavigationBarHelp}>
      <div className={classes.Phone}>
        {" "}
        <img src={phone} alt="Contact" onClick={handleOnContactClick} />
        <div className={classes.InfoSegmentWrapper}>
          {showContactInfo ? (
            <InfoSegment
              content={props.phoneContentText}
              clickedOutside={() => {
                setShowContactInfo(false);
              }}
            />
          ) : null}
        </div>
      </div>
      <div className={classes.Letter}>
        <img src={letter} alt="Email" onClick={handleOnLetterClick} />
        <div className={classes.EmailInfoSegmentWrapper}>
          {showEmailInfo ? (
            <InfoSegment
              content={props.emailContentText}
              clickedOutside={() => {
                setShowEmailInfo(false);
              }}
            />
          ) : null}
        </div>
      </div>
    </div>
  );
};

export default NavigationBarHelp;
