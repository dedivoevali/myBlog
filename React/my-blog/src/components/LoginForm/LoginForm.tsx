import {AssignmentInd,} from '@mui/icons-material';
import {Button, Checkbox, FormControl, FormControlLabel, FormHelperText, Input, InputLabel, Paper} from '@mui/material';
import {useFormik} from 'formik';
import React, {useEffect, useState} from 'react';
import {authApi, userApi} from '../../shared/api/http/api';
import {AuthenticateResponse, ErrorResponse} from '../../shared/api/types';
import {palette, PasswordValidationConstraints, UsernameValidationConstraints} from '../../shared/assets';
import {FormHeader} from '../FormHeader';
import {AuthenticationFormProps} from './AuthenticationFormProps';
import * as Yup from 'yup';
import {AxiosError, AxiosResponse} from 'axios';
import {CenteredLoader} from '../CenteredLoader';
import {useDispatch} from 'react-redux';
import {ReduxActionTypes} from '../../redux';
import {Link} from 'react-router-dom';
import {useNotifier} from '../../hooks';
import {UserInfoCache} from "../../shared/types";
import { PasskeyApi } from '../../shared/api/http/passkey-api';
import { WebauthnService } from '../../shared/services/webauthn-service';
import { arrayBufferToBase64, arrayBufferToUtf8 } from '../../shared/assets/array-buffer-utils';
import { PasskeyAuthenticationRequest } from '../../shared/api/types/authentication/passkey/passkey-authentication-request';

const textFieldStyle: React.CSSProperties = {
    maxWidth: "400px",
    width: "20vw",
    minWidth: "300px",
    margin: "0 auto",
    padding: "0 0 20px 0"
};

const paperStyle: React.CSSProperties = {
    width: "450px",
    padding: "20px",
    display: "flex",
    flexDirection: "column"
}

const buttonStyle: React.CSSProperties = {
    margin: "0 auto"
}

const checkboxStyle: React.CSSProperties = {
    margin: "0 auto"
}

const errorTextStyle: React.CSSProperties = {
    color: "red",
    fontStyle: "italic"
}

const LoginForm = () => {

    const passkeyApi = PasskeyApi.create();
    const webauthnService = new WebauthnService(navigator, window);

    const dispatch = useDispatch();

    const setUser = (user: UserInfoCache) => {
        dispatch({type: ReduxActionTypes.ChangeUser, payload: user});
    }

    const fetchAvatarUrl = (userId: number) => userApi.getAvatarUrlById(userId).then((result: AxiosResponse<string>) => result.data);

    const [loading, setLoading] = useState(false);
    const displayNotification = useNotifier();
    const notifySucessfullAuth = () => displayNotification("Authorization successfull", "success");

    useEffect(() => {
        passkeyApi.getAuthenticationOptions().then((response) => {
            const ac = new AbortController();
            webauthnService.authenticateCredentialRequest(response, ac).then((resp) => {

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
                    authApi.setJwtAndPayloadInStorage(response, formik.values.rememberMe);
                    setUser(new UserInfoCache(response.id, response.username));
                    notifySucessfullAuth();
                })
                .catch((err) => displayNotification(err.response.data.Message, "error"));
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
            rememberMe: false
        },
        onSubmit: async (values) => {
            setLoading(true);
            const result = await authApi.TryAuthenticateAndPayloadInHeaders({...values}, values.rememberMe);

            if (result.status !== 200 && result instanceof AxiosError<ErrorResponse>) {
                const errorMessage = result.response?.data.Message;
                displayNotification(errorMessage, "error");
            } else {
                fetchAvatarUrl(result.data.id).then((resultOfAvatarFetching) => {
                    setUser(new UserInfoCache(result.data.id, result.data.username, resultOfAvatarFetching));
                    notifySucessfullAuth();
                })
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
                <CenteredLoader/>
                :
                <form style={{display: "inline-block"}} onSubmit={formik.handleSubmit}>

                    <Paper style={paperStyle} elevation={12}>

                        <FormHeader iconColor={palette.SOFT_ORANGE} caption="Login" icon={<AssignmentInd/>}/>

                        <FormControl style={textFieldStyle}>
                            <InputLabel htmlFor="username">Username</InputLabel>
                            <Input onChange={formik.handleChange} value={formik.values.username} name="username" autoComplete="username webauthn"/>
                            <FormHelperText>
                                {formik.touched.username && formik.errors.username && (
                                    <span style={errorTextStyle}>{formik.errors.username}</span>)}
                            </FormHelperText>
                        </FormControl>

                        <FormControl style={textFieldStyle}>
                            <InputLabel htmlFor="password">Password</InputLabel>
                            <Input type="password" onChange={formik.handleChange} value={formik.values.password}
                                   name="password" autoComplete="current-password webauthn"/>
                            <FormHelperText>
                                {formik.touched.password && formik.errors.password && (
                                    <span style={errorTextStyle}>{formik.errors.password}</span>)}
                            </FormHelperText>
                        </FormControl>

                        <FormControlLabel name="rememberMe" style={checkboxStyle} onChange={formik.handleChange}
                                          label="Remember me?" control={<Checkbox/>}/>

                        <Button variant="outlined" style={buttonStyle} type="submit">Log in</Button>

                        <Link style={{
                            textAlign: "center",
                            fontStyle: "italic",
                            textDecoration: "bold",
                            color: palette.JET
                        }} to="/register">Do not have an account? <u>Click here</u></Link>
                    </Paper>
                </form>
            }
        </>
    );
};

export {LoginForm};