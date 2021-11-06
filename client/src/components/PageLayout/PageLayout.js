import React from "react";

import NavigationBar from "../../features/navigationBar/NavigationBar";
import Header from "../../features/header/Header";

import classes from "./PageLayout.module.css";
/**
 * Top level component containing Navigation bar , Header and service component.
 * Selected service is passed by props as children property.
 * @param {object} props
 */
const PageLayout = (props) => {
  return (
    <div className={classes.PageLayout}>
      <NavigationBar />
      <div className={classes.RightSideContent}>
        <Header />
        {props.children}
      </div>
    </div>
  );
};

export default PageLayout;
