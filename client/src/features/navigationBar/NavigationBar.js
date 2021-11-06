import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useHistory } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";

import {
  setActiveService,
  selectActiveService,
  getHelpInformation,
  selectHelpInfo,
  selectHelpInfoStatus,
} from "./navigationBarSlice";

import NavigationBarLogo from "./NavigationBarLogo/NavigationBarLogo";
import NavigationBarList from "./NavigationBarList/NavigationBarList";
import NavigationBarHelp from "./NavigationBarHelp/NavigationBarHelp";
import { SERVICES, SERVICES_PATH, FETCH_DATA_STATUS } from "../../constants";

import classes from "./NavigationBar.module.css";

/**
 * Component for showing companies logo , name ,
 * available services and help information
 * @component
 */
const NavigationBar = () => {
  const { t } = useTranslation();
  const history = useHistory();
  const dispatch = useDispatch();
  const activeService = useSelector(selectActiveService);
  const helpInfo = useSelector(selectHelpInfo);
  const helpInfoStatus = useSelector(selectHelpInfoStatus);
  const services = [
    { serviceName: t(SERVICES.HOME), serviceCode: SERVICES.HOME },
    { serviceName: t(SERVICES.HISTORY), serviceCode: SERVICES.HISTORY },
    { serviceName: t(SERVICES.EXCHANGE), serviceCode: SERVICES.EXCHANGE },
    { serviceName: t(SERVICES.PAYMENT), serviceCode: SERVICES.PAYMENT },
    { serviceName: t(SERVICES.LOAN), serviceCode: SERVICES.LOAN },
  ];

  /**
   * Event handler triggered when user clicks on some service.
   * Content of clicked service will be displayed and that service
   * will become actiive.
   * @event
   * @param {SERVICES} serviceCode
   */
  const handleOnServiceClicked = (serviceCode) => {
    dispatch(setActiveService(serviceCode));
    switch (serviceCode) {
      case SERVICES.HOME:
        history.push(SERVICES_PATH.HOME);
        break;
      case SERVICES.LOAN:
        history.push(SERVICES_PATH.LOAN);
        break;
      case SERVICES.PAYMENT:
        history.push(SERVICES_PATH.PAYMENT);
        break;
      case SERVICES.HISTORY:
        history.push(SERVICES_PATH.HISTORY);
        break;
      case SERVICES.EXCHANGE:
        history.push(SERVICES_PATH.EXCHANGE);
        break;
      default:
        history.push(SERVICES_PATH.HOME);
        break;
    }
  };

  /**
   *This lifecycle hook will be fired every time application is
   * loaded or user changes active service
   */
  useEffect(() => {
    const initPath = history.location.pathname;
    let initService = SERVICES.HOME;

    switch (initPath) {
      case SERVICES_PATH.HOME:
        initService = SERVICES.HOME;
        break;
      case SERVICES_PATH.LOAN:
      case SERVICES_PATH.NEW_LOAN:
        initService = SERVICES.LOAN;
        break;
      case SERVICES_PATH.PAYMENT:
        initService = SERVICES.PAYMENT;
        break;
      case SERVICES_PATH.HISTORY:
        initService = SERVICES.HISTORY;
        break;
      case SERVICES_PATH.EXCHANGE:
        initService = SERVICES.EXCHANGE;
        break;
      default:
        initService = SERVICES.HOME;
        break;
    }
    dispatch(setActiveService(initService));
  }, [dispatch, history.location.pathname]);

  /**
   * When page is loaded first time useEffect will try to get help information from server
   */
  useEffect(() => {
    dispatch(getHelpInformation());
  }, [dispatch]);

  let phonecontentText = t("PhoneContactInfo");
  let emailContentText = t("EmailContactInfo");
  if (helpInfoStatus === FETCH_DATA_STATUS.SUCEEDED) {
    phonecontentText += helpInfo.helpPhoneNumber;
    emailContentText += helpInfo.helpEmailAddress;
  }

  return (
    <div className={classes.NavigationBar}>
      <div className={classes.TopNavigation}>
        {" "}
        <NavigationBarLogo LogoTitle={t("OnlineBanking")} />
        <NavigationBarList
          activeService={activeService}
          handleOnClicked={handleOnServiceClicked}
          services={services}
        />
      </div>
      {helpInfoStatus !== FETCH_DATA_STATUS.FAILED ? (
        <div className={classes.BottomNavigation}>
          <NavigationBarHelp
            phoneContentText={phonecontentText}
            emailContentText={emailContentText}
          />
        </div>
      ) : null}
    </div>
  );
};

export default NavigationBar;
