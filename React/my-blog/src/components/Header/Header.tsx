import { Avatar, Box, Button, Toolbar, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { ApplicationState, CurrentUserState } from '../../redux';
import { CustomNavbar } from '../CustomNavbar';
import { CustomNavLink } from '../CustomNavLink';
import { Link } from 'react-router-dom';
import { AccountMenuDropdown } from '../AccountMenuDropdown';
import styles from "./header.module.scss";
import { UserModel } from '../../shared/api/types/user';
import { UserApi } from '../../shared/api/http/user-api';

const Header = () => {
    const user = useSelector<ApplicationState, (CurrentUserState | undefined | null)>(state => state.user);
    const [userInfo, setUserInfo] = useState<UserModel>();
    const [avatarUrl, setAvatarUrl] = useState<string>();

    useEffect(() => {
        setUserInfo(undefined);
        setAvatarUrl(undefined);
        if (user) {
            UserApi.getUserById(user.id).then((response) => {
                setUserInfo(response.data);
                UserApi.getAvatarUrlById(user.id).then((response1) => {
                    let avatarUrl = response1.data;
                    setAvatarUrl(avatarUrl);
                });
            });
        }
    }, [user]);

    return (
        <CustomNavbar>
            <Toolbar>
                <Box className={styles["toolbar__buttons"]}>
                    <CustomNavLink to={"/"}>
                        Home
                    </CustomNavLink>
                </Box>
                {
                    user ?
                        <div>
                            <Typography display={"inline"}>Welcome, {userInfo?.username}!</Typography>
                            <AccountMenuDropdown icon={<Avatar
                                src={avatarUrl}>{userInfo?.initials}</Avatar>}/>
                        </div>
                        :
                        <Button variant="contained" component={Link} to="/login">Login</Button>
                }
            </Toolbar>
        </CustomNavbar>
    );
};

export {Header};