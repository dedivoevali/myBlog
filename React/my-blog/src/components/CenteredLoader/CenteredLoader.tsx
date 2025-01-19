import React from 'react';
import { Box, CircularProgress } from '@mui/material';
import styles from "./centered-loader.module.scss";
import { MUIColor } from '../../assets/mui-color';

export interface CenteredLoaderProps {
    color?: MUIColor;
    verticalMarginPx?: number;
    sizePx?: number;
}

const CenteredLoader = ({ color = "primary", verticalMarginPx = 50, sizePx = 40 }: CenteredLoaderProps) => {
    return (
        <Box
            className={styles["container"]}
            style={{ margin: `${verticalMarginPx}px auto`}}>
            <CircularProgress size={`${sizePx}px`} color={color}/>
        </Box>
    );
};

export {CenteredLoader};