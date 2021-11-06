import { combineReducers } from "redux";
import loginReducer from "../features/login/loginSlice";
import navigationBarReducer from "../features/navigationBar/navigationBarSlice";
import homeReducer from "../features/home/homeSlice";
import exchangeReducer from "../features/exchange/exchangeSlice";
import paymentReducer from "../features/payment/paymentSlice";
import loanReducer from "../features/loan/loanSlice";
import notificationsReducer from "../features/notifications/notificationsSlice";
import { persistReducer } from "redux-persist";
import storage from "redux-persist/lib/storage";

import { ACTION_TYPES } from "../constants";

/**Configuration object for persistance of state */
const persistConfig = {
  key: "root",
  version: 1,
  storage,
};

/**
 * Making of app reducer by combining all reducer from slices into one.
 */
const appReducer = combineReducers({
  login: loginReducer,
  navigationBar: navigationBarReducer,
  home: homeReducer,
  exchange: exchangeReducer,
  payment: paymentReducer,
  loan: loanReducer,
  notifications: notificationsReducer,
});

/**
 * Root reducer is created because in some cases we want to reset
 * state of redux to default value. So if action type is reset , reseting
 * of state will be performed , otherwise appReducer with existing state
 * will be returned.
 * @param {object} state
 * @param {object} action
 */
const rootReducer = (state, action) => {
  if (action.type === ACTION_TYPES.RESET_STATE) {
    return appReducer(undefined, action);
  }

  return appReducer(state, action);
};

const persistedReducer = persistReducer(persistConfig, rootReducer);

export default persistedReducer;
