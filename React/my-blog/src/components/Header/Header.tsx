import {Avatar, Box, Button, Toolbar, Typography} from '@mui/material';
import React, {useEffect} from 'react';
import {useSelector} from 'react-redux';
import {ApplicationState} from '../../redux';
import {CustomNavbar} from '../CustomNavbar';
import {CustomNavLink} from '../CustomNavLink';
import {Link} from 'react-router-dom';
import {AccountMenuDropdown} from '../AccountMenuDropdown';
import {UserInfoCache} from "../../shared/types";


const Header = () => {

    const user = useSelector<ApplicationState, (UserInfoCache | null)>(state => state.user);


    useEffect(() => {
    }, [user]);

    return (
        <CustomNavbar>
            <Toolbar>

                <Box sx={{flexGrow: 1}} style={{display: "flex", justifyContent: "flex-start"}}>

                    <CustomNavLink to={"/"}>
                        Home
                    </CustomNavLink>

                </Box>

                {
                    user ?
                        <div>
                            <Typography display={"inline"}>Welcome, {user?.username}!</Typography>
                            <AccountMenuDropdown icon={<Avatar
                                src={user?.avatar}>{user?.username[0]}</Avatar>}></AccountMenuDropdown>
                        </div>
                        :
                        <Button variant="contained" component={Link} to="/login">Login</Button>
                }
            </Toolbar>
        </CustomNavbar>
    );
};

export {Header};