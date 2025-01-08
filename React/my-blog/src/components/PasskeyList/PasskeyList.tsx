import { useEffect, useState } from "react";
import { PasskeyListModel } from "../../shared/api/types/authentication/passkey/passkey-info-model";
import { CenteredLoader } from "../CenteredLoader";
import { VpnKey } from "@mui/icons-material";
import styles from "./passkey-list.module.scss";
import { dateTimeFormats } from "../../shared/assets/dateTimeUtils";
import dayjs from "dayjs";
import { UserApi } from "../../shared/api/http/user-api";

const PasskeyList = () => {
    const [isLoading, setLoading] = useState<boolean>(true);
    const [passkeys, setPasskeys] = useState<PasskeyListModel>();

    useEffect(() => {
        UserApi.getPasskeysInfoByCurrentUserId().then((model) => {
            setPasskeys(model.data);
            setLoading(false);
        })
    }, []);

    return (
        isLoading ? <CenteredLoader /> : <div>
            {
                passkeys?.passkeys.map(p => 
                <div
                    key={`passkey-info-${p.id}`}
                    id={`passkey-info-${p.id}`}
                    className={styles[`passkey-info__item`]}>
                    <VpnKey />
                    <span className={styles[`passkey-info__caption`]}>{p.name + ' ' + dayjs(p.registrationDate).format(dateTimeFormats.SIMPLE_WITH_TIME)}</span>
                </div>)
            }
        </div>
    )
}

export { PasskeyList };