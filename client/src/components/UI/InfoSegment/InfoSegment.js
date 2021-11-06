import React, { useEffect, useRef } from "react";

import classes from "./InfoSegment.module.css";

/**
 * Component is used for displaying help informations.
 * When user clicks on contact or send email for help
 * this component will be shown. If user clicks anywhere
 * outside component , component will be closed.
 * @param {object} props
 */
const InfoSegment = (props) => {
  const wrapperRef = useRef(null);

  /**
   * Lifecycle method will be called every time user press mouse button.
   * In that case handleClickedOutside is triggered which checks if user clicked
   * outside of this component and if so clickedOutside method is called.
   */
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target)) {
        props.clickedOutside();
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [wrapperRef, props]);

  return (
    <div ref={wrapperRef} className={classes.InfoSegment}>
      <div className={classes.Content}>{props.content}</div>
      <div className={classes.Triangle}></div>
    </div>
  );
};

export default InfoSegment;
