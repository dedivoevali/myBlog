import React from 'react';
import ReactDOM from 'react-dom/client';
import {Provider} from 'react-redux';
import {BrowserRouter} from 'react-router-dom';
import {createStore} from 'redux';
import App from './App';
import {ApplicationState, CustomNotificationPayload, ReduxActionTypes} from './redux';
import {AvatarTokenKeyName, JwtTokenKeyName, UserIdTokenKeyName, UsernameTokenKeyName, applicationTheme} from './shared/config';
import {UserInfoCache} from "./shared/types";
import { ThemeProvider } from '@emotion/react';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);

const storageContainsPayload = (): boolean => localStorage.getItem(JwtTokenKeyName) !== null || sessionStorage.getItem(JwtTokenKeyName) !== null;

const fetchUserInfoFromStorage = (): (UserInfoCache | null) => {
    if (storageContainsPayload()) {
        return {
            id: parseInt(localStorage.getItem(UserIdTokenKeyName) || sessionStorage.getItem(UserIdTokenKeyName) || "0"),
            username: localStorage.getItem(UsernameTokenKeyName) || sessionStorage.getItem(UsernameTokenKeyName) || "",
            avatar: localStorage.getItem(AvatarTokenKeyName) || sessionStorage.getItem(AvatarTokenKeyName) || ""
        }
    } else {
        return null;
    }
}

const updateUserCacheInBrowserStorage = (cache: UserInfoCache) => {

    if (localStorage.getItem(JwtTokenKeyName)) {

        localStorage.setItem(UserIdTokenKeyName, cache.id.toString());
        localStorage.setItem(UsernameTokenKeyName, cache.username);
        localStorage.setItem(AvatarTokenKeyName, cache.avatar);

    } else {
        sessionStorage.setItem(UserIdTokenKeyName, cache.id.toString());
        sessionStorage.setItem(UsernameTokenKeyName, cache.username);
        sessionStorage.setItem(AvatarTokenKeyName, cache.avatar)
    }

}

const defaultState: ApplicationState = {
    isCurrentlyNotifying: false,
    notificationText: '',
    notificationSeverity: 'info',
    user: fetchUserInfoFromStorage()
}

const reducer = (state = defaultState, action: { type: string, payload: boolean | CustomNotificationPayload | UserInfoCache }) => {
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

        case ReduxActionTypes.ChangeUser: {
            if (action.payload instanceof UserInfoCache) {

                updateUserCacheInBrowserStorage(action.payload);
                return {...state, user: action.payload, isAuthorized: true}
            } else if (action.payload === null) {
                return {...state, user: null, isAuthorized: false}
            }
            return state;
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