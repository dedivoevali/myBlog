import {
    Avatar,
    Box,
    Card,
    CardActions,
    CardContent,
    CardHeader,
    Chip,
    Collapse,
    IconButton,
    Menu,
    MenuItem,
    Typography
} from '@mui/material';
import React, {useEffect, useState} from 'react';
import {PostCardProps} from './PostCardProps';
import MoreVertIcon from "@mui/icons-material/MoreVert";
import * as assets from '../../shared/assets';
import CommentIcon from '@mui/icons-material/Comment';
import {postApi, userApi} from '../../shared/api/http/api';
import {CommentReel} from "../CommentReel";
import {DefaultPageSize} from "../../shared/config";
import {FilterLogicalOperator} from "../../shared/api/types/paging";
import {CursorPagedRequest} from "../../shared/api/types/paging/cursorPaging";
import {ExpandMoreCard} from './ExpandMoreCard';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import {PostReactionBox} from "../PostReactionBox";
import {Link, useNavigate} from 'react-router-dom';
import {PostForm} from '../PostForm';
import {AxiosResponse} from 'axios';
import {PostDto, PostModel} from '../../shared/api/types/post';
import {useSelector} from "react-redux";
import {ApplicationState} from '../../redux';
import {ConfirmActionCustomModal} from "../CustomModal";
import {UserInfoCache} from "../../shared/types";
import {useNotifier} from '../../hooks';
import { FlagEmoji } from '../FlagEmoji/FlagEmoji';
import { Analytics } from '@mui/icons-material';
import { PostCardStatisticsDialog } from './PostCardStatisticsDialog';
import styles from "./post-card.module.scss";

const PostCard = ({
                      initialPost,
                      width = "100%",
                      commentPortionSize = DefaultPageSize,
                      disappearPostCallback,
                      enableCommentInfiniteScroll = false,
                      redirectToAfterDelete = undefined
                  }: PostCardProps) => {

    const [post, setPost] = useState<PostModel>(initialPost);
    const [editPostMode, setEditPostMode] = useState<boolean>(false);
    const [confirmDeleteDialogOpen, setConfirmDeleteDialogOpen] = useState<boolean>(false);
    const navigate = useNavigate();

    const commentsPagingRequestDefault: CursorPagedRequest = {
        pageSize: commentPortionSize,
        getNewer: false,
        requestFilters: {
            logicalOperator: FilterLogicalOperator.And,
            filters: [
                {
                    path: "PostId",
                    value: post.id.toString()
                }
            ]
        }
    }

    const user = useSelector<ApplicationState, (UserInfoCache | null)>(state => state.user);

    const [statsDialogOpen, setStatsDialogOpen] = useState(false);

    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);

    const handleOpenMenu = (e: React.MouseEvent<HTMLButtonElement>) => setAnchorEl(e.currentTarget);

    const handleCloseMenu = () => setAnchorEl(null);

    const notifyUser = useNotifier();

    const [avatarLink, setAvatarLink] = useState("");
    const [commentsOpen, setCommentsOpen] = useState<boolean>(false);

    const fetchAvatarUrl = (userId: number) => userApi.getAvatarUrlById(userId).then(response => setAvatarLink(response.data));

    const handleExpandCommentSection = () => setCommentsOpen(!commentsOpen);

    const handleEditPost = async (newPost: PostDto): Promise<AxiosResponse<PostModel>> => {
        return postApi.editPost(post.id, newPost).then((result: AxiosResponse<PostModel>) => {
            if (result.status === 200 && user) {
                setPost({...post, content: result.data.content, title: result.data.title, topic: result.data.topic});
            }
            handleCloseMenu();
            setEditPostMode(false);
            return result;
        }).catch((result) => result);
    }

    const handleDeletePost = (postId: number) => {
        postApi.removePostId(postId).then((result) => {
            if (result.status === 200 && user) {
                disappearPostCallback();
                notifyUser("Post was successfully deleted", "success");
                if (redirectToAfterDelete) {
                    navigate(redirectToAfterDelete);
                }
            } else {
                notifyUser("Error occurred", "error");
            }

        })
    }

    useEffect(() => {
        fetchAvatarUrl(post.authorId);
    }, []);

    return (
        <>
            {
                user && editPostMode ?
                    <PostForm initialPost={post} width={"50%"} caption={"Edit post"} formActionCallback={handleEditPost}
                              formCloseHandler={() => {
                                  handleCloseMenu();
                                  setEditPostMode(false);
                              }}/>
                    :
                    <Card elevation={10} style={{width: width, margin: "20px auto"}}>
                        <>
                            {
                                user && confirmDeleteDialogOpen &&
                                <ConfirmActionCustomModal actionCallback={() => handleDeletePost(post.id)}
                                                          title={"Delete post"}
                                                          caption={"Are you sure you want to delete this post"}
                                                          modalOpen={confirmDeleteDialogOpen}
                                                          setModalOpen={setConfirmDeleteDialogOpen}/>
                            }
                            {
                                user &&
                                <Menu anchorEl={anchorEl} open={open} onClose={handleCloseMenu}>
                                    <MenuItem onClick={() => setEditPostMode(true)}>Edit post</MenuItem>
                                    <MenuItem onClick={() => setConfirmDeleteDialogOpen(true)}>Remove post</MenuItem>
                                    <MenuItem onClick={handleCloseMenu}>Close</MenuItem>
                                </Menu>
                            }
                            <CardHeader
                                avatar={<Avatar src={avatarLink}>{post.authorInitials}</Avatar>}
                                action={user && user?.id === post.authorId ?
                                    <IconButton onClick={handleOpenMenu}><MoreVertIcon/></IconButton> : <></>}
                                title={<Link to={`/user/${post.authorId}`}>{post.authorUsername}</Link>}
                                subheader={
                                    <Box
                                        display={"flex"}
                                        justifyContent={'left'}
                                        flexDirection={'row'}
                                        gap={'10px'}
                                        alignItems={"center"}>
                                        <Link
                                            to={`/post/${post.id}`}>Posted
                                            at {assets.transformUtcStringToDateMonthHoursMinutesString(post.registrationDate)}
                                        </Link>
                                        <FlagEmoji fontSizePx={16} emoji={assets.getFlagEmoji(post.language)}/>
                                        <IconButton className={styles['stats-icon']} onClick={() => setStatsDialogOpen(true)}>
                                            <Analytics/>
                                        </IconButton>
                                        <PostCardStatisticsDialog open={statsDialogOpen} post={post} close={() => setStatsDialogOpen(false)}/>
                                    </Box>}/>
                            <CardContent>
                                <>
                                    <Typography variant="h5">{post.title}</Typography>
                                    {post.topic &&
                                        <Link to={{pathname: `/topic/${post.topic}`}}>
                                            <Chip style={{display: "block", width: "fit-content", padding: "5px 5px"}}
                                                  variant="outlined" color={"primary"} label={"#" + post.topic}/>
                                        </Link>
                                    }
                                    <Typography>{post.content}</Typography>
                                </>
                            </CardContent>

                            <CardActions>

                                <PostReactionBox postId={post.id}/>

                                <IconButton onClick={() => setCommentsOpen(true)} style={{display: "flex"}}
                                            aria-label="comments">
                                    <CommentIcon/>
                                    {post.amountOfComments}
                                </IconButton>

                                <ExpandMoreCard expanded={commentsOpen} onClick={() => handleExpandCommentSection()}>
                                    <ExpandMoreIcon/>
                                </ExpandMoreCard>

                            </CardActions>

                            <Collapse in={commentsOpen} orientation={"vertical"} timeout={"auto"}>
                                <CardContent>
                                    <CommentReel enableInfiniteScroll={enableCommentInfiniteScroll} reelWidth={"100%"}
                                                 pagingRequestDefault={commentsPagingRequestDefault} post={post}/>
                                </CardContent>
                            </Collapse>
                        </>
                    </Card>
            }
        </>
    )

};

export {PostCard};