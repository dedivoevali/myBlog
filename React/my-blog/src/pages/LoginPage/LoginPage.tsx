import React from 'react';
import { LoginForm } from '../../components';
import styles from './login-page.module.scss';
import { useTitle } from '../../hooks/use-title';

const LoginPage = () => {
    useTitle('LOGIN');
    return (
        <div className={styles["container"]}>
            <LoginForm/>
        </div>
    );
};

export {LoginPage};