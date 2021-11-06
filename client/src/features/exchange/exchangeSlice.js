import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

import { API_URL, ENDPOINT_PATHS, FETCH_DATA_STATUS } from "../../constants";
import { extractErrorsFromResponse } from "../../utility";

const initialState = {
  currencies: [],
  currenciesStatus: FETCH_DATA_STATUS.IDLE,
  exchangeError: null,
  fee: null,
  feeStatus: FETCH_DATA_STATUS.IDLE,
  exchangeStatus: FETCH_DATA_STATUS.IDLE,
};

export const getCurrencies = createAsyncThunk(
  "exchange/getCurrencies",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.CURRENCIES,
        data
      );
      return response.data;
    } catch (err) {
      const error = extractErrorsFromResponse(
        err.response ? err.response.data : err
      );
      return rejectWithValue(error);
    }
  }
);

export const getFeeInfo = createAsyncThunk(
  "exchange/getFeeInfo",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(API_URL + ENDPOINT_PATHS.FEES, data);
      return response.data;
    } catch (err) {
      const error = extractErrorsFromResponse(
        err.response ? err.response.data : err
      );
      return rejectWithValue(error);
    }
  }
);

export const confirmExchange = createAsyncThunk(
  "exchange/confirmExchange",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.EXCHANGE,
        data
      );
      return response.data;
    } catch (err) {
      const error = extractErrorsFromResponse(
        err.response ? err.response.data : err
      );
      return rejectWithValue(error);
    }
  }
);

/**
 * Slice of redux state which is refering to the exchange information.
 * It has reducers for reseting fee information, for reseting exchange
 * information and reducers wrapped with thunk middleware for retrieveing
 * currencies , fee infromations and for confirming exchange process from/with
 * server.
 * It exposes that information as actions so it can be used inside components.
 */
export const exchangeSlice = createSlice({
  name: "exchange",
  initialState,
  reducers: {
    resetFeeInformation(state, action) {
      state.feeStatus = FETCH_DATA_STATUS.IDLE;
      state.fee = null;
    },
    resetExchangeInformation(state, action) {
      state.currencies = [];
      state.feeStatus = FETCH_DATA_STATUS.IDLE;
      state.fee = null;
      state.exchangeError = null;
      state.exchangeStatus = FETCH_DATA_STATUS.IDLE;
      state.currenciesStatus = FETCH_DATA_STATUS.IDLE;
    },
  },
  extraReducers: {
    [getCurrencies.pending]: (state, action) => {
      state.currenciesStatus = FETCH_DATA_STATUS.LOADING;
    },
    [getCurrencies.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.exchangeError = action.payload.errorMessage;
        state.currenciesStatus = FETCH_DATA_STATUS.FAILED;
      } else if (
        !action.payload.currencies ||
        action.payload.currencies.length === 0
      ) {
        state.exchangeError = "CurrenciesNotRetrievedError";
        state.currenciesStatus = FETCH_DATA_STATUS.FAILED;
      } else {
        state.currencies = action.payload.currencies;
        state.currenciesStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [getCurrencies.rejected]: (state, action) => {
      state.status = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.exchangeError = action.payload;
        } else if (action.payload.errorMessage)
          state.exchangeError = action.payload.errorMessage;
        else {
          state.exchangeError = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message) state.exchangeError = action.error.message;
          else if (typeof action.error === "string")
            state.exchangeError = action.error;
          else {
            state.exchangeError = "ErrorOccurred";
          }
        } else {
          state.exchangeError = "ErrorOccurred";
        }
      }
    },

    [getFeeInfo.pending]: (state, action) => {
      state.feeStatus = FETCH_DATA_STATUS.LOADING;
    },
    [getFeeInfo.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.exchangeError = action.payload.errorMessage;
        state.feeStatus = FETCH_DATA_STATUS.FAILED;
      } else {
        state.fee = action.payload;
        state.feeStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [getFeeInfo.rejected]: (state, action) => {
      state.feeStatus = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.exchangeError = action.payload;
        } else if (action.payload.errorMessage)
          state.exchangeError = action.payload.errorMessage;
        else {
          state.exchangeError = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message) state.exchangeError = action.error.message;
          else if (typeof action.error === "string")
            state.exchangeError = action.error;
          else {
            state.exchangeError = "ErrorOccurred";
          }
        } else {
          state.exchangeError = "ErrorOccurred";
        }
      }
    },
    [confirmExchange.pending]: (state, action) => {
      state.exchangeStatus = FETCH_DATA_STATUS.LOADING;
    },
    [confirmExchange.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.exchangeError = action.payload.errorMessage;
        state.exchangeStatus = FETCH_DATA_STATUS.FAILED;
      } else {
        state.exchangeStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [confirmExchange.rejected]: (state, action) => {
      state.exchangeStatus = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.exchangeError = action.payload;
        } else if (action.payload.errorMessage)
          state.exchangeError = action.payload.errorMessage;
        else {
          state.exchangeError = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message) state.exchangeError = action.error.message;
          else if (typeof action.error === "string")
            state.exchangeError = action.error;
          else {
            state.exchangeError = "ErrorOccurred";
          }
        } else {
          state.exchangeError = "ErrorOccurred";
        }
      }
    },
  },
});

export default exchangeSlice.reducer;

export const { resetFeeInformation, resetExchangeInformation } =
  exchangeSlice.actions;

export const selectCurrencies = (state) => state.reducer.exchange.currencies;
export const selectCurrenciesStatus = (state) =>
  state.reducer.exchange.currenciesStatus;
export const selectExchangeError = (state) =>
  state.reducer.exchange.exchangeError;
export const selectFee = (state) => state.reducer.exchange.fee;
export const selectFeeStatus = (state) => state.reducer.exchange.feeStatus;
export const selectExchangeStatus = (state) =>
  state.reducer.exchange.exchangeStatus;
