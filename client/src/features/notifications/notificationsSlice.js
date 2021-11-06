import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

import { API_URL, ENDPOINT_PATHS, FETCH_DATA_STATUS } from "../../constants";

const initialState = {
  notificationStatus: FETCH_DATA_STATUS.IDLE,
  notificationIsReadStatus: FETCH_DATA_STATUS.IDLE,
  notificationIsDeltedStatus: FETCH_DATA_STATUS.IDLE,
  notificationError: null,
  notifications: [],
};

export const getNotifications = createAsyncThunk(
  "notifications/getNotifications",
  async (userToken, { rejectWithValue }) => {
    try {
      const response = await axios.get(
        API_URL + ENDPOINT_PATHS.GET_NOTIFICATIONS + "/" + userToken
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

export const deleteNotification = createAsyncThunk(
  "notifications/deleteNotification",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.delete(
        API_URL + ENDPOINT_PATHS.NOTIFICATIONS,
        {
          data: data,
        }
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

export const notificationIsRead = createAsyncThunk(
  "notifications/notificationIsRead",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.NOTIFICATIONS,
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

export const notificationSlice = createSlice({
  name: "notifications",
  initialState,
  reducers: {
    setNotificationIsReadStatus(state, action) {
      state.notificationIsReadStatus = action.payload;
    },
    setNotificationIsDeleteStatus(state, action) {
      state.notificationIsDeltedStatus = action.payload;
    },
    setNotifications(state, action) {
      state.notifications = action.payload;
    },
    resetNotifications(state, action) {
      state.notificationStatus = FETCH_DATA_STATUS.IDLE;
      state.notificationError = null;
    },
  },
  extraReducers: {
    [getNotifications.pending]: (state, action) => {
      state.notificationStatus = FETCH_DATA_STATUS.LOADING;
    },
    [getNotifications.fulfilled]: (state, action) => {
      if (action.payload.errorMessage) {
        state.notificationError = action.payload.errorMessage;
        state.notificationStatus = FETCH_DATA_STATUS.FAILED;
      } else {
        state.notifications = action.payload.notifications;
        state.notificationStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [getNotifications.rejected]: (state, action) => {
      if (action.payload) {
        state.notificationError = action.payload.errorMessage
          ? action.payload.errorMessage
          : action.payload;
      } else {
        state.notificationError = action.error;
      }
      state.notificationStatus = FETCH_DATA_STATUS.FAILED;
    },
    [deleteNotification.pending]: (state, action) => {
      state.notificationIsDeltedStatus = FETCH_DATA_STATUS.LOADING;
    },
    [deleteNotification.fulfilled]: (state, action) => {
      state.notificationIsDeltedStatus = FETCH_DATA_STATUS.SUCEEDED;
    },
    [deleteNotification.rejected]: (state, action) => {
      if (action.payload) {
        state.notificationError = action.payload.errorMessage
          ? action.payload.errorMessage
          : action.payload;
      } else {
        state.notificationError = action.error;
      }
      state.notificationIsDeltedStatus = FETCH_DATA_STATUS.FAILED;
    },
    [notificationIsRead.pending]: (state, action) => {
      state.notificationIsReadStatus = FETCH_DATA_STATUS.LOADING;
    },
    [notificationIsRead.fulfilled]: (state, action) => {
      state.notificationIsReadStatus = FETCH_DATA_STATUS.SUCEEDED;
    },
    [notificationIsRead.rejected]: (state, action) => {
      if (action.payload) {
        state.notificationError = action.payload.errorMessage
          ? action.payload.errorMessage
          : action.payload;
      } else {
        state.notificationError = action.error;
      }
      state.notificationIsReadStatus = FETCH_DATA_STATUS.IDLE;
    },
  },
});

export default notificationSlice.reducer;

export const {
  setNotifications,
  resetNotifications,
  setNotificationIsReadStatus,
  setNotificationIsDeleteStatus,
} = notificationSlice.actions;

export const selectNotificationStatus = (state) =>
  state.reducer.notifications.notificationStatus;
export const selectNotificationIsDeletedStatus = (state) =>
  state.reducer.notifications.notificationIsDeltedStatus;
export const selectNotificationIsReadStatus = (state) =>
  state.reducer.notifications.notificationIsReadStatus;
export const selectNotificationError = (state) =>
  state.reducer.notifications.notificationError;
export const selectNotifications = (state) =>
  state.reducer.notifications.notifications;
