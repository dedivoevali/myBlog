import {Logout} from '@mui/icons-material';
import {IconButton, ListItemIcon, Menu, MenuItem, Tooltip, Typography} from '@mui/material';
import PersonIcon from '@mui/icons-material/Person';
import React, {useState} from 'react';
import {authApi} from '../../shared/api/http/api';
import {ReduxActionTypes} from '../../redux';
import {useDispatch} from 'react-redux';
import {ConfirmActionCustomModal} from '../CustomModal';
import {Link} from 'react-router-dom';
import {palette} from '../../shared/assets';
import {useNotifier} from '../../hooks';

const AccountMenuDropdown = ({icon}: { icon: JSX.Element }) => {

    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>();
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const isDropdownOpened = Boolean(anchorEl);
    const notifyUser = useNotifier();

    const dispatch = useDispatch();


    const deleteUserFromRedux = () => {
        dispatch({type: ReduxActionTypes.DeleteUser });
    }

    const handleLogout = () => {
        authApi.logout();
        deleteUserFromRedux();
        notifyUser("Logged out successfully", "success");
    }

    const openLogoutModal = () => setModalOpen(true);

    const handleDropdownTriggerClick = (event: React.MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget);

    const handleDropdownClose = () => setAnchorEl(null);

    return (
        <>
            <ConfirmActionCustomModal actionCallback={handleLogout} title={"Logout"}
                                      caption={"Are you sure you want to log out?"} modalOpen={modalOpen}
                                      setModalOpen={setModalOpen}/>

            <Tooltip title="Account settings">
                <IconButton
                    onClick={handleDropdownTriggerClick}
                    size="medium"
                    sx={{ml: 2}}
                    aria-controls={isDropdownOpened ? "account-menu" : undefined}
                    aria-haspopup="true"
                    aria-expanded={isDropdownOpened ? 'true' : undefined}>
                    {icon}
                </IconButton>
            </Tooltip>

            <Menu anchorEl={anchorEl} id="account-menu" open={isDropdownOpened} onClick={handleDropdownClose}
                  onClose={handleDropdownClose}
                  PaperProps={{
                      elevation: 0, sx: {
                          overflow: 'visible',
                          filter: 'drop-shadow(0px 2px 8px rgba(0,0,0,0.50))',
                      }
                  }}
                  transformOrigin={{horizontal: "right", vertical: "top"}}
                  anchorOrigin={{horizontal: "right", vertical: "bottom"}}
            >

                <Link to="/me">
                    <MenuItem>
                        <ListItemIcon>
                            <PersonIcon/>
                        </ListItemIcon>
                        <Typography style={{textDecoration: "none", color: palette.JET}}>Personal page</Typography>
                    </MenuItem>
                </Link>

                <MenuItem onClick={openLogoutModal}>
                    <ListItemIcon>
                        <Logout/>
                    </ListItemIcon>
                    Logout
                </MenuItem>
            </Menu>
        </>
    );
};

export {AccountMenuDropdown};