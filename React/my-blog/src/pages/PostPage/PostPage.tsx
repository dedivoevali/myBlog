import React, {useEffect, useState} from 'react';
import { useNavigate, useParams}  from 'react-router-dom';
import {PostCard} from '../../components/PostCard';
import {DefaultPageSize} from '../../shared/config';
import {postApi} from "../../shared/api/http/api";
import {PostModel} from '../../shared/api/types/post';
import {AxiosResponse} from "axios";
import {Box, CircularProgress, Typography} from "@mui/material";
import CancelIcon from '@mui/icons-material/Cancel';
import styles from './PostPage.module.scss';

const PostPage = () => {

    const {postId} = useParams();
    const [post, setPost] = useState<PostModel>();
    const [hasError, setError] = useState<boolean>(false);
    const [isLoading, setLoading] = useState<boolean>();
    const navigate = useNavigate();

    const redirectToMain = () => navigate(`/`);

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
                    <Box className={styles['loading-wrapper']}>
                        <CircularProgress size={100}/>
                    </Box>
                    :
                    (hasError
                            ? <Box className={styles['error-wrapper']}>
                                <Typography variant={"h2"} className={styles['error-wrapper__caption']}>
                                    No post of id {postId} was found on the server
                                </Typography>

                                <CancelIcon className={styles['error-wrapper__icon']}/>
                            </Box>
                            :
                            post && <PostCard enableCommentInfiniteScroll initialPost={post} width={"70%"}
                                              commentPortionSize={DefaultPageSize}
                                              disappearPostCallback={redirectToMain}
                                              redirectToAfterDelete='/'/>
                    )
            }
        </>
    );
};

export {PostPage};