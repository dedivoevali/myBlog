import { Box, Typography } from "@mui/material"
import WarningIcon from '@mui/icons-material/Warning';
import DoneIcon from '@mui/icons-material/Done';
import CancelRoundedIcon from '@mui/icons-material/CancelRounded';
import { WarningProfileProps } from "./WarningProfileProps";
import styles from "./warning-profile-box.module.scss";

const WarningProfileBox = ({ warnings, isBanned }: WarningProfileProps) => {
    return (
        <>
            {
                isBanned ?
                    <Box className={`${styles.container} ${styles.banned}`}>
                        <Typography>User is blocked!</Typography>
                        <CancelRoundedIcon />
                    </Box>
                    :
                    warnings.length === 0 ?
                        <Box className={`${styles.container} ${styles.clean}`}>
                            <Typography>No active warns!</Typography>
                            <DoneIcon />
                        </Box>
                        :
                        <Box className={`${styles.container} ${styles.warned}`}>
                            <Typography>{warnings.length}/3 active warns</Typography>
                            <WarningIcon />
                        </Box>
            }
        </>
    )
}

export { WarningProfileBox }