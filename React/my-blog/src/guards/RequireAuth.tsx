import React from 'react';
import {useSelector} from 'react-redux';
import {Navigate, useLocation} from 'react-router-dom';
import {ApplicationState, CurrentUserState} from '../redux';

const RequireAuth = ({children}: { children: JSX.Element }) => {

    const location = useLocation();

    const user = useSelector<ApplicationState, (CurrentUserState | null | undefined)>(state => state.user);

    if (user) {
        return children
    } else {
        return <Navigate to="/login" state {...{from: location}} replace/>
    }
};

export {RequireAuth};