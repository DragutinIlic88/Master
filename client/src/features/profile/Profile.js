import React, { useRef, useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useSelector, useDispatch } from "react-redux";
import { ToastContainer, toast } from "react-toastify";

import {
  selectLogoUploadStatus,
  editUser,
  selectStatus,
  setLogoUploadStatus,
  editUserLogo,
  getUserData,
  selectUserGetStatus,
  selectUser,
  setUserGetStatus,
  selectToken,
  setEditStatus,
} from "../login/loginSlice";
import Modal from "../../components/UI/Modal/Modal";
import EditProfilePage from "./EditProfilePage/EditProfilePage";
import Loader from "../../components/UI/Loader/Loader";

import { emailPattern, mobileInsertPattern } from "../../utility";
import { FETCH_DATA_STATUS, PROFILE_INFO_TYPES } from "../../constants";

import classes from "./Profile.module.css";
import "react-toastify/dist/ReactToastify.css";
import ImagePlaceholder from "../../assets/images/user-placeholder.png";
import PencilIcon from "../../assets/images/edit-pencil-icon.png";

/**
 * Component for showing and editing profile information of user.
 *
 * @component
 */
const Profile = () => {
  const uploadedImage = useRef(null);
  const { t } = useTranslation();
  const user = useSelector(selectUser);
  const token = useSelector(selectToken);
  const editStatus = useSelector(selectStatus);
  const logoUploadStatus = useSelector(selectLogoUploadStatus);
  const userGetStatus = useSelector(selectUserGetStatus);
  const dispatch = useDispatch();
  const [editTab, setEditTab] = useState("");
  const [userName, setUserName] = useState("");
  const [mobile, setMobile] = useState("");
  const [email, setEmail] = useState("");
  const [address, setAddress] = useState("");
  const [editErrorMessage, setEditErrorMessage] = useState(null);
  const [logoImage, setLogoImage] = useState(null);
  const [showSubmitLogo, setShowSubmitLogo] = useState(false);
  const [registrationDate, setRegistrationDate] = useState("");
  const [lastLoginDate, setLastLoginDate] = useState("");
  const [progress, setProgress] = useState(0);

  let editContent = null;

  useEffect(() => {
    dispatch(getUserData(token));
  }, [token, dispatch]);

  /**
   * @event
   * Event handler which is triggered when user wants to
   * cancel editing of some propertie. Sets edited inforamtion
   * back on initial value and closes editProfilePage component
   * @param {PROFILE_INFO_TYPES} type
   */
  const handleOnEditPageClosed = (type) => {
    if (type) {
      switch (type) {
        case PROFILE_INFO_TYPES.USER_NAME:
          setUserName(user.userName);
          break;
        case PROFILE_INFO_TYPES.MOBILE:
          setMobile(user.mobileNumber);
          break;
        case PROFILE_INFO_TYPES.EMAIL:
          setEmail(user.email);
          break;
        case PROFILE_INFO_TYPES.ADDRESS:
          setAddress(user.homeAddress);
          break;
        default:
          return;
      }
    }
    setEditTab("");
  };

  /**
   * @event
   * Event handler which is triggered when user
   * wants to submit edited information to the server.
   * Information which is changed is dispatched and sent
   * to the server. EditProfilePage component is closed
   * @param {PROFILE_INFO_TYPES} type
   */
  const handleOnEditPageSubmit = (type) => {
    switch (type) {
      case PROFILE_INFO_TYPES.USER_NAME:
        dispatch(editUser({ userName, userToken: token }));
        break;
      case PROFILE_INFO_TYPES.MOBILE:
        dispatch(editUser({ mobile, userToken: token }));
        break;
      case PROFILE_INFO_TYPES.EMAIL:
        let regex = new RegExp(emailPattern);
        if (regex.test(email)) {
          dispatch(editUser({ email, userToken: token }));
        } else {
          setEditErrorMessage(t("EmailIncorrect"));
        }
        break;
      case PROFILE_INFO_TYPES.ADDRESS:
        dispatch(editUser({ address, userToken: token }));
        break;
      default:
        return;
    }
    setEditErrorMessage(null);
    setEditTab("");
  };

  /**
   * @event
   * Event is triggered when user wants to change logo image.
   * It reads image file which user choosed , set that image as logo
   * and displays submit button in case that user wants to save changes.
   * @param {object} e
   */
  const handleImageUpload = (e) => {
    const [file] = e.target.files;
    if (file) {
      const reader = new FileReader();
      const { current } = uploadedImage;
      current.file = file;
      reader.onload = (e) => {
        current.src = e.target.result;
      };
      reader.readAsDataURL(file);
      setLogoImage(file);
      setShowSubmitLogo(true);
    }
  };

  /**
   * @event
   * Event handler is triggered when user whants to save changed logo.
   * Appends logo image to the form data and dispatches that data to be
   * sent to the server
   */
  const sendImageToServer = () => {
    setProgress(0);
    const fd = new FormData();
    fd.append("logoFile", logoImage);
    fd.append("logoName", logoImage.name);
    fd.append("userToken", token);
    const onUploadProgress = (event) => {
      setProgress(Math.round((100 * event.loaded) / event.total));
    };

    dispatch(editUserLogo({ logo: fd, onUploadProgress }));
  };

  /**
   * Based on information which will be edited
   * proper EditProfilePage component is created
   */
  switch (editTab) {
    case PROFILE_INFO_TYPES.USER_NAME:
      editContent = (
        <EditProfilePage
          editStatus={editStatus}
          title={t("EditUserName")}
          labelText={t("InsertNewUserName")}
          inputValue={userName}
          inputChanged={(e) => {
            setUserName(e.target.value);
          }}
          inputType="text"
          closeLabel={t("Close")}
          changeLabel={t("Submit")}
          close={() => {
            handleOnEditPageClosed(PROFILE_INFO_TYPES.USER_NAME);
          }}
          change={() => {
            handleOnEditPageSubmit(PROFILE_INFO_TYPES.USER_NAME);
          }}
        />
      );
      break;
    case PROFILE_INFO_TYPES.MOBILE:
      editContent = (
        <EditProfilePage
          editStatus={editStatus}
          title={t("EditMobile")}
          labelText={t("InsertNewMobile")}
          inputValue={mobile}
          inputChanged={(e) => {
            setMobile(e.target.value);
          }}
          inputType="text"
          inputPattern={mobileInsertPattern}
          closeLabel={t("Close")}
          changeLabel={t("Submit")}
          close={() => {
            handleOnEditPageClosed(PROFILE_INFO_TYPES.MOBILE);
          }}
          change={() => {
            handleOnEditPageSubmit(PROFILE_INFO_TYPES.MOBILE);
          }}
        />
      );
      break;
    case PROFILE_INFO_TYPES.EMAIL:
      editContent = (
        <EditProfilePage
          editStatus={editStatus}
          title={t("EditEmail")}
          labelText={t("InsertNewEmail")}
          inputValue={email}
          inputChanged={(e) => {
            setEmail(e.target.value);
          }}
          inputType="email"
          closeLabel={t("Close")}
          changeLabel={t("Submit")}
          close={() => {
            handleOnEditPageClosed(PROFILE_INFO_TYPES.EMAIL);
          }}
          editErrorMessage={editErrorMessage}
          change={() => {
            handleOnEditPageSubmit(PROFILE_INFO_TYPES.EMAIL);
          }}
        />
      );
      break;
    case PROFILE_INFO_TYPES.ADDRESS:
      editContent = (
        <EditProfilePage
          editStatus={editStatus}
          title={t("EditAddress")}
          labelText={t("InsertNewAddress")}
          inputValue={address}
          inputChanged={(e) => {
            setAddress(e.target.value);
          }}
          inputType="text"
          closeLabel={t("Close")}
          changeLabel={t("Submit")}
          close={() => {
            handleOnEditPageClosed(PROFILE_INFO_TYPES.ADDRESS);
          }}
          change={() => {
            handleOnEditPageSubmit(PROFILE_INFO_TYPES.ADDRESS);
          }}
        />
      );
      break;
    default:
      editContent = null;
      break;
  }

  /**
   * handling change of status of geting user data from server
   * which can be used as profile information
   */
  if (userGetStatus === FETCH_DATA_STATUS.LOADING) {
    return (
      <div className={classes.Profile}>
        <Loader />
      </div>
    );
  }
  if (userGetStatus === FETCH_DATA_STATUS.FAILED) {
    return (
      <div className={classes.ProfileErrorMessage}>
        {t("LoadProfileDataError")}
      </div>
    );
  }
  if (userGetStatus === FETCH_DATA_STATUS.SUCEEDED) {
    setUserName(user.userName);
    setMobile(user.mobileNumber);
    setEmail(user.email);
    setAddress(user.homeAddress);
    setLogoImage(user.logo);
    setRegistrationDate(user.registrationDate);
    setLastLoginDate(user.lastLoginDate);
    dispatch(setUserGetStatus(FETCH_DATA_STATUS.IDLE));
  }
  if (
    editStatus === FETCH_DATA_STATUS.FAILED ||
    logoUploadStatus === FETCH_DATA_STATUS.FAILED
  ) {
    toast.error(t("EditUserDataFailed"), {
      autoClose: 5000,
    });
    setUserName(user.userName);
    setMobile(user.mobileNumber);
    setEmail(user.email);
    setAddress(user.homeAddress);
    setLogoImage(user.logo);
    setRegistrationDate(user.registrationDate);
    setLastLoginDate(user.lastLoginDate);
    dispatch(setEditStatus(FETCH_DATA_STATUS.IDLE));
    dispatch(setLogoUploadStatus(FETCH_DATA_STATUS.IDLE));
  }

  /**
   * when logo image is successfuly submited , "save image" button
   * is removed from screen and status is reseted to IDLE state
   */
  if (logoUploadStatus === FETCH_DATA_STATUS.SUCEEDED) {
    setShowSubmitLogo(false);
    dispatch(setLogoUploadStatus(FETCH_DATA_STATUS.IDLE));
    dispatch(getUserData(token));
  }

  if (editStatus === FETCH_DATA_STATUS.SUCEEDED) {
    dispatch(getUserData(token));
    dispatch(setEditStatus(FETCH_DATA_STATUS.IDLE));
  }

  if (logoUploadStatus === FETCH_DATA_STATUS.LOADING) {
    return (
      <div className={classes.Profile}>
        <Loader />
      </div>
    );
  }

  return (
    <div className={classes.Profile}>
      <Modal show={editTab !== ""} modalClosed={handleOnEditPageClosed}>
        {editContent}
      </Modal>
      <div className={classes.UserName}>
        <div className={classes.ProfileImageDiv}>
          <label htmlFor="file-input">
            <img
              src={logoImage ? logoImage : ImagePlaceholder}
              alt={t("ImageProfile")}
              className={classes.ProfileImage}
              ref={uploadedImage}
            />
          </label>
          <input
            id="file-input"
            type="file"
            accept="image/*"
            multiple={false}
            onChange={handleImageUpload}
          />
          {showSubmitLogo ? (
            <div className={classes.SaveImage} onClick={sendImageToServer}>
              {t("UploadImage")}
            </div>
          ) : null}
        </div>

        <div className={classes.UserNameDiv}>
          {user.userName}
          <div className={classes.EditDiv}>
            <img
              className={classes.EditImg}
              src={PencilIcon}
              alt="Edit user name"
              onClick={() => {
                setEditTab(PROFILE_INFO_TYPES.USER_NAME);
              }}
            />
          </div>
        </div>
      </div>
      <div className={classes.UserInfo}>
        <div className={classes.MobileInfo}>
          <div>{t("MobileNumber")}</div>
          <div className={classes.ProfileData}>
            <div className={classes.Data}>{user.mobileNumber}</div>
            <div className={classes.EditDiv}>
              <img
                className={classes.EditImg}
                src={PencilIcon}
                alt="Edit mobile"
                onClick={() => {
                  setEditTab(PROFILE_INFO_TYPES.MOBILE);
                }}
              />
            </div>
          </div>
        </div>
        <div className={classes.EmailInfo}>
          <div>{t("EmailAddressInput")}</div>
          <div className={classes.ProfileData}>
            <div className={classes.Data}>{user.email}</div>
            <div className={classes.EditDiv}>
              <img
                className={classes.EditImg}
                src={PencilIcon}
                alt="Edit email"
                onClick={() => {
                  setEditTab(PROFILE_INFO_TYPES.EMAIL);
                }}
              />
            </div>
          </div>
        </div>
        <div className={classes.AddressInfo}>
          <div>{t("Address")}</div>
          <div className={classes.ProfileData}>
            <div className={classes.Data}>{user.homeAddress}</div>
            <div className={classes.EditDiv}>
              <img
                className={classes.EditImg}
                src={PencilIcon}
                alt="Edit address"
                onClick={() => {
                  setEditTab(PROFILE_INFO_TYPES.ADDRESS);
                }}
              />
            </div>
          </div>
        </div>
        <div className={classes.SignInInfo}>
          <div>{t("SignUpDate")}</div>
          <div className={classes.ProfileData}>
            <div className={classes.Data}>{registrationDate}</div>
            <div className={classes.EditDiv}></div>
          </div>
        </div>
        <div className={classes.LastLoginInfo}>
          <div>{t("LastLogIn")}</div>
          <div className={classes.ProfileData}>
            <div className={classes.Data}>{lastLoginDate}</div>
            <div className={classes.EditDiv}></div>
          </div>
        </div>
      </div>
      <ToastContainer />
    </div>
  );
};

export default Profile;
