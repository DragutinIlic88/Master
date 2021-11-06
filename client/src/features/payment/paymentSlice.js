import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { extractErrorsFromResponse } from "../../utility";
import { API_URL, ENDPOINT_PATHS, FETCH_DATA_STATUS } from "../../constants";

const initialState = {
  paymentStatus: FETCH_DATA_STATUS.IDLE,
  paymentError: null,
};

export const sendPaymentInfo = createAsyncThunk(
  "payment/sendPaymentInfo",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(API_URL + ENDPOINT_PATHS.PAYMENT, data);
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
 * Slice of redux state which stores and exposes information for payment service.
 * It exposes action creators for reseting state to default and for sending payment
 * information to the server. State has error and status fields.
 */
export const paymentSlice = createSlice({
  name: "payment",
  initialState,
  reducers: {
    resetPaymentInformation(state, action) {
      state.paymentStatus = FETCH_DATA_STATUS.IDLE;
      state.paymentError = null;
    },
  },
  extraReducers: {
    [sendPaymentInfo.pending]: (state, action) => {
      state.paymentStatus = FETCH_DATA_STATUS.LOADING;
    },
    [sendPaymentInfo.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.paymentError = action.payload.errorMessage;
        state.paymentStatus = FETCH_DATA_STATUS.FAILED;
      } else {
        state.paymentStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [sendPaymentInfo.rejected]: (state, action) => {
      state.paymentStatus = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (typeof action.payload === "string") {
          state.paymentError = action.payload;
        } else if (action.payload.errorMessage)
          state.paymentError = action.payload.errorMessage;
        else {
          state.paymentError = "ErrorOccurred";
        }
      } else {
        if (action.error) {
          if (action.error.message) state.paymentError = action.error.message;
          else if (typeof action.error === "string")
            state.paymentError = action.error;
          else {
            state.paymentError = "ErrorOccurred";
          }
        } else {
          state.paymentError = "ErrorOccurred";
        }
      }
    },
  },
});

export default paymentSlice.reducer;

export const { resetPaymentInformation } = paymentSlice.actions;

export const selectPaymentStatus = (state) =>
  state.reducer.payment.paymentStatus;
export const selectPaymentError = (state) => state.reducer.payment.paymentError;
