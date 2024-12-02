import React, { ChangeEvent, useEffect, useState } from 'react';
import { CustomModal } from '../CustomModal';
import { EditProfileCustomModalProps } from "./EditProfileCustomModalProps";
import {
    Avatar,
    Box,
    Button,
    DialogActions,
    DialogContent,
    FormControl,
    FormHelperText,
    Input,
    InputLabel
} from "@mui/material";
import * as Yup from "yup";
import { useFormik } from "formik";
import { UserInfoDto, UserModel } from "../../../shared/api/types/user";
import {
    FirstnameLastnameConstraints,
    palette,
    UsernameValidationConstraints
} from "../../../shared/assets";
import { FormHeader } from '../../FormHeader';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import { avatarApi, userApi } from '../../../shared/api/http/api';
import { AxiosError, AxiosResponse } from "axios";
import { useNotifier } from '../../../hooks';
import { ErrorResponse } from "../../../shared/api/types";
import UploadFileRoundedIcon from '@mui/icons-material/UploadFileRounded';
import CancelRoundedIcon from '@mui/icons-material/CancelRounded';
import { ApplicationState, ReduxActionTypes } from '../../../redux';
import { useDispatch, useSelector } from 'react-redux';
import { UserInfoCache } from '../../../shared/types';
import { MaxAvatarSizeBytes } from "../../../shared/config";
import styles from './edit-profile-custom-modal.module.scss';
import VpnKeyIcon from '@mui/icons-material/VpnKey';
import { RegisterPasskeyButton } from '../../RegisterPasskeyButton';
import { PasskeyList } from '../../PasskeyList/PasskeyList';

const EditProfileCustomModal = ({modalOpen, setModalOpen, user, setUser}: EditProfileCustomModalProps) => {

    const reduxUser = useSelector<ApplicationState, (UserInfoCache | null)>(state => state.user);

    const dispatch = useDispatch();

    const setReduxUserInfo = (userInfo: UserModel) => {
        const cache = new UserInfoCache(userInfo.id, userInfo.username, reduxUser?.avatar || "");
        dispatch({type: ReduxActionTypes.ChangeUser, payload: cache})
    }

    const setReduxAvatar = (avatar: string) => {
        if (reduxUser) {
            const cache = new UserInfoCache(reduxUser.id, reduxUser.username, avatar);

            dispatch({type: ReduxActionTypes.ChangeUser, payload: cache});
        }
    }

    const notifyUser = useNotifier();

    const formik = useFormik<UserInfoDto>({
        initialValues: {
            username: user.username,
            firstName: user.fullName.split(' ')[0],
            lastName: user.fullName.split(' ')[1]
        },
        onSubmit: (values, formikHelpers) => {

            if (values.username === user.username) {
                values.username = undefined;
            }

            if ((!values.firstName && !values.lastName) || (values.firstName?.length === 0 && values.lastName?.length === 0)) {
                values.firstName = undefined;
                values.lastName = undefined;
            }


            userApi.editProfileOfAuthorizedUser(values).then((response) => {
                setUser({
                    ...response.data,
                    lastActivity: new Date().toUTCString(),
                    fullName: `${values.firstName} ${values.lastName}`,
                    username: values.username || user.username
                });

                setReduxUserInfo({...response.data});

                notifyUser("User information was successfully updated", "success")
                setModalOpen(false);
                formikHelpers.resetForm();

            }).catch((result: AxiosError<ErrorResponse>) => {
                notifyUser(result.response?.data.Message || "Unknown error", "error");
            });
        },
        validationSchema: Yup.object({
            username: Yup.string()
                .min(UsernameValidationConstraints.MinLength)
                .max(UsernameValidationConstraints.MaxLength)
                .matches(UsernameValidationConstraints.Regexp),
            firstName: Yup.string()
                .matches(FirstnameLastnameConstraints.Regexp, "First name shall be a single word and begin with capital letter!")
                .min(FirstnameLastnameConstraints.MinLength)
                .max(FirstnameLastnameConstraints.MaxLength)
                .nullable(),
            lastName: Yup.string()
                .matches(FirstnameLastnameConstraints.Regexp, "Last name shall be a single word and begin with capital letter!")
                .min(FirstnameLastnameConstraints.MinLength)
                .max(FirstnameLastnameConstraints.MaxLength)
                .nullable()

        })
    });

    const fetchAvatarUrl = (userId: number) => userApi.getAvatarUrlById(userId).then((result: AxiosResponse<string>) => result.data);

    const [avatarPreview, setAvatarPreview] = useState<string>("");
    const [avatarFile, setAvatarFile] = useState<File | null>(null);
    const [passkeyListUpdateTrigger, setPasskeyListUpdateTrigger] = useState<number>(0);

    const handleFile = (e: ChangeEvent<HTMLInputElement>) => {


        if (e.target.files) {
            let file: File = e.target.files[0];

            if (file.size > MaxAvatarSizeBytes) {
                notifyUser(`Maximum avatar size is ${MaxAvatarSizeBytes / 1024}Kb. Please pick smaller one`, "warning");
                e.target.value = "";
                return;
            }

            setAvatarFile(file);
        }
    }

    const handleUpload = async () => {

        if (avatarFile) {
            avatarApi.UploadNewAvatarForAuthorizedUser(avatarFile).then((response) => {
                notifyUser("Avatar was successfully loaded", "success");
                setReduxAvatar(response.data);
            }).catch((error: AxiosError<ErrorResponse>) => {
                notifyUser(error.response?.data.Message || "Error occurred while uploading avatar", "error");
            });

        } else {
            notifyUser("Please select image.", "info");
        }
    }

    const handleDeleteAvatar = () => {
        avatarApi.RemoveAvatarForAuthorizedUser().then(() => {
            notifyUser("Avatar was successfully removed", "success");
            setAvatarPreview("");
            setReduxAvatar("");
        }).catch((result: AxiosError<ErrorResponse>) => {
            notifyUser(result.response?.data.Message || "Unknown error", "error");
        });
    }

    useEffect(() => {
        fetchAvatarUrl(user.id).then((result) => setAvatarPreview((result)));
    }, []);


    useEffect(() => {
        if (avatarFile) {
            const avatarPreviewUrl = URL.createObjectURL(avatarFile);

            setAvatarPreview(avatarPreviewUrl);
        }
    }, [avatarFile]);

    return (
        <CustomModal modalOpen={modalOpen} setModalOpen={setModalOpen}>
            <form className={styles["edit-profile-form"]}
                  onSubmit={formik.handleSubmit}>

                <DialogContent>

                    <Box className={styles["avatar-section"]}>

                        <Avatar
                            sx={{minHeight: "128px", minWidth: "128px", width: "2vw", height: "2vw", fontSize: "64px"}}
                            style={{margin: "0 auto"}}
                            src={avatarPreview?.toString()}>{user.initials}</Avatar>

                        <input name={"avatar"} accept={"image/png,image/jpeg"} id={"contained-button-file"}
                               type={"file"} onChange={handleFile}/>

                        <Button onClick={handleUpload} color={"primary"} variant={"contained"}
                                startIcon={<UploadFileRoundedIcon/>}>Upload new avatar</Button>

                        <Button disabled={!user.hasAvatar} onClick={handleDeleteAvatar} color={"error"} variant={"contained"}
                                startIcon={<CancelRoundedIcon/>}>Remove Avatar</Button>


                    </Box>

                    <Box className={styles["profile-section__wrapper"]}>

                        <FormHeader className={styles["profile-section__header"]}
                            iconColor={palette.BAYERN_BLUE} caption={"Edit profile information"}
                            icon={<AccountBoxIcon/>}/>

                        <FormControl className={styles["form-field"]}>
                            <InputLabel htmlFor="username">Username</InputLabel>
                            <Input onChange={formik.handleChange} value={formik.values.username} name="username"/>
                            <FormHelperText>
                                {formik.touched.username && formik.errors.username && (
                                    <span className={styles.error}>{formik.errors.username}</span>)}
                            </FormHelperText>
                        </FormControl>

                        <FormControl className={styles["form-field"]}>
                            <InputLabel htmlFor="firstName">First name</InputLabel>
                            <Input onChange={formik.handleChange} value={formik.values.firstName} name="firstName"/>
                            <FormHelperText>
                                {formik.touched.firstName && formik.errors.firstName && (
                                    <span className={styles.error}>{formik.errors.firstName}</span>)}
                            </FormHelperText>
                        </FormControl>

                        <FormControl className={styles["form-field"]}>
                            <InputLabel htmlFor="lastName">Last name</InputLabel>
                            <Input onChange={formik.handleChange} value={formik.values.lastName} name="lastName"/>
                            <FormHelperText>
                                {formik.touched.lastName && formik.errors.lastName && (
                                    <span className={styles.error}>{formik.errors.lastName}</span>)}
                            </FormHelperText>
                        </FormControl>
                    </Box>

                    <Box className={styles["security"]}>
                        <FormHeader iconColor={palette.SUNRISE} caption="Security" icon={<VpnKeyIcon/>}/>
                        <PasskeyList key={passkeyListUpdateTrigger}/>
                        <RegisterPasskeyButton caption="ADD PASSKEY" onSuccess={() => setPasskeyListUpdateTrigger(passkeyListUpdateTrigger + 1)}/>
                    </Box>
                </DialogContent>

                <DialogActions>
                    <Button disabled={JSON.stringify(formik.values) === JSON.stringify(formik.initialValues)}
                            type={"submit"}>Update</Button>
                    <Button onClick={() => setModalOpen(false)}>Go back</Button>
                </DialogActions>
            </form>
        </CustomModal>
    );
};

export {EditProfileCustomModal};