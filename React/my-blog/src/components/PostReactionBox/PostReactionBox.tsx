import React, {useEffect, useState} from 'react';
import {Avatar, AvatarGroup, Box, ClickAwayListener, IconButton, Popper, Typography} from "@mui/material";
import {useSelector} from "react-redux";
import {ApplicationState, CurrentUserState} from "../../redux";
import {PostReactionModel} from "../../shared/api/types/postReaction";
import {PostReactionBoxProps} from "./PostReactionBoxProps";
import { postReactionApi } from "../../shared/api/http/api";
import {AxiosResponse} from "axios";
import {ReactionType} from "../../shared/api/types";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import ThumbUpOutlinedIcon from "@mui/icons-material/ThumbUpOutlined";
import ThumbDownOutlinedIcon from "@mui/icons-material/ThumbDownOutlined";
import FavoriteIcon from "@mui/icons-material/Favorite";
import ThumbUpIcon from "@mui/icons-material/ThumbUp";
import ThumbDownIcon from "@mui/icons-material/ThumbDown";
import AuthorizationRequiredCustomModal
    from "../CustomModal/AuthorizationRequiredCustomModal/AuthorizationRequiredCustomModal";
import {DefaultAvatarGroupMaxLength} from "../../shared/config";
import { UserApi } from '../../shared/api/http/user-api';

const PostReactionBox = ({postId}: PostReactionBoxProps) => {

    const user = useSelector<ApplicationState, (CurrentUserState | undefined | null)>(state => state.user);
    const [reactions, setReactions] = useState<PostReactionModel[]>([]);
    const [userReaction, setUserReaction] = useState<{ exists: boolean, type?: ReactionType }>({exists: false});
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const [avatarUrls, setAvatarUrls] = useState<string[]>([]);
    const [userAvatar, setUserAvatar] = useState<string>("");
    const [isAvatarsLoading, setAvatarsLoading] = useState<boolean>(true);
    const [isReactionsLoading, setIsReactionsLoading] = useState<boolean>(true);
    const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);


    const handlePopupOpen = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    }

    const handlePopupClose = () => {
        setAnchorEl(null);
    }

    const open = Boolean(anchorEl);

    const fetchPostReactions = (postId: number) => postReactionApi.getReactionsByPost(postId).then((result: AxiosResponse<PostReactionModel[]>) => {
        setReactions(result.data);

        if (user) {
            const reactionsFilteredByUserId = result.data.filter((val) => val.userId === user?.id)

            if (reactionsFilteredByUserId.length === 1) {
                setUserReaction({exists: true, type: reactionsFilteredByUserId[0].reactionType});
            } else {
                setUserReaction({exists: false});
            }
        } else {
            setUserReaction({exists: false});
        }

        return result.data
    });

    const handleNewReaction = (type: ReactionType) => {
        if (!user) {
            setModalOpen(true);
            return;
        }

        if (user) {
            if (userReaction.exists) {

                postReactionApi.updateReactionOnPost({postId: postId, reactionType: type}).then(() => {
                    setUserReaction({exists: true, type: type});
                    setReactions([...reactions.filter(reaction => reaction.userId !== user.id), {
                        postId: postId,
                        userId: user.id,
                        reactionType: type
                    }]);
                });
            } else {

                postReactionApi.addReactionToPost({postId: postId, reactionType: type}).then(() => {
                    setUserReaction({exists: true, type: type});
                    setReactions([...reactions, {postId: postId, userId: user.id, reactionType: type}])
                });
            }
        }
    }

    const handleRemoveReaction = () => {
        if (!user) {
            setModalOpen(true);
            return;
        }

        if (userReaction.exists && user) {
            postReactionApi.removeReactionFromPost(postId).then(() => {
                setUserReaction({exists: false});
                setReactions(reactions.filter(reaction => reaction.userId !== user.id))
            });
        }
    }

    // this system is based on order of reaction types like in enum.
    const reactionsAndComponentsInactive = [
        <IconButton onClick={() => handleNewReaction(ReactionType.Like)} children={<ThumbUpOutlinedIcon/>}/>,
        <IconButton onClick={() => handleNewReaction(ReactionType.Dislike)} children={<ThumbDownOutlinedIcon/>}/>,
        <IconButton onClick={() => handleNewReaction(ReactionType.Love)} children={<FavoriteBorderIcon/>}/>
    ]

    const reactionsAndComponentsActive = [
        <IconButton onClick={() => handleRemoveReaction()} children={<ThumbUpIcon color={"info"}/>}/>,
        <IconButton onClick={() => handleRemoveReaction()} children={<ThumbDownIcon color={"warning"}/>}/>,
        <IconButton onClick={() => handleRemoveReaction()} children={<FavoriteIcon color={"error"}/>}/>
    ]

    const fetchAvatarUrl = (userId: number) => UserApi.getAvatarUrlById(userId).then((result: AxiosResponse<string>) => result.data);


    useEffect(() => {
        setIsReactionsLoading(true);
        setAvatarsLoading(true);

        fetchPostReactions(postId).then(async (result) => {
            let avatars: string[] = [];

            const groupLength: number = result.length < DefaultAvatarGroupMaxLength ? result.length : DefaultAvatarGroupMaxLength;

            for (let i = 0; i < groupLength; i++) {
                const link: string = await fetchAvatarUrl(result[i].userId).then((link) => link);

                if (result[i].userId !== (user?.id || 0)) {
                    avatars.push(link);
                }
            }

            return avatars;
        }).then(avatars => {
            setAvatarUrls(avatars);

            if (user && user) {
                fetchAvatarUrl(user?.id).then(result => setUserAvatar(result));
            }

            setIsReactionsLoading(false);
            setAvatarsLoading(false);
        });

    }, [user]);

    return (
        <>
            <AuthorizationRequiredCustomModal modalOpen={modalOpen} setModalOpen={setModalOpen} caption={"Please sign up to share your thoughts"}/>

            {
                !isReactionsLoading
                &&
                <Box style={{display: "flex", flexDirection: "row"}}>

                    <Box onMouseEnter={handlePopupOpen}>
                        {
                            (userReaction.exists && userReaction.type)
                                ?
                                reactionsAndComponentsActive[userReaction.type - 1]
                                :
                                reactionsAndComponentsInactive[ReactionType.Love - 1]
                        }
                    </Box>

                    <ClickAwayListener onClickAway={handlePopupClose}>
                        <Popper sx={{boxShadow: "4px 6px 40px 0px rgba(0,0,0,0.75)", borderRadius: "20px"}} open={open}
                                anchorEl={anchorEl} placement={"left"} onMouseLeave={handlePopupClose}>

                            <Box style={{
                                width: "200px",
                                display: "flex",
                                flexDirection: "column",
                                backgroundColor: "#FFFFFF",
                                borderRadius: "5px",
                                padding: "1px 20px",
                                justifyContent: "space-around"
                            }}>


                                <Box style={{margin: "auto 0"}}>
                                    {userReaction.type === ReactionType.Like ? reactionsAndComponentsActive[ReactionType.Like - 1] : reactionsAndComponentsInactive[ReactionType.Like - 1]}
                                    {userReaction.type === ReactionType.Dislike ? reactionsAndComponentsActive[ReactionType.Dislike - 1] : reactionsAndComponentsInactive[ReactionType.Dislike - 1]}
                                    {userReaction.type === ReactionType.Love ? reactionsAndComponentsActive[ReactionType.Love - 1] : reactionsAndComponentsInactive[ReactionType.Love - 1]}
                                </Box>

                                <Typography fontSize={"smaller"}>{
                                    reactions.length === 0
                                        ?
                                        "There are no reactions yet."
                                        :
                                        `${reactions.length} people reacted`
                                }
                                </Typography>
                                {
                                    (isAvatarsLoading || isReactionsLoading)
                                        ?
                                        <AvatarGroup>
                                            {[...new Array(DefaultAvatarGroupMaxLength)].map((value, index) => <Avatar
                                                key={index}/>)}
                                        </AvatarGroup>
                                        :
                                        <AvatarGroup max={DefaultAvatarGroupMaxLength} total={reactions.length}>
                                            {userReaction.exists &&
                                                <Avatar style={{border: "none"}} key={userAvatar} src={userAvatar}/>}

                                            {avatarUrls.map((link) => <Avatar style={{border: "none"}} key={link}
                                                                              src={link}/>)}
                                        </AvatarGroup>
                                }
                            </Box>
                        </Popper>
                    </ClickAwayListener>
                </Box>
            }
        </>
    );
};

export {PostReactionBox};
