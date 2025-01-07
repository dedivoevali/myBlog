import { AxiosResponse, HttpStatusCode } from "axios";
import { instance } from "./api";
import { UserInfoDto, UserModel } from "../types/user";
import { PasskeyListModel } from "../types/authentication/passkey";

export class UserApi {
    static getAvatarUrlById(userId: number): Promise<AxiosResponse<string>> {
        return instance.get(`/avatars/${userId}`, {
            validateStatus: (status) => [HttpStatusCode.Ok, HttpStatusCode.NotFound].includes(status)
        });
    }

    static getUserById(userId: number): Promise<AxiosResponse<UserModel>> {
        return instance.get(`/users/${userId}`)
    }

    static editProfileOfAuthorizedUser(dto: UserInfoDto): Promise<AxiosResponse<UserModel>> {
        return instance.patch(`/users/`, dto);
    }

    static getPasskeysInfoByCurrentUserId(): Promise<AxiosResponse<PasskeyListModel>> {
        return instance.get(`/users/current/passkeys`);
    }
}