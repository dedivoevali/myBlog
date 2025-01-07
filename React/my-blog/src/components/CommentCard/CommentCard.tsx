import React, {useEffect, useState} from 'react';
import {CommentCardProps} from "./CommentCardProps";
import {transformUtcStringToDateMonthHoursMinutesString} from "../../shared/assets";
import {Avatar, Box, Card, CardContent, CardHeader, IconButton, Menu, MenuItem, Typography} from '@mui/material';
import { commentApi } from "../../shared/api/http/api";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import {PostMentioningInline} from "../PostMentioningInline";
import {UserMentioningInline} from "../UserMentioningInline";
import {useSelector} from "react-redux";
import {ApplicationState, CurrentUserState} from '../../redux';
import {CommentDto, CommentModel} from '../../shared/api/types/comment';
import {AxiosResponse} from 'axios';
import {ConfirmActionCustomModal} from "../CustomModal";
import {CommentForm} from "../CommentForm";
import { UserApi } from '../../shared/api/http/user-api';

const CommentCard = ({width = "100%", initialComment, disappearCommentCallback, post}: CommentCardProps) => {

    const [comment, setComment] = useState<CommentModel>(initialComment);
    const user = useSelector<ApplicationState, (CurrentUserState | undefined | null)>(state => state.user);
    const [avatarLink, setAvatarLink] = useState("");
    const [confirmDeleteDialogOpen, setConfirmDeleteDialogOpen] = useState<boolean>(false);
    const [editPostModeEnabled, setEditPostMode] = useState<boolean>(false);

    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);


    const handleOpenMenu = (e: React.MouseEvent<HTMLButtonElement>) => setAnchorEl(e.currentTarget);

    const closeMenu = () => setAnchorEl(null);

    const openDeleteCommentDialog = () => setConfirmDeleteDialogOpen(true);

    const openEditWindow = () => setEditPostMode(true);

    const handleCloseEditWindow = () => setEditPostMode(false);

    const handleEditComment = async (commentDto: CommentDto): Promise<AxiosResponse<CommentModel>> => {
        return commentApi.editComment(comment.id, commentDto).then((result: AxiosResponse<CommentModel>) => {
            if (result.status === 200 && user) {
                setComment({...comment, content: result.data.content});
                handleCloseEditWindow();
            }
            closeMenu();
            return result;
        }).catch(result => result);
    }

    const handleDeleteComment = (commentId: number) => {
        return commentApi.removeComment(commentId).then((result: AxiosResponse) => {
            if (result.status === 200 && user) {
                disappearCommentCallback();
            }
        })
    }

    const fetchAvatarUrl = (userId: number) => UserApi.getAvatarUrlById(userId).then(response => setAvatarLink(response.data));

    useEffect(() => {
        fetchAvatarUrl(comment.authorId);
    }, []);


    return (
        <>
            {
                editPostModeEnabled ?
                    <CommentForm formActionCallback={handleEditComment} formCloseHandler={openEditWindow}
                                 post={post} initialComment={comment}/>
                    :
                    <Card elevation={10} style={{width: width, margin: "20px auto", minHeight: "fit-content"}}>
                        <>
                            {
                                user &&
                                <Menu anchorEl={anchorEl} open={open} onClose={closeMenu}>
                                    <MenuItem onClick={openEditWindow}>Edit comment</MenuItem>
                                    <MenuItem onClick={openDeleteCommentDialog}>Remove comment</MenuItem>
                                    <MenuItem onClick={closeMenu}>Close</MenuItem>
                                </Menu>
                            }

                            {
                                user && confirmDeleteDialogOpen &&
                                <ConfirmActionCustomModal actionCallback={() => handleDeleteComment(comment.id)}
                                                          caption={"Are you sure you want to delete this comment?"}
                                                          modalOpen={confirmDeleteDialogOpen}
                                                          setModalOpen={setConfirmDeleteDialogOpen}/>
                            }

                            <CardHeader
                                avatar={<Avatar
                                    src={avatarLink}>{comment.authorInitials}</Avatar>}
                                title={
                                    <Typography style={{display: "inline"}}>
                                        <UserMentioningInline userId={comment.authorId}
                                                              username={comment.authorUsername}/>
                                        <span style={{fontStyle: "italic"}}>{` replied to `}</span>
                                        <PostMentioningInline postId={comment.postId} postTitle={comment.postTitle}/>
                                    </Typography>}
                                action={user && user?.id === comment.authorId ?
                                    <IconButton onClick={handleOpenMenu}><MoreVertIcon/></IconButton> : <></>}
                                subheader={transformUtcStringToDateMonthHoursMinutesString(comment.registrationDate)}/>

                            <CardContent style={{display: "block"}}>
                                <Box style={{wordWrap: "break-word"}}>
                                    {comment.content}
                                </Box>
                            </CardContent>
                        </>
                    </Card>
            }
        </>
    );
};
export {CommentCard};