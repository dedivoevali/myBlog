import React from 'react';
import {useSelector} from 'react-redux';
import {LoginForm} from '../../components';
import {ApplicationState} from '../../redux';

const LoginPage = () => {

    const isAuthorized = useSelector<ApplicationState>(state => state.isAuthorized);

    return (
        <div style={{padding: "15vh 0 0 0", margin: "0 auto", display: "flex", justifyContent: "space-around"}}>
            <LoginForm/>
        </div>
    );
};

export {LoginPage};