import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

import { API_URL, FETCH_DATA_STATUS, ENDPOINT_PATHS } from "../../constants";
import { extractErrorsFromResponse } from "../../utility";

const initialState = {
  user: {},
  token: null,
  status: FETCH_DATA_STATUS.IDLE,
  registrationStatus: FETCH_DATA_STATUS.IDLE,
  logoutStatus: FETCH_DATA_STATUS.IDLE,
  logoUploadStatus: FETCH_DATA_STATUS.IDLE,
  userGetStatus: FETCH_DATA_STATUS.IDLE,
  error: null,
  signUp: false,
};

export const auth = createAsyncThunk(
  "login/auth",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.AUTHENTICATE,
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

export const logout = createAsyncThunk(
  "login/logout",
  async (token, { rejectWithValue }) => {
    try {
      const response = await axios.get(
        `${API_URL}${ENDPOINT_PATHS.LOGOUT}/${token}`
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

export const signUp = createAsyncThunk(
  "login/signUp",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        API_URL + ENDPOINT_PATHS.REGISTER,
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

export const editUser = createAsyncThunk(
  "login/editUser",
  async (data, { rejectWithValue }) => {
    try {
      const response = await axios.post(
        `${API_URL}${ENDPOINT_PATHS.EDIT_PROFILE}`,
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

export const editUserLogo = createAsyncThunk(
  "login/editUserLogo",
  async (data, { rejectWithValue }) => {
    try {
      const response = axios.post(
        `${API_URL}${ENDPOINT_PATHS.EDIT_PROFILE_LOGO}`,
        data.logo,
        {
          onUploadProgress: data.onUploadProgress,
        }
      );
      return (await response).data;
    } catch (err) {
      const error = extractErrorsFromResponse(
        err.response ? err.response.data : err
      );
      return rejectWithValue(error);
    }
  }
);

export const getUserData = createAsyncThunk(
  "login/getUserData",
  async (userToken, { rejectWithValue }) => {
    try {
      const response = await axios.get(
        `${API_URL}${ENDPOINT_PATHS.GET_USER_PROFILE}/${userToken}`
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
 * Slice of redux state for storing state and exposing logic for saveing data
 * regarding login/signup and profile service. This slice exposes action
 * creators and reducers for switching between sinng up and login page and
 * for seting logo image on profile service. It also has wrapped reduceres
 * with thunk middleware for posting user information for sign up , log in
 * or editing some user information on server.
 * Exposed state information are user object which contain all information of
 * currently logged in user, status of login/signup process , status of uploading
 * logo image, error if some occurr while trying to login/signup or upload logo
 * and switcher between sign up and login page.
 */
export const loginSlice = createSlice({
  name: "login",
  initialState,
  reducers: {
    signUpSwitched(state, action) {
      state.signUp = action.payload.signUp;
    },
    setLogoUploadStatus(state, action) {
      state.logoUploadStatus = action.payload;
    },
    setUserGetStatus(state, action) {
      state.userGetStatus = action.payload;
    },
    setRegistrationStatus(state, action) {
      state.registrationStatus = action.payload;
    },
    setLogoutStatus(state, action) {
      state.logoutStatus = action.payload;
    },
    setLoginStatus(state, action) {
      state.status = action.payload;
    },
    setEditStatus(state, action) {
      state.status = action.payload;
    },
    setError(state, action) {
      state.error = action.payload;
    },
  },
  extraReducers: {
    [auth.pending]: (state, action) => {
      state.status = FETCH_DATA_STATUS.LOADING;
    },
    [auth.fulfilled]: (state, action) => {
      if (action.payload?.errorMessage) {
        state.status = FETCH_DATA_STATUS.FAILED;
        state.error = action.payload.errorMessage;
      } else {
        state.status = FETCH_DATA_STATUS.SUCEEDED;
        state.token = action.payload.userToken;
      }
    },
    [auth.rejected]: (state, action) => {
      state.status = FETCH_DATA_STATUS.FAILED;
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
    [logout.pending]: (state, action) => {
      state.logoutStatus = FETCH_DATA_STATUS.LOADING;
    },
    [logout.fulfilled]: (state, action) => {
      state.logoutStatus = FETCH_DATA_STATUS.SUCEEDED;
    },
    [logout.rejected]: (state, action) => {
      state.logoutStatus = FETCH_DATA_STATUS.FAILED;
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
    [signUp.pending]: (state) => {
      state.registrationStatus = FETCH_DATA_STATUS.LOADING;
    },
    [signUp.fulfilled]: (state, action) => {
      if (action.payload.isSuccess) {
        state.registrationStatus = FETCH_DATA_STATUS.SUCEEDED;
      } else if (action.payload?.errorMessage) {
        state.registrationStatus = FETCH_DATA_STATUS.FAILED;
        state.error = action.payload.errorMessage;
      } else {
        state.registrationStatus = FETCH_DATA_STATUS.FAILED;
      }
    },
    [signUp.rejected]: (state, action) => {
      state.registrationStatus = FETCH_DATA_STATUS.FAILED;
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

    [editUser.pending]: (state) => {
      state.status = FETCH_DATA_STATUS.LOADING;
    },
    [editUser.fulfilled]: (state, action) => {
      if (action.payload?.errorMessage) {
        state.status = FETCH_DATA_STATUS.FAILED;
        state.error = action.payload.errorMessage;
      } else {
        state.status = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [editUser.rejected]: (state, action) => {
      state.status = FETCH_DATA_STATUS.FAILED;
      if (action.payload) {
        if (action.payload.errorMessage)
          state.error = action.payload.errorMessage;
        else state.error = action.payload;
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

    [editUserLogo.pending]: (state) => {
      state.logoUploadStatus = FETCH_DATA_STATUS.LOADING;
    },
    [editUserLogo.fulfilled]: (state, action) => {
      if (action.payload?.errorMessage) {
        state.logoUploadStatus = FETCH_DATA_STATUS.FAILED;
        state.error = action.payload.errorMessage;
      } else {
        state.logoUploadStatus = FETCH_DATA_STATUS.SUCEEDED;
      }
    },
    [editUserLogo.rejected]: (state, action) => {
      state.logoUploadStatus = FETCH_DATA_STATUS.FAILED;
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

    [getUserData.pending]: (state) => {
      state.userGetStatus = FETCH_DATA_STATUS.LOADING;
    },
    [getUserData.fulfilled]: (state, action) => {
      if (action.payload?.errorMessage) {
        state.userGetStatus = FETCH_DATA_STATUS.FAILED;
        state.error = action.payload.errorMessage;
      } else {
        state.userGetStatus = FETCH_DATA_STATUS.SUCEEDED;
        state.user = action.payload;
      }
    },
    [getUserData.rejected]: (state, action) => {
      state.userGetStatus = FETCH_DATA_STATUS.FAILED;
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

export const {
  signUpSwitched,
  setLogoUploadStatus,
  setUserGetStatus,
  setRegistrationStatus,
  setLoginStatus,
  setError,
  setEditStatus,
} = loginSlice.actions;

export default loginSlice.reducer;

export const selectUser = (state) => state.reducer.login.user;
export const selectToken = (state) => state.reducer.login.token;
export const selectStatus = (state) => state.reducer.login.status;
export const selectRegistrationStatus = (state) =>
  state.reducer.login.registrationStatus;
export const selectLogoutStatus = (state) => state.reducer.login.logoutStatus;
export const selectLogoUploadStatus = (state) =>
  state.reducer.login.logoUploadStatus;
export const selectUserGetStatus = (state) => state.reducer.login.userGetStatus;
export const selectError = (state) => state.reducer.login.error;
export const selectSignUp = (state) => state.reducer.login.signUp;
