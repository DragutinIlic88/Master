import React from "react";
import NavigationBarListItem from "./NavigationBarListItem/NavigationBarListItem";
import classes from "./NavigationBarList.module.css";

/**
 * Component is part of NavigationBar component
 * and it is used for creating list of active services.
 * NavigationBarList component creates a list of
 * NavigationBarListItem components and passes service
 * information to every item
 * @component
 * @param {object} props
 */
const NavigationBarList = (props) => {
  const items = props.services.map((service) => (
    <NavigationBarListItem
      isActive={props.activeService === service.serviceCode}
      listItemName={service.serviceName}
      key={service.serviceCode}
      clicked={() => {
        props.handleOnClicked(service.serviceCode);
      }}
    />
  ));
  return <ul className={classes.NavigationBarList}>{items}</ul>;
};

export default NavigationBarList;
