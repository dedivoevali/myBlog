import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import { createStore } from 'redux';
import App from './App';
import { ApplicationState, CurrentUserState, CustomNotificationPayload, ReduxActionTypes } from './redux';
import { JwtTokenKeyName, UserIdTokenKeyName, applicationTheme } from './shared/config';
import { ThemeProvider } from '@emotion/react';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);

const storageContainsPayload = (): boolean => localStorage.getItem(JwtTokenKeyName) !== null || sessionStorage.getItem(JwtTokenKeyName) !== null;

const fetchUserInfoFromStorage = (): (CurrentUserState | null) => {
    if (storageContainsPayload()) {
        return {
            id: parseInt(localStorage.getItem(UserIdTokenKeyName) || sessionStorage.getItem(UserIdTokenKeyName) || "0"),
            accessToken: localStorage.getItem(JwtTokenKeyName) || sessionStorage.getItem(JwtTokenKeyName) || "" 
        };
    } else {
        return null;
    }
}

const updateUserCacheInBrowserStorage = (cache: CurrentUserState) => {
    if (localStorage.getItem(JwtTokenKeyName)) {
        localStorage.setItem(UserIdTokenKeyName, cache.id.toString());
    } else {
        sessionStorage.setItem(UserIdTokenKeyName, cache.id.toString());
    }
}

const defaultState: ApplicationState = {
    isCurrentlyNotifying: false,
    notificationText: '',
    notificationSeverity: 'info',
    user: fetchUserInfoFromStorage()
}

const reducer = (state = defaultState, action: { type: string, payload: boolean | CustomNotificationPayload | CurrentUserState }) => {
    switch (action.type) {

        case ReduxActionTypes.ChangeNotification: {
            if (action.payload instanceof CustomNotificationPayload) {
                return {
                    ...state,
                    notificationText: action.payload.message,
                    notificationSeverity: action.payload.severity
                };
            }
            return state;
        }
        case ReduxActionTypes.DisplayNotification: {
            if (typeof action.payload === "boolean") {
                return {...state, isCurrentlyNotifying: action.payload}
            }
            return state;
        }
        case ReduxActionTypes.DeleteUser: {
            localStorage.clear();
            sessionStorage.clear();
            return { ...state, user: null, isAuthorized: false }
        }
        case ReduxActionTypes.ChangeUser: {
            let payload = action.payload as CurrentUserState;
            updateUserCacheInBrowserStorage(payload);
            return {...state, user: action.payload, isAuthorized: true}
        }
        default:
            return state
    }
}

const store = createStore(reducer);


root.render(
    <Provider store={store}>
        <ThemeProvider theme={applicationTheme}>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
                <BrowserRouter>
                    <App/>
                </BrowserRouter>
            </LocalizationProvider>
        </ThemeProvider>
    </Provider>
);