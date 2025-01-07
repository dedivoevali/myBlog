import axios, { AxiosError, AxiosResponse, HttpStatusCode } from "axios";
import { API_URL, JwtTokenKeyName, UserIdTokenKeyName } from "../../config"
import { AuthenticateRequest, AuthenticateResponse, RegistrationDto } from "../types"
import { CursorPagedRequest } from "../types/paging/cursorPaging";
import { PostDto, PostModel } from "../types/post";
import { PostReactionDto } from "../types/postReaction";
import { CommentDto } from "../types/comment";

const IMMEDIATE_LOGOUT_STATUSES = [ HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden ];

export const instance = axios.create({
    withCredentials: false,
    baseURL: API_URL
});

instance.interceptors.request.use((config) => {

    if (config && config.headers)
        config.headers["Authorization"] = localStorage.getItem(JwtTokenKeyName) || sessionStorage.getItem(JwtTokenKeyName)
            ? `Bearer ${localStorage.getItem(JwtTokenKeyName) ?? sessionStorage.getItem(JwtTokenKeyName)}`
            : undefined

    return config;
})

instance.interceptors.response.use((response) => response,
(err) => {
    if (IMMEDIATE_LOGOUT_STATUSES.includes(err.status)) {
        localStorage.clear();
        sessionStorage.clear();
        window.location.reload();
    }
    throw err;
})

export class avatarApi {

    static RemoveAvatarForAuthorizedUser() {
        return instance.delete(`/avatars/`);
    }

    static UploadNewAvatarForAuthorizedUser(image: File) {
        const formData = new FormData();
        formData.append("image", image);

        return instance.post(`/avatars`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    }
}

export class authApi {

    static async getCurrent() {
        return await instance.get(`/users/current`);
    }

    static logout(): Promise<void> {
        const claimNames = [JwtTokenKeyName, UserIdTokenKeyName];
        return instance.post(`auth/logout`).then((_) => {
            claimNames.forEach(claim => {
                localStorage.removeItem(claim);
                sessionStorage.removeItem(claim);
            });
        });
    }

    static tryAuthenticate(credentials: AuthenticateRequest, useLocalStorage: boolean): Promise<AxiosResponse<AuthenticateResponse>> {
        return instance.post(`auth/login`, credentials).then((response: AxiosResponse<AuthenticateResponse>) => {
            const payload = response.data;
            this.setUserInStorage(payload, useLocalStorage);
            return response;
        }).catch(reason => reason);
    }

    static TryRegister(dto: RegistrationDto) {

        if (dto.firstName === "") {
            dto.firstName = null;
        }

        if (dto.lastName === "") {
            dto.lastName = null;
        }

        return instance.post(`/register`, dto)
            .then((response) => {
                return response;
            })
            .catch((reason) => reason as AxiosError);
    }

    public static setUserInStorage(payload: AuthenticateResponse, useLocalStorage: boolean): void {
        let storage: Storage = useLocalStorage ? localStorage : sessionStorage;
        storage.setItem(JwtTokenKeyName, payload.accessToken);
        storage.setItem(UserIdTokenKeyName, payload.userId.toString());
    }
}

export class postApi {
    static getCursorPagedPosts(request: CursorPagedRequest) {
        return instance.post(`/posts/paginated-search-cursor`, request);
    }

    static getPostById(id: number) {
        return instance.get(`/posts/${id}`);
    }

    static addPost(post: PostDto) {
        return instance.post<PostModel>(`/posts`, post);
    }

    static editPost(postId: number, post: PostDto) {
        return instance.put<PostModel>(`/posts/${postId}`, post);
    }

    static removePostId(postId: number) {
        return instance.delete(`/posts/${postId}`);
    }
}

export class postReactionApi {
    static getReactionsByPost(postId: number) {
        return instance.get(`/reactions/${postId}`);
    }

    static addReactionToPost(request: PostReactionDto) {
        return instance.post(`/reactions`, request);
    }

    static removeReactionFromPost(postId: number) {
        return instance.delete(`/reactions/${postId}`);
    }

    static updateReactionOnPost(request: PostReactionDto) {
        return instance.put(`/reactions`, request);
    }

}

export class commentApi {
    static getCursorPagedComments(request: CursorPagedRequest) {
        return instance.post(`comments/paginated-search-cursor`, request);
    }

    static addComment(request: CommentDto) {
        return instance.post(`/comments`, request);
    }

    static editComment(commentId: number, request: CommentDto) {
        return instance.put(`/comments/${commentId}`, request);
    }

    static removeComment(commentId: number) {
        return instance.delete(`/comments/${commentId}`);
    }
}