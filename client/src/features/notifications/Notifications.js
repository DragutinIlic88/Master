import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch, useSelector } from "react-redux";

import Modal from "../../components/UI/Modal/Modal";
import Loader from "../../components/UI/Loader/Loader";
import NotificationDetails from "./NotificationDetails/NotificationDetails";
import { FETCH_DATA_STATUS } from "../../constants";

import {
  selectNotifications,
  selectNotificationStatus,
  selectNotificationIsReadStatus,
  selectNotificationIsDeletedStatus,
  deleteNotification,
  notificationIsRead,
  getNotifications,
  setNotificationIsReadStatus,
  setNotificationIsDeleteStatus,
} from "./notificationsSlice";

import { selectToken } from "../login/loginSlice";

import classes from "./Notifications.module.css";

/**
 * Component for rendering notification service to the logged in user.
 * User can see all notifications received as a list of messages. All unread
 * notificatons will be marked.User can open NotificationDetails component and
 * check and delete every message.
 *@component
 */
const Notifications = () => {
  //#region init data
  const [messageDetails, setMessageDetails] = useState(null);
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const notifications = useSelector(selectNotifications);
  const notificationStatus = useSelector(selectNotificationStatus);
  const token = useSelector(selectToken);
  const notificationIsDeltedStatus = useSelector(
    selectNotificationIsDeletedStatus
  );
  const notificationIsReadStatus = useSelector(selectNotificationIsReadStatus);
  let listItems = null;
  //#endregion

  //#region  event handlers and helper methods
  /**
   * Event handler is triggered when user clicks on some message.
   * Clicked message is set for displaying it details.
   * @param {object} message
   */
  const onHandleMessageClicked = (message) => {
    setMessageDetails(message);
  };

  /**
   * Event handler is triggered when user click on delete button inside
   * notification details component. Task is dispatched to the redux which
   * contact server and message details page is closed.
   */
  const onHandleDeleteMessage = () => {
    dispatch(deleteNotification(messageDetails));
    setMessageDetails(null);
  };

  /**
   * Event handler is triggered when user wants to close message details page.
   * If message was not read previously now it is set as read and info is dispatched
   * to the redux. Details page is closed.
   */
  const onHandleMessageClosed = () => {
    if (messageDetails.isRead === false) {
      dispatch(notificationIsRead(messageDetails));
    }
    setMessageDetails(null);
  };
  //#endregion

  useEffect(() => {
    dispatch(getNotifications(token));
  }, [token, dispatch]);

  if (notificationIsReadStatus === FETCH_DATA_STATUS.SUCEEDED) {
    dispatch(getNotifications(token));
    dispatch(setNotificationIsReadStatus(FETCH_DATA_STATUS.IDLE));
  }

  if (notificationIsDeltedStatus === FETCH_DATA_STATUS.SUCEEDED) {
    dispatch(getNotifications(token));
    dispatch(setNotificationIsDeleteStatus(FETCH_DATA_STATUS.IDLE));
  }

  /**
   * While notifiactions are retreived from server Loader is displayed
   */
  if (
    notificationStatus === FETCH_DATA_STATUS.LOADING ||
    notificationIsDeltedStatus === FETCH_DATA_STATUS.LOADING ||
    notificationIsReadStatus === FETCH_DATA_STATUS.LOADING
  ) {
    return (
      <div className={classes.Notifications}>
        <Loader />
      </div>
    );
  }

  /**
   * list of notifications is created and displayed to the screen as a list.
   * Every element of list has title, time and date presented to the user and it
   * is clickable.
   */
  if (notifications) {
    if (notifications.length > 0) {
      listItems = notifications.map((message) => {
        let classNames = classes.Message;
        classNames += message.isRead ? "" : " " + classes.UnreadMessage;
        return (
          <li
            key={message.messageId}
            className={classNames}
            onClick={() => onHandleMessageClicked(message)}
          >
            <span className={classes.MessgeTitle}>{message.title}</span>
            <span className={classes.MessageDateTime}>
              <span className={classes.MessageTime}>{message.time}</span>
              <span className={classes.MessageData}>{message.date}</span>
            </span>
          </li>
        );
      });
    } else {
      return (
        <div className={classes.Notifications}>
          <div className={classes.NoMessage}>{t("NoMessges")}</div>
        </div>
      );
    }
  }

  return (
    <div className={classes.Notifications}>
      <Modal show={messageDetails !== null} modalClosed={onHandleMessageClosed}>
        {messageDetails !== null ? (
          <NotificationDetails
            notification={messageDetails}
            titleLabel={t("MessageTitle")}
            timeLabel={t("MessageTime")}
            typeLabel={t("MessageType")}
            contentLabel={t("MessageContent")}
            closeLabel={t("MessageClose")}
            deleteLabel={t("MessageDelete")}
            handleCloseDetails={onHandleMessageClosed}
            handleDeleteMessage={onHandleDeleteMessage}
          />
        ) : null}
      </Modal>
      <ul className={classes.Messages}>{listItems}</ul>
    </div>
  );
};

export default Notifications;
