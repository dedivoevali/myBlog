import { useEffect, useState } from "react";
import { PasskeyListModel } from "../../shared/api/types/authentication/passkey/passkey-info-model";
import { CenteredLoader } from "../CenteredLoader";
import { VpnKey, Delete } from "@mui/icons-material";
import styles from "./passkey-list.module.scss";
import { dateTimeFormats } from "../../shared/assets/dateTimeUtils";
import dayjs from "dayjs";
import { UserApi } from "../../shared/api/http/user-api";
import { IconButton } from "@mui/material";
import { passkeyApi } from "../../shared/api/http/passkey-api";
import { useNotifier } from "../../hooks";

const PasskeyList = () => {
    const [isLoading, setLoading] = useState<boolean>(true);
    const [passkeys, setPasskeys] = useState<PasskeyListModel>();
    const displayNotification = useNotifier();

    useEffect(() => {
        UserApi.getPasskeysInfoByCurrentUserId().then((model) => {
            setPasskeys(model.data);
            setLoading(false);
        })
    }, []);

    const handleDeletePasskey = (id: number) => {
        passkeyApi.deactivate(id).then(() => {
            displayNotification("Passkey deleted", "success");
            setPasskeys({
                passkeys: passkeys?.passkeys.filter(p => p.id !== id) || []
            });
        }).catch((resp) => {
            displayNotification(resp.response.data.Message, "error");
        })
    }

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
                    <IconButton className={styles[`passkey-info__delete`]} color="error" onClick={() => handleDeletePasskey(p.id)}>
                        <Delete />
                    </IconButton>
                </div>)
            }
        </div>
    )
}

export { PasskeyList };