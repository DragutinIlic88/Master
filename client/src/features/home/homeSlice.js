import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

import { API_URL, FETCH_DATA_STATUS, ENDPOINT_PATHS } from "../../constants";

const initialState = {
  accounts: null,
  transactions: null,
  status: FETCH_DATA_STATUS.IDLE,
  transactionStatus: FETCH_DATA_STATUS.IDLE,
  error: null,
  transactionError: null,
  accountError: null,
  loadMore: false,
};

export const getTransactions = createAsyncThunk(
  "home/getTransactions",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.TRANSACTIONS,
        data
      );
      return response.data;
    } catch (err) {
      if (!err.response) {
        throw err;
      }
      return rejectWithValue(err.response.data);
    }
  }
);

export const getAccounts = createAsyncThunk(
  "home/getAccounts",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.ACCOUNTS,
        data
      );
      return response.data;
    } catch (err) {
      if (!err.response) {
        throw err;
      }
      return rejectWithValue(err.response.data);
    }
  }
);

/**
 * Slice of redux state which is refering to the home information.
 * It has reducers seting account status, laoding more transactions and
 * reducers wrapped with thunk middleware for retrieveing accounts and transactions
 * from server.
 * It exposes that information as actions so it can be used inside components.
 */
export const homeSlice = createSlice({
  name: "home",
  initialState,
  reducers: {
    setAccountStatus(state, action) {
      state.status = action.payload;
    },
    setTransactionStatus(state, action) {
      state.transactionStatus = action.payload;
    },
    setTransactionError(state, action) {
      state.transactionError = action.payload;
    },
    setLoadMore(state, action) {
      state.loadMore = action.payload;
    },
  },
  extraReducers: {
    [getAccounts.pending]: (state, action) => {
      state.status = FETCH_DATA_STATUS.LOADING;
    },
    [getAccounts.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.accountError = action.payload.errorMessage;
        state.status = FETCH_DATA_STATUS.FAILED;
      } else if (
        !action.payload.accounts ||
        action.payload.accounts.length === 0
      ) {
        state.accountError = "UserDoesNotHaveAccountError";
        state.status = FETCH_DATA_STATUS.FAILED;
      } else {
        state.accounts = action.payload.accounts;
        state.accountError = null;
        state.status = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [getAccounts.rejected]: (state, action) => {
      state.status = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.accountError = action.payload;
        } else if (action.payload.errorMessage)
          state.accountError = action.payload.errorMessage;
        else {
          state.accountError = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message) state.accountError = action.error.message;
          else if (typeof action.error === "string")
            state.accountError = action.error;
          else {
            state.accountError = "ErrorOccurred";
          }
        } else {
          state.accountError = "ErrorOccurred";
        }
      }
    },

    [getTransactions.pending]: (state, action) => {
      state.transactionStatus = FETCH_DATA_STATUS.LOADING;
    },
    [getTransactions.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.transactionError = action.payload.errorMessage;
        state.transactionStatus = FETCH_DATA_STATUS.FAILED;
      } else {
        if (state.loadMore) {
          state.transactions = state.transactions.concat(
            action.payload.transactions
          );
          state.loadMore = false;
        } else {
          state.transactions = action.payload.transactions;
        }
        state.transactionError = null;
        state.transactionStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [getTransactions.rejected]: (state, action) => {
      state.transactionStatus = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.transactionError = action.payload;
        } else if (action.payload.errorMessage)
          state.transactionError = action.payload.errorMessage;
        else {
          state.transactionError = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message)
            state.transactionError = action.error.message;
          else if (typeof action.error === "string")
            state.transactionError = action.error;
          else {
            state.transactionError = "ErrorOccurred";
          }
        } else {
          state.transactionError = "ErrorOccurred";
        }
      }
    },
  },
});

export default homeSlice.reducer;

export const {
  setAccountStatus,
  setLoadMore,
  setTransactionStatus,
  setTransactionError,
} = homeSlice.actions;

export const selectTransactions = (state) => state.reducer.home.transactions;
export const selectAccounts = (state) => state.reducer.home.accounts;
export const selectStatus = (state) => state.reducer.home.status;
export const selectTransactionStatus = (state) =>
  state.reducer.home.transactionStatus;
export const selectError = (state) => state.reducer.home.error;
export const selectAccountError = (state) => state.reducer.home.accountError;
export const selectTransactionError = (state) =>
  state.reducer.home.transactionError;
export const selectLoadMore = (state) => state.reducer.home.loadMore;
