import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { API_URL, ENDPOINT_PATHS, FETCH_DATA_STATUS } from "../../constants";

import { SERVICES } from "../../constants";

const initialState = {
  activeService: SERVICES.HOME,
  getHelpInformationStatus: FETCH_DATA_STATUS.IDLE,
  getHelpInformationError: null,
  helpInfo: {},
};

export const getHelpInformation = createAsyncThunk(
  "navigationBar/getHelpInformation",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.get(API_URL + ENDPOINT_PATHS.HELP);
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
 * Slice of redux state which is refering to the navigation bar information.
 * Stores and exposes informations for service which is currently active and for help information
 * which are received from server.
 */
export const navigationBarSlice = createSlice({
  name: "navigationBar",
  initialState,
  reducers: {
    setActiveService(state, action) {
      state.activeService = action.payload;
    },
  },
  extraReducers: {
    [getHelpInformation.pending]: (state, action) => {
      state.getHelpInformationStatus = FETCH_DATA_STATUS.LOADING;
    },
    [getHelpInformation.fulfilled]: (state, action) => {
      state.helpInfo = action.payload;
      state.getHelpInformationStatus = FETCH_DATA_STATUS.SUCEEDED;
    },
    [getHelpInformation.rejected]: (state, action) => {
      if (action.payload) {
        if (action.payload.errorMessge) {
          state.getHelpInformationError = action.payload.errorMessge;
        } else {
          state.getHelpInformationError = action.payload;
        }
      } else {
        state.error = action.error;
      }
      state.getHelpInformationStatus = FETCH_DATA_STATUS.FAILED;
    },
  },
});

export const { setActiveService } = navigationBarSlice.actions;

export default navigationBarSlice.reducer;

export const selectActiveService = (state) =>
  state.reducer.navigationBar.activeService;

export const selectHelpInfo = (state) => state.reducer.navigationBar.helpInfo;
export const selectHelpInfoStatus = (state) =>
  state.reducer.navigationBar.getHelpInformationStatus;
