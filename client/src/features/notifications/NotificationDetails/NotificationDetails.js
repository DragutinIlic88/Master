import React from "react";

import classes from "./NotificationDetails.module.css";
/**
 * Component for displaying notification details to the user.
 * Information which user can see are notification title, date and
 * time of creation, notification type, content and options to delete
 * notification or to close it.
 * @param {object} props
 */
const NotificationDetails = (props) => {
  const { title, date, time, type, content } = props.notification;

  return (
    <div className={classes.NotificationDetails}>
      <div className={classes.FirstRow}>
        <div className={classes.Title}>
          <div className={classes.TitleLabel}>{props.titleLabel}</div>
          <div className={classes.TitleValue}>{title}</div>
        </div>
        <div className={classes.Time}>
          <div className={classes.TimeLabel}>{props.timeLabel}</div>
          <div className={classes.TimeValue}>
            <span className={classes.TimeSpan}>{time}</span>
            <span className={classes.DateSpan}>{date}</span>
          </div>
        </div>
        <div className={classes.Type}>
          <div className={classes.TypeLabel}>{props.typeLabel}</div>
          <div className={classes.TypeValue}>{type}</div>
        </div>
      </div>
      <div className={classes.Content}>
        <div className={classes.ContentLabel}>{props.contentLabel}</div>
        <div className={classes.ContentValue}>{content}</div>
      </div>
      <div className={classes.NavigationWrapper}>
        <div className={classes.Button} onClick={props.handleCloseDetails}>
          {props.closeLabel}
        </div>
        <div className={classes.Button} onClick={props.handleDeleteMessage}>
          {props.deleteLabel}
        </div>
      </div>
    </div>
  );
};

export default NotificationDetails;
