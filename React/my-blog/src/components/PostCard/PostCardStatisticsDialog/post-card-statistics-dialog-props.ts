import { PostModel } from "../../../shared/api/types/post";

export interface PostCardStatisticsDialogProps {
    open: boolean;
    post: PostModel;
    close: () => void;
}