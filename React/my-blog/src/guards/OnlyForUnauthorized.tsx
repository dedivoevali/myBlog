import React from 'react';
import {useSelector} from 'react-redux';
import {Navigate, useLocation} from 'react-router-dom';
import {ApplicationState, CurrentUserState} from '../redux';

const OnlyForUnauthorized = ({children}: { children: JSX.Element }) => {

    const location = useLocation();

    const user = useSelector<ApplicationState, (CurrentUserState | undefined | null)>(state => state.user);

    const fromPage = location.state?.from?.pathname || "/";

    if (!user) {
        return children
    } else {
        return <Navigate to={fromPage} state {...{from: location}} replace/>
    }
};

export default OnlyForUnauthorized;