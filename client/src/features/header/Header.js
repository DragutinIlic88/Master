import React, { useEffect, useState, useCallback } from "react";
import { toast } from "react-toastify";
import { useSelector, useDispatch } from "react-redux";
import { useTranslation } from "react-i18next";
import { useHistory } from "react-router-dom";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { selectActiveService } from "../navigationBar/navigationBarSlice";
import {
  selectNotifications,
  setNotifications,
} from "../notifications/notificationsSlice";
import { logout, selectToken, selectLogoutStatus } from "../login/loginSlice";

import {
  SERVICES,
  ACTION_TYPES,
  SERVICES_PATH,
  SIGNALR_URL,
  FETCH_DATA_STATUS,
} from "../../constants";
import classes from "./Header.module.css";
import userIcon from "../../assets/images/user-icon.png";
import signOutIcon from "../../assets/images/sign-out-header.png";
import letterIcon from "../../assets/images/letter-header.png";
import LoaderScreen from "../../components/UI/LoaderScreen/LoaderScreen";

/**
 * Component for displaying header information when user is logged in.
 * Component displayes name of the current service , notification, profile and
 * sign out options.
 * Component comunicates web socket server because of two way communication,
 *  so new notification can be sent while user is logged in. When componet is
 * rendered new socket chanell is open and this user is regestered to the socket
 * server. If there is new messages for that user web socket server will send it
 * to this component and notification counter will be increased.
 * @component
 */
const Header = () => {
  //#region  init data
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const history = useHistory();
  const activeService = useSelector(selectActiveService);
  const notifications = useSelector(selectNotifications);
  let unreadNotifications = null;
  const token = useSelector(selectToken);
  const logoutStatus = useSelector(selectLogoutStatus);
  const [connection, setConnection] = useState(null);
  //#endregion

  const registerServerMethods = useCallback(() => {
    connection.on("ReceiveNotification", (notification) => {
      dispatch(setNotifications([...notifications, notification]));
    });
    connection.on("ReceiveNotifications", (newNotifications) => {
      dispatch(setNotifications([...notifications, ...newNotifications]));
    });
  }, [connection, dispatch, notifications]);

  const sendUserTokenToHub = useCallback(async () => {
    if (connection.connectionStarted) {
      try {
        await connection.send("ReceivedUserToken", token);
        console.log("User token sent");
      } catch (e) {
        console.log(e);
      }
    } else {
      console.error("No connection to server yet.");
    }
  }, [connection, token]);

  const start = useCallback(async () => {
    try {
      await connection.start();
      console.log("SignalR Connected");
      registerServerMethods();
      await sendUserTokenToHub();
    } catch (err) {
      console.log(err);
      setTimeout(start, 60000);
    }
  }, [connection, registerServerMethods, sendUserTokenToHub]);

  if (connection != null) {
    connection.onclose(async () => {
      await start();
    });
  }

  //#region  lifecycle methods
  /**
   * Creating signalr connection when component is mounted,
   */
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(SIGNALR_URL)
      .withAutomaticReconnect()
      .build();
    setConnection(connection);
  }, []);

  /**
   * Startgin connection,
   * regestering listener handlers from server,
   * sending to server user token
   */
  useEffect(() => {
    if (connection) {
      start();
    }
  }, [connection, start]);
  //#endregion

  //#region  event handlers and helper methods
  /**
   * Event handler is triggered when user press sign out button
   * All app information will be reseted.
   */
  const onHandleSignOut = () => {
    dispatch(logout(token));
  };

  /**
   * Event handler is triggered when user clicks profile icon.
   * It will be navigated to profile service.
   */
  const onHandleProfileClick = () => {
    history.push(SERVICES_PATH.PROFILE);
  };

  /**
   * Event handler is triggered when user clicks notification icon.
   * It will be navigated to notifiaction service.
   */
  const onHandleNotificationClick = () => {
    history.push(SERVICES_PATH.NOTIFICATIONS);
  };
  //#endregion

  if (Array.isArray(notifications)) {
    unreadNotifications = notifications.filter(
      (notification) => notification.isRead === false
    );
  }

  if (logoutStatus === FETCH_DATA_STATUS.SUCEEDED) {
    dispatch({ type: ACTION_TYPES.RESET_STATE });
    history.push("/");
  } else if (logoutStatus === FETCH_DATA_STATUS.LOADING) {
  } else if (logoutStatus === FETCH_DATA_STATUS.FAILED) {
    toast.error(t("LogoutFailed"), { autoClose: 5000 });
  }

  return (
    <div className={classes.Header}>
      <div className={classes.ServiceTitle}>
        {activeService === SERVICES.HOME
          ? t("WelcomeMessage")
          : t(activeService)}
      </div>
      <div className={classes.HeaderInfoDiv}>
        <div
          className={classes.NotificationDiv}
          onClick={onHandleNotificationClick}
        >
          <img src={letterIcon} alt="Notifications" />
          {Array.isArray(unreadNotifications) &&
          unreadNotifications.length > 0 ? (
            <div className={classes.NotificationCounter}>
              {unreadNotifications.length}
            </div>
          ) : null}
        </div>
        <div className={classes.UserProfileDiv}>
          <img
            src={userIcon}
            alt="UserProfile"
            onClick={onHandleProfileClick}
          />
        </div>
        <div className={classes.SignOutDiv}>
          <img src={signOutIcon} alt="signOut" onClick={onHandleSignOut} />
        </div>
      </div>
      {logoutStatus === FETCH_DATA_STATUS.LOADING && <LoaderScreen />}
    </div>
  );
};

export default Header;
