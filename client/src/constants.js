/** @module Constants */

/**Base url of endpoint api */
//export const API_URL = "http://localhost:9000/api";
export const API_URL = "https://localhost:5001/api";

/**Base url of web socket api */
export const WEB_SOCKET_URL = "ws://localhost:8000";

export const SIGNALR_URL = "https://localhost:5001/hubs/notificationHub";

/**
 * Paths of web socket api
 */
export const ENDPOINT_PATHS = {
  AUTHENTICATE: "/login/auth",
  REGISTER: "/login/signUp",
  LOGOUT: "/login/logout",
  SIGNUP_VERIFICATION: "/login/verify",
  GET_USER_PROFILE: "/profile/getUserProfile",
  EDIT_PROFILE: "/profile/editUser",
  EDIT_PROFILE_LOGO: "/profile/editLogo",
  CURRENCIES: "/currencies/getCurrencies",
  FEES: "/fees/getFeeInfo",
  EXCHANGE: "/exchange/confirmExchange",
  TRANSACTIONS: "/transactions/getTransactions",
  ACCOUNTS: "/accounts/getAccounts",
  LOANS: "/loans",
  HELP: "/help",
  PAYMENT: "/payment/performPayment",
  NOTIFICATIONS: "/notifications",
  GET_NOTIFICATIONS: "/notifications/getNotifications",
};

/**
 * Enum for status of fetching data
 * @readonly
 * @enum
 * */
export const FETCH_DATA_STATUS = {
  IDLE: "idle",
  LOADING: "loading",
  SUCEEDED: "suceeded",
  FAILED: "failed",
};

/**
 * Enum for all services inside application
 * @readonly
 * @enum
 * */
export const SERVICES = {
  HOME: "Home",
  HISTORY: "History",
  EXCHANGE: "Exchange",
  PAYMENT: "Payment",
  LOAN: "Loan",
  PROFILE: "Profile",
};

/**
 * Enum for all paths inside application
 * @readonly
 * @enum
 * */
export const SERVICES_PATH = {
  HOME: "/home",
  HISTORY: "/history",
  EXCHANGE: "/exchange",
  PAYMENT: "/payment",
  LOAN: "/loan",
  NEW_LOAN: "/loan/new",
  PROFILE: "/profile",
  NOTIFICATIONS: "/notifications",
};

export const ACTION_TYPES = {
  RESET_STATE: "store/reset",
};

/**
 * Enum for editable fields inside profile service
 * @readonly
 * @enum
 * */
export const PROFILE_INFO_TYPES = {
  MOBILE: "mobile",
  EMAIL: "email",
  ADDRESS: "address",
  USER_NAME: "userName",
};

/**
 * Enum for account type values
 * @readonly
 * @enum
 * */
export const ACCOUNT_TYPE = {
  0: "CHECKING",
  1: "SAVINGS",
  2: "MONEY MARKET",
  3: "CERTIFICATE OF DEPOSIT",
  4: "INDIVIDUAL RETIREMENT ARRANGEMENT",
  5: "BROKERAGE",
};

/**
 * Enum for transaction status values
 * @readonly
 * @enum
 * */
export const TRANSACTION_STATUS = {
  0: "COMPLETED",
  1: "AUTHORISED",
  2: "INVALID",
  3: "CANCELLED",
  4: "AUTHORISATION_DECLINED",
  5: "AUTHORISED_CANCELED",
};
