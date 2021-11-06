import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { extractErrorsFromResponse } from "../../utility";
import { API_URL, ENDPOINT_PATHS, FETCH_DATA_STATUS } from "../../constants";

const initialState = {
  loans: null,
  loanStatus: FETCH_DATA_STATUS.IDLE,
  postLoanStatus: FETCH_DATA_STATUS.IDLE,
  error: null,
};

export const getLoans = createAsyncThunk(
  "loan/getLoans",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.LOANS + "/getLoans",
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

export const postLoan = createAsyncThunk(
  "loan/postLoan",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.LOANS + "/postLoanRequest",
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
 * Slice of redux state which is refering to the loans information.
 * Stores and exposes informations for loans of currently logged in user.
 * It has reducers for geting list of all loans from server for logged in user and posting new loan.
 */
export const loanSlice = createSlice({
  name: "loan",
  initialState,
  reducers: {
    resetLoansState(state, action) {
      state.error = null;
      state.postLoanStatus = FETCH_DATA_STATUS.IDLE;
    },
  },
  extraReducers: {
    [getLoans.pending]: (state, action) => {
      state.loanStatus = FETCH_DATA_STATUS.LOADING;
    },
    [getLoans.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.error = action.payload.errorMessage;
        state.loanStatus = FETCH_DATA_STATUS.FAILED;
      } else {
        state.loans = action.payload;
        state.loanStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [getLoans.rejected]: (state, action) => {
      state.loanStatus = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.error = action.payload;
        } else if (action.payload.errorMessage)
          state.error = action.payload.errorMessage;
        else {
          state.error = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message) state.error = action.error.message;
          else if (typeof action.error === "string") state.error = action.error;
          else {
            state.error = "ErrorOccurred";
          }
        } else {
          state.error = "ErrorOccurred";
        }
      }
    },

    [postLoan.pending]: (state, action) => {
      state.postLoanStatus = FETCH_DATA_STATUS.LOADING;
    },
    [postLoan.fulfilled]: (state, action) => {
      state.loans = action.payload.loansList;
      state.postLoanStatus = FETCH_DATA_STATUS.SUCEEDED;
    },
    [postLoan.rejected]: (state, action) => {
      state.loanStatus = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.error = action.payload;
        } else if (action.payload.errorMessage)
          state.error = action.payload.errorMessage;
        else {
          state.error = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message) state.error = action.error.message;
          else if (typeof action.error === "string") state.error = action.error;
          else {
            state.error = "ErrorOccurred";
          }
        } else {
          state.error = "ErrorOccurred";
        }
      }
    },
  },
});

export default loanSlice.reducer;

export const { resetLoansState } = loanSlice.actions;

export const selectLoans = (state) => state.reducer.loan.loans;
export const selectLoanStatus = (state) => state.reducer.loan.loanStatus;
export const selectPostLoanStatus = (state) =>
  state.reducer.loan.postLoanStatus;
export const selectError = (state) => state.reducer.loan.error;
