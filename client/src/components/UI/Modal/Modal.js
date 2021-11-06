import React from "react";
import Backdrop from "../Backdrop/Backdrop";

import classes from "./Modal.module.css";

/**
 * Component can show some other component passed to it via props object as a modal.
 * It will be displayed if show prperty is true and it has clicked event.
 * @param {object} props
 */
const Modal = (props) => {
  return (
    <div>
      <Backdrop show={props.show} clicked={props.modalClosed} />
      <div
        className={classes.Modal}
        style={{
          transform: props.show ? "translateY(0)" : "translateY(-100vh)",
          opacity: props.show ? "1" : "0",
        }}
      >
        {props.children}
      </div>
    </div>
  );
};

export default Modal;
