import { AssignmentInd, } from '@mui/icons-material';
import { Button, Checkbox, FormControl, FormControlLabel, FormHelperText, Input, InputLabel, Paper } from '@mui/material';
import { useFormik } from 'formik';
import React, { useEffect, useState } from 'react';
import { authApi } from '../../shared/api/http/api';
import { AuthenticateResponse, ErrorResponse } from '../../shared/api/types';
import { palette, PasswordValidationConstraints, UsernameValidationConstraints } from '../../shared/assets';
import { FormHeader } from '../FormHeader';
import { AuthenticationFormProps } from './AuthenticationFormProps';
import * as Yup from 'yup';
import { CenteredLoader } from '../CenteredLoader';
import { useDispatch } from 'react-redux';
import { CurrentUserState, ReduxActionTypes } from '../../redux';
import { Link } from 'react-router-dom';
import { useNotifier } from '../../hooks';
import { PasskeyApi } from '../../shared/api/http/passkey-api';
import { WebauthnService } from '../../shared/services/webauthn-service';
import { arrayBufferToBase64, arrayBufferToUtf8 } from '../../shared/assets/array-buffer-utils';
import { PasskeyAuthenticationRequest } from '../../shared/api/types/authentication/passkey/passkey-authentication-request';
import styles from  './login-form.module.scss';

const PASSKEY_OPERATION_CANCELED = "Passkey Operation Canceled";

const LoginForm = () => {

    const passkeyApi = PasskeyApi.create();
    const webauthnService = new WebauthnService(navigator, window);
    const passkeyAbortController = new AbortController();

    const dispatch = useDispatch();

    const setUser = (user: CurrentUserState) => {
        dispatch({type: ReduxActionTypes.ChangeUser, payload: user});
    }

    const [loading, setLoading] = useState(false);
    const displayNotification = useNotifier();
    const notifySucessfullAuth = () => displayNotification("Authorization successfull", "success");

    // clean up after destroy
    useEffect(() => {
        return () => passkeyAbortController.abort(PASSKEY_OPERATION_CANCELED);
    }, []);

    useEffect(() => {
        passkeyApi.getAuthenticationOptions().then((response) => {
            webauthnService.authenticateCredentialRequest(response, passkeyAbortController).then((resp) => {
                const credential = resp?.credential;
                const challenge = resp?.challenge;

                if (!credential || !challenge) {
                    return;
                }
                const response = credential.response as AuthenticatorAssertionResponse;
                const credentialId = arrayBufferToBase64(credential.rawId);
                const authenticatorData = arrayBufferToBase64(response.authenticatorData);
                const clientDataJson = arrayBufferToBase64(credential.response.clientDataJSON);
                const signature = arrayBufferToBase64(response.signature);
                const userHandle = arrayBufferToUtf8(response.userHandle ?? new ArrayBuffer(0));
                const type = credential.type;

                const payload: PasskeyAuthenticationRequest = {
                    credentialId,
                    authenticatorData,
                    clientDataJson,
                    signature,
                    userHandle,
                    type,
                    challenge
                }

                passkeyApi.authenticate(payload)
                .then((response: AuthenticateResponse) => {
                    authApi.setUserInStorage(response, formik.values.rememberMe);
                    setUser({ id: response.userId, accessToken: response.accessToken });
                    notifySucessfullAuth();
                })
                .catch((err) => displayNotification(err.response.data.Message, "error"));
                
            }).catch((err) => {
                // Supress
            });
        }).catch(err => {
            if (err.status !== 404) { // Not found is thrown when passkey feature is disabled
                throw err;
            }
        });
    }, []);

    const formik = useFormik<AuthenticationFormProps>({
        initialValues: {
            username: "",
            password: "",
            rememberMe: true
        },
        onSubmit: async (values) => {
            setLoading(true);
            const result = await authApi.tryAuthenticate({...values}, values.rememberMe);
            if (result.status !== 200) {
                const errorResult = (result as any).response.data as ErrorResponse;
                displayNotification(errorResult.Message, "error");
            } else {
                setUser({
                    id: result.data.userId,
                    accessToken: result.data.accessToken
                });
                notifySucessfullAuth();
            }
            setLoading(false);
        },
        validationSchema: Yup.object({
            username:
                Yup.string()
                    .required()
                    .min(UsernameValidationConstraints.MinLength)
                    .max(UsernameValidationConstraints.MaxLength)
                    .matches(UsernameValidationConstraints.Regexp, "Username should not begin or end with _")
            ,
            password:
                Yup.string()
                    .required()
                    .min(PasswordValidationConstraints.MinLength)
                    .max(PasswordValidationConstraints.MaxLength)
        })
    });


    return (
        <>
            {loading ?
                <CenteredLoader />
                :
                <form className={styles.form} onSubmit={formik.handleSubmit}>

                    <Paper className={styles["block-wrapper"]} elevation={12}>

                        <FormHeader iconColor={palette.SOFT_ORANGE} caption="Login" icon={<AssignmentInd />} />

                        <FormControl className={styles["form-field"]}>
                            <InputLabel htmlFor="username">Username</InputLabel>
                            <Input onChange={formik.handleChange} value={formik.values.username} name="username" autoComplete="username webauthn" />
                            <FormHelperText>
                                {formik.touched.username && formik.errors.username && (
                                    <span className={styles.error}>{formik.errors.username}</span>)}
                            </FormHelperText>
                        </FormControl>

                        <FormControl className={styles["form-field"]}>
                            <InputLabel htmlFor="password">Password</InputLabel>
                            <Input type="password" onChange={formik.handleChange} value={formik.values.password}
                                name="password" autoComplete="current-password webauthn" />
                            <FormHelperText>
                                {formik.touched.password && formik.errors.password && (
                                    <span className={styles.error}>{formik.errors.password}</span>)}
                            </FormHelperText>
                        </FormControl>

                        <FormControlLabel name="rememberMe" checked={formik.values.rememberMe} className={styles.checkbox} onChange={formik.handleChange}
                            label="Remember me?" control={<Checkbox />} />

                        <Button variant="outlined" className={styles.submit} type="submit">Log in</Button>

                        <Link className={styles.register} style={{ color: palette.JET }} to="/register">
                            Do not have an account? <u>Click here</u>
                        </Link>
                    </Paper>
                </form>
            }
        </>
    );
};

export {LoginForm};