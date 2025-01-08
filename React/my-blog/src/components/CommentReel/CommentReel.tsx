import React, {useEffect, useState} from 'react';
import {CommentReelProps} from "./CommentReelProps";
import {CommentDto, CommentModel} from "../../shared/api/types/comment";
import {CursorPagedRequest, CursorPagedResult} from "../../shared/api/types/paging/cursorPaging";
import {commentApi} from "../../shared/api/http/api";
import {AxiosResponse} from "axios";
import {Box, Button, CircularProgress, IconButton, Typography} from '@mui/material';
import {CommentCard} from "../CommentCard";
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
import {useSelector} from 'react-redux';
import {ApplicationState, CurrentUserState} from "../../redux";
import {CommentForm} from "../CommentForm";
import {EmptyReelPlate} from '../EmptyReelPlate';
import {FilterLogicalOperator} from "../../shared/api/types/paging";
import {DefaultPageSize} from "../../shared/config";
import ArrowUpwardIcon from "@mui/icons-material/ArrowUpward";
import {Waypoint} from "react-waypoint";
import {CenteredLoader} from "../CenteredLoader";


const CommentReel = ({
                         reelWidth = "100%",
                         pagingRequestDefault = {
                             requestFilters: {
                                 logicalOperator: FilterLogicalOperator.Or,
                                 filters: []
                             }, pageSize: DefaultPageSize, getNewer: false
                         },
                         post,
                         enableInfiniteScroll = false,
                         onCommentAdded,
                         onCommentDeleted
                     }: CommentReelProps) => {


    const user = useSelector<ApplicationState, (CurrentUserState | undefined | null)>(state => state.user);

    const [comments, setComments] = useState<CommentModel[]>([]);
    const [isLoading, setLoading] = useState<boolean>(true);
    const [pagingRequest, setPagingRequest] = useState<CursorPagedRequest>(pagingRequestDefault);
    const [noMoreComments, setNoMoreComments] = useState<boolean>(false);
    const [newCommentFormEnabled, setNewCommentForm] = useState<boolean>(true);


    const handleOpenNewCommentForm = () => setNewCommentForm(true);

    const handleCloseNewCommentForm = () => setNewCommentForm(false);

    const fetchComments = (pagingRequest: CursorPagedRequest) =>
        commentApi.getCursorPagedComments(pagingRequest).then((result: AxiosResponse<CursorPagedResult<CommentModel>>) => result.data);

    const handleDeleteComment = (commentToDeleteId: number) => {
        setComments(comments.filter((comment) => comment.id !== commentToDeleteId))
        onCommentDeleted();
    };


    const loadMoreComments = () => {
        if (noMoreComments) {
            return;
        }

        setLoading(true);
        fetchComments(pagingRequest).then((result) => {

            setComments([...comments, ...result.items]);
            setPagingRequest({...pagingRequest, pivotElementId: result.tailElementId});

            if (result.items.length < pagingRequest.pageSize) {
                setNoMoreComments(true);
            }

            setLoading(false);
        });
    }

    useEffect(() => {
        loadMoreComments();
    }, []);

    const handleAddNewComment = async (comment: CommentDto) => {
        setLoading(true);
        return commentApi.addComment(comment).then((result: AxiosResponse<CommentModel>) => {
            if (result.status === 200 && user) {
                setComments([result.data, ...comments]);
            }
            setLoading(false);
            onCommentAdded();
            return result;
        }).catch(result => result);
    }


    return (
        <>
            {user && post && (newCommentFormEnabled ?
                    <CommentForm caption="New Comment" formActionCallback={handleAddNewComment}
                                 formCloseHandler={handleCloseNewCommentForm} post={post}/>
                    :
                    <Box width={"100%"} style={{margin: "0 auto", width: "fit-content"}}>
                        <Button variant="outlined" onClick={handleOpenNewCommentForm}>Add new comment</Button>
                    </Box>
            )}
            {isLoading && comments.length === 0
                ?
                <Box style={{margin: "50px auto", width: "fit-content"}}>
                    <CircularProgress/>
                </Box>
                :
                <div>
                    {comments.length === 0
                        ?
                        <EmptyReelPlate width={reelWidth}/>
                        :
                        <>
                            {comments.map((comment) => <CommentCard key={comment.id} initialComment={comment}
                                                                    width={"100%"}
                                                                    disappearCommentCallback={() => handleDeleteComment(comment.id)}/>)}

                            {enableInfiniteScroll &&
                                <Waypoint bottomOffset="-700px" onEnter={() => !noMoreComments && loadMoreComments()}/>}

                            {isLoading && <CenteredLoader/>}

                            {
                                enableInfiniteScroll
                                    ?
                                    (noMoreComments && <Box style={{margin: "50px auto", width: "fit-content"}}>
                                            <IconButton children={<ArrowUpwardIcon fontSize={"large"}/>}
                                                        style={{margin: "0 auto", display: "block"}}
                                                        onClick={() => window.scrollTo({top: 0, behavior: "smooth"})}/>
                                            <Typography textAlign={"center"}>
                                                Oops.. Looks like there is nothing for you to show<br/>
                                                Press the arrow button to scroll to the top
                                            </Typography>
                                        </Box>
                                    )
                                    :
                                    (!noMoreComments &&
                                        <div style={{display: "flex", alignItems: "flex-start"}}>
                                            <IconButton onClick={() => loadMoreComments()}>
                                                <ArrowDownwardIcon fontSize={"large"}/>
                                            </IconButton>
                                            <Typography style={{margin: "auto 0"}}>Please,push the arrow to load more
                                                comments</Typography>
                                        </div>)
                            }
                        </>
                    }
                </div>
            }
        </>)
};

export {CommentReel};