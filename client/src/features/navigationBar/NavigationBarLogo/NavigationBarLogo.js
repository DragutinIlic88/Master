import React from "react";

import classes from "./NavigationBarLogo.module.css";
import Logo from "../../../assets/images/angel-wings-logo.png";

/**
 * Component is part of NavigationBar and is used for presentation of
 * companies logo and name. Name is passed through props object from
 * parrent component.
 * @component
 * @param {object} props
 */
const NavigationBarLogo = (props) => {
  return (
    <div className={classes.NavigationBarLogo}>
      <div className={classes.LogoImageDiv}>
        <img src={Logo} alt="Logo" className={classes.LogoImage} />
      </div>
      <div className={classes.LogoTitle}>{props.LogoTitle}</div>
    </div>
  );
};

export default NavigationBarLogo;
