import { Avatar, Box, Button, Paper, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { ApplicationState } from '../../redux';
import { ProfileHeaderProps } from './ProfileHeaderProps';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import ScheduleIcon from '@mui/icons-material/Schedule';
import {
    transformUtcStringToDateMonthHoursMinutesString,
    transformUtcToLocalDate
} from '../../shared/assets';
import { EditProfileCustomModal } from "../CustomModal";
import { WarningProfileBox } from './WarningsProfileBox/WarningProfileBox';
import styles from './ProfileHeader.module.scss';
import { UserApi } from '../../shared/api/http/user-api';

const ProfileHeader = ({user, setUser}: ProfileHeaderProps) => {
    const isUserOnHisProfilePage = useSelector<ApplicationState, boolean>(state => state.user !== undefined && state.user?.id === user.id);
    const [editProfileModalOpen, setEditProfileModalOpen] = useState<boolean>(false);
    const [avatarLink, setAvatarLink] = useState<string>("");
    const fetchAvatarUrl = (userId: number) => UserApi.getAvatarUrlById(userId).then(response => setAvatarLink(response.data));

    useEffect(() => {
        if (user) {
            fetchAvatarUrl(user.id);
        }
    }, []);

    return (
        <>
            <EditProfileCustomModal modalOpen={editProfileModalOpen} setModalOpen={setEditProfileModalOpen}
                                    user={user}
                                    setUser={setUser}/>

            <Paper className={styles.wrapper}>
                <Paper elevation={0} className={styles['upper-background']}/>

                <Box style={{
                    margin: "0 32px",
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center"
                }}>
                    <div>
                        <Avatar className={styles.avatar} src={avatarLink}>{user.initials}</Avatar>
                    </div>

                    <div>
                        {isUserOnHisProfilePage &&
                            <Button variant="outlined" onClick={() => setEditProfileModalOpen(true)}>Edit
                                profile</Button>}
                    </div>
                </Box>

                <Box style={{
                    margin: "0 32px",
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "self-start",
                    flexDirection: "column"
                }}>

                    <Typography variant="h2">{user.username}</Typography>

                    {user.fullName.length !== 0 && <Typography variant="h6">Name: {user.fullName}</Typography>}

                    <Box style={{margin: "32px 0 0 0", display: "flex"}}>

                        <Box style={{margin: "0 32px 0 0"}}>
                            <CalendarMonthIcon/>
                            <Typography>
                                Joined on {transformUtcToLocalDate(user.registrationDate)}
                            </Typography>
                        </Box>

                        <Box>
                            <ScheduleIcon/>
                            <Typography>
                                Last activity {transformUtcStringToDateMonthHoursMinutesString(user.lastActivity)}
                            </Typography>
                        </Box>

                    </Box>
                    <WarningProfileBox isBanned={user.isBanned} warnings={user.activeWarnings}/>
                </Box>
            </Paper>
        </>
    );
};

export {ProfileHeader};