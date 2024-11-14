import React, {useEffect, useState} from 'react';
import {Navigate, useParams} from 'react-router-dom';
import {PostCard} from '../../components/PostCard';
import {DefaultPageSize} from '../../shared/config';
import {postApi} from "../../shared/api/http/api";
import {PostModel} from '../../shared/api/types/post';
import {AxiosResponse} from "axios";
import {Box, CircularProgress, Typography} from "@mui/material";
import CancelIcon from '@mui/icons-material/Cancel';

const PostPage = () => {

    const {postId} = useParams();
    const [post, setPost] = useState<PostModel>();
    const [hasError, setError] = useState<boolean>(false);
    const [isLoading, setLoading] = useState<boolean>();

    const handleDeletePostAndRedirectToHomePage = () => <Navigate to={`/`}/>

    useEffect(() => {
        setLoading(true);

        if (typeof postId === "string") {
            postApi.getPostById(parseInt(postId) || 0).then((result: AxiosResponse<PostModel>) => {
                setPost(result.data)
            })
                .catch(() => {
                    setError(true);
                });

            setLoading(false);
        }
    }, []);

    return (
        <>
            {
                isLoading
                    ?
                    <Box style={{margin: "15% 0 0 0", display: "flex", justifyContent: "space-around"}}>
                        <CircularProgress size={100}/>
                    </Box>
                    :
                    (hasError
                            ? <Box style={{margin: "15% auto"}}>
                                <Typography variant={"h2"} style={{textAlign: "center"}}>
                                    No post of id {postId} was found on the server
                                </Typography>

                                <CancelIcon
                                    style={{margin: "0 auto", display: "block", width: "100px", height: "100px"}}/>
                            </Box>

                            :
                            post && <PostCard enableCommentInfiniteScroll initialPost={post} width={"70%"}
                                              commentPortionSize={DefaultPageSize}
                                              disappearPostCallback={handleDeletePostAndRedirectToHomePage}
                                              redirectToAfterDelete='/'/>
                    )
            }
        </>
    );
};

export {PostPage};