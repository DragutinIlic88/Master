import React from "react";
import { Route, Redirect } from "react-router-dom";
import { useSelector } from "react-redux";

import { selectToken } from "../../features/login/loginSlice";

/**
 * Wrapper around Route component which has constraint that can be show just in case if user is logged in.
 * Every time user trys to go on some page component will check if user is logged in by checking token of user object.
 * If user is logged in Component will go to the requested page, otherwise user will be redirected on login page.
 * @param {object} props
 */
const ProtectedRoute = ({ children, ...rest }) => {
  const token = useSelector(selectToken);
  return (
    <Route
      {...rest}
      render={({ location }) =>
        !!token ? (
          children
        ) : (
          <Redirect to={{ pathname: "/login", state: { from: location } }} />
        )
      }
    />
  );
};

export default ProtectedRoute;
