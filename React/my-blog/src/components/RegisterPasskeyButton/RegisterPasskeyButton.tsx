import { Button } from "@mui/material"
import { PasskeyApi } from "../../shared/api/http/passkey-api";
import { WebauthnService } from "../../shared/services/webauthn-service";
import { ApplicationState, CurrentUserState } from "../../redux";
import { useSelector } from "react-redux";
import { RegisterPasskeyButtonProps } from "./RegisterPasskeyButtonProps";
import { VpnKeySharp } from "@mui/icons-material";
import styles from './register-passkey-button.module.scss';
import { IPasskeyRegistrationRequest } from "../../shared/api/types/authentication/passkey/passkey-registration-request";
import { arrayBufferToBase64 } from "../../shared/assets/array-buffer-utils";
import { useNotifier } from "../../hooks";
import { useState } from "react";

const RegisterPasskeyButton = ({ caption, onSuccess }: RegisterPasskeyButtonProps) => {

    const passkeyApi = PasskeyApi.create();
    const webauthnService = new WebauthnService(navigator, window);
    const user = useSelector<ApplicationState, (CurrentUserState | undefined | null)>(state => state.user);
    const notifyUser = useNotifier();
    const [loading, setLoading] = useState(false);
    const onClick = () => {
        setLoading(true);
        passkeyApi.getRegistrationOptions().then(result => {
            webauthnService.generateCredential(result).then((credential: any) => {
                const request: IPasskeyRegistrationRequest = {
                    id: credential.id,
                    rawId: arrayBufferToBase64(credential.rawId),
                    clientDataJson: arrayBufferToBase64(credential.response.clientDataJSON),
                    attestationObject: arrayBufferToBase64(credential.response.attestationObject),
                    type: credential.type  
                };
                passkeyApi.register(request)
                .then(() => {
                    notifyUser("Passkey created successfully!", "success");
                    onSuccess();
                })
                .catch((result) => notifyUser(result.response?.data.Message, "error"));
            }).catch((err) => {
                notifyUser("Passkey authentication aborted", "info");
            }).finally(() => {
                setLoading(false);
            })
        })
    };

    return (
        <div className={styles["passkey-button-wrapper"]}>
            {
                user ? <Button variant="contained" disabled={loading} onClick={onClick}>{
                    <div className={styles["passkey-button-content"]}>
                        <VpnKeySharp />
                        <span>{caption}</span>
                    </div> 
                }</Button> : <></>}
        </div>
    )
}

export { RegisterPasskeyButton };