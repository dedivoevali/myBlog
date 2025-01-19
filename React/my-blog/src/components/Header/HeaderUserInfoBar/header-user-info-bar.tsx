import { Avatar, Typography } from "@mui/material"
import React, { useEffect, useState } from "react"
import { AccountMenuDropdown } from "../../AccountMenuDropdown"
import { CurrentUserState } from "../../../redux";
import { UserApi } from "../../../shared/api/http/user-api";
import { UserBadgeModel } from "../../../shared/api/types/user";
import styles from "./header-user-info-bar.module.scss";
import { CenteredLoader } from "../../CenteredLoader";

export interface HeaderUserInfoBarProps {
    user: CurrentUserState
}

const HeaderUserInfoBar = ({ user }: HeaderUserInfoBarProps) => {
    const [userInfo, setUserInfo] = useState<UserBadgeModel>();
    const [avatarUrl, setAvatarUrl] = useState<string>();
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        setIsLoading(true);
        setUserInfo(undefined);
        setAvatarUrl(undefined);
        if (user) {
            UserApi.getCurrentUserBadge().then((response) => {
                setUserInfo(response.data);
                UserApi.getAvatarUrlById(user.id).then((response1) => {
                    const avatarUrl = response1.data;
                    setAvatarUrl(avatarUrl);
                    setIsLoading(false);
                });
            });
        }
    }, [user]);

    return <React.Fragment>
        {
            isLoading ?
            <CenteredLoader color="secondary" verticalMarginPx={10} sizePx={40}/> :
            <div className={styles["box"]}>
                <Typography className={styles["box__caption"]}>Welcome, {userInfo?.username}!</Typography>
                <AccountMenuDropdown icon={<Avatar src={avatarUrl}>{userInfo?.initials}</Avatar>} />
            </div>
        }
    </React.Fragment>

}

export { HeaderUserInfoBar }