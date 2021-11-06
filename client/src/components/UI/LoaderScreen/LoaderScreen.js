import React from "react";
import Loader from "../Loader/Loader";

import classes from "./LoaderScreen.module.css";

/**
 * Component will show loader as whole page
 * @param {object} props
 */
const LoaderScreen = (props) => {
  return (
    <div className={classes.LoaderScreen}>
      <Loader />
    </div>
  );
};

export default LoaderScreen;
