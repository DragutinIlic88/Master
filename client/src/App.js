import React, { Suspense } from "react";
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Redirect,
} from "react-router-dom";

import { SERVICES_PATH } from "./constants";

import Login from "./features/login/Login";
import SignUpConfiramtion from "./features/login/SignUpConfirmation/SignUpConfirmation";
import classes from "./App.module.css";

import Loader from "./components/UI/Loader/Loader";
import ProtectedRoute from "./components/ProtectedRoute/ProtectedRoute";
import PageLayout from "./components/PageLayout/PageLayout";
import Home from "./features/home/Home";
import History from "./features/history/History";
import Exchange from "./features/exchange/Exchange";
import Payment from "./features/payment/Payment";
import Loan from "./features/loan/Loan";
import Profile from "./features/profile/Profile";
import Notifications from "./features/notifications/Notifications";

/**
 * Top level component which contains navigation logic.
 * It uses routing mechanisam of react-router-dom package used
 * for navigation between different components inside app.
 */
function App() {
  return (
    <Router>
      <div className={classes.App}>
        <Switch>
          <Route exact path="/">
            <Suspense fallback={<Loader />}>
              <Login />
            </Suspense>
          </Route>
          <Route exact path="/login">
            <Suspense fallback={<Loader />}>
              <Login />
            </Suspense>
          </Route>
          <Route exact path="/signUpConfirmation">
            <SignUpConfiramtion />
          </Route>
          <ProtectedRoute exact path={SERVICES_PATH.HOME}>
            <PageLayout>
              <Home />
            </PageLayout>
          </ProtectedRoute>
          <ProtectedRoute exact path={SERVICES_PATH.HISTORY}>
            <PageLayout>
              <History />
            </PageLayout>
          </ProtectedRoute>
          <ProtectedRoute exact path={SERVICES_PATH.EXCHANGE}>
            <PageLayout>
              <Exchange />
            </PageLayout>
          </ProtectedRoute>
          <ProtectedRoute exact path={SERVICES_PATH.PAYMENT}>
            <PageLayout>
              <Payment />
            </PageLayout>
          </ProtectedRoute>
          <ProtectedRoute exact path={SERVICES_PATH.NEW_LOAN}>
            <PageLayout>
              <Loan />
            </PageLayout>
          </ProtectedRoute>
          <ProtectedRoute exact path={SERVICES_PATH.LOAN}>
            <PageLayout>
              <Loan />
            </PageLayout>
          </ProtectedRoute>
          <ProtectedRoute exact path={SERVICES_PATH.PROFILE}>
            <PageLayout>
              <Profile />
            </PageLayout>
          </ProtectedRoute>
          <ProtectedRoute exact path={SERVICES_PATH.NOTIFICATIONS}>
            <PageLayout>
              <Notifications />
            </PageLayout>
          </ProtectedRoute>
          <Redirect to="/" />
        </Switch>
      </div>
    </Router>
  );
}

export default App;
