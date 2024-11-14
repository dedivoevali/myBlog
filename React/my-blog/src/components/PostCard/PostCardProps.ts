import {PostModel} from "../../shared/api/types/post";

export interface PostCardProps {
    initialPost: PostModel,
    disappearPostCallback: () => void,
    width: string,
    commentPortionSize: number,
    enableCommentInfiniteScroll?: boolean
    redirectToAfterDelete?: string
}