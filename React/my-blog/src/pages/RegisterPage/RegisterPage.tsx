import React from 'react';
import { RegistrationForm } from '../../components/RegistrationForm';
import styles from "./profile-page.module.scss";
import { useTitle } from '../../hooks/use-title';

const RegisterPage = () => {
    useTitle('REGISTER');
    return (
        <div className={styles["container"]}>
            <RegistrationForm />
        </div>
    );
};

export { RegisterPage };