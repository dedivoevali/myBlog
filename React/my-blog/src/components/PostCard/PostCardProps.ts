import {PostModel} from "../../shared/api/types/post";

// TODO: why this component has disappear callback && redirectToAfterDelete
export interface PostCardProps {
    initialPost: PostModel,
    disappearPostCallback: () => void,
    width: string,
    commentPortionSize: number,
    enableCommentInfiniteScroll?: boolean
    redirectToAfterDelete?: string
}