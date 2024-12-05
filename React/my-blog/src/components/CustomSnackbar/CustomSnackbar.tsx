import {Alert, AlertColor, Snackbar} from '@mui/material';
import React from 'react';
import styles from "./custom-snackbar.module.scss";

interface SnackbarProps {
    isOpen: boolean,
    alertMessage: string,
    alertType: AlertColor,
    closeHandler: () => void
}

const CustomSnackbar = ({
                            isOpen = false,
                            alertMessage,
                            alertType,
                            closeHandler
                        }: SnackbarProps) => {

    const onClose = () => {
        closeHandler();
    }

    return (

        <Snackbar transitionDuration={1000} onClose={onClose} open={isOpen} autoHideDuration={3000}
                  message={alertMessage}>
            <Alert className={styles.alert} severity={alertType}>{alertMessage}</Alert>
        </Snackbar>
    )
};

export {CustomSnackbar}