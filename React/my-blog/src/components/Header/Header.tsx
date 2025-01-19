import { Box, Button, Toolbar } from '@mui/material';
import { useSelector } from 'react-redux';
import { ApplicationState, CurrentUserState } from '../../redux';
import { CustomNavbar } from '../CustomNavbar';
import { CustomNavLink } from '../CustomNavLink';
import { Link } from 'react-router-dom';
import styles from "./header.module.scss";
import { HeaderUserInfoBar } from './HeaderUserInfoBar';

const Header = () => {
    const user = useSelector<ApplicationState, (CurrentUserState | undefined | null)>(state => state.user);
    console.log(user);

    return (
        <CustomNavbar>
            <Toolbar>
                <Box className={styles["toolbar__buttons"]}>
                    <CustomNavLink to={"/"}>
                        Home
                    </CustomNavLink>
                </Box>
                {
                    user ? <HeaderUserInfoBar user={user}/> : <Button variant="contained" component={Link} to="/login">Login</Button>
                }
            </Toolbar>
        </CustomNavbar>
    );
};

export {Header};