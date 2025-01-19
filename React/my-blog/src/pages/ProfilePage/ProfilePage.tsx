import {Box, Tab, Tabs, Typography} from '@mui/material';
import React, {useEffect, useState} from 'react';
import {useSelector} from 'react-redux';
import {useParams} from 'react-router-dom';
import {BlogReel} from '../../components/BlogReel';
import {CommentReel} from '../../components/CommentReel';
import {CustomTabPanel} from '../../components/CustomTabPanel';
import {ProfileHeader} from '../../components/ProfileHeader';
import {CenteredLoader} from '../../components/CenteredLoader';
import {ApplicationState, CurrentUserState} from '../../redux';
import {FilterLogicalOperator} from '../../shared/api/types/paging';
import {CursorPagedRequest} from '../../shared/api/types/paging/cursorPaging';
import {UserModel} from '../../shared/api/types/user';
import {DefaultPageSize} from '../../shared/config';
import {AxiosError, AxiosResponse} from "axios";
import {ErrorResponse} from "../../shared/api/types";
import CancelIcon from "@mui/icons-material/Cancel";
import { UserApi } from '../../shared/api/http/user-api';
import { useTitle } from '../../hooks/use-title';

const ProfilePage = () => {

    const isAuthorized = useSelector<ApplicationState, CurrentUserState | undefined | null>(state => state.user);

    const [isLoading, setLoading] = useState<boolean>(true);
    const {userId} = useParams();
    const [visibleTabIndex, setVisibleTabIndex] = useState<number>(0);
    
    const [pageRequestPostReel, setPageRequestPostReel] = useState<CursorPagedRequest>();
    const [pageRequestCommentReel, setPageRequestCommentReel] = useState<CursorPagedRequest>();

    const [hasError, setHasError] = useState<boolean>(false);
    const [errorText, setErrorText] = useState<string>("Error occurred");

    const fetchUser = () => UserApi.getUserById(parseInt(userId || "0")).then(response => {
        return response;
    });

    const [user, setUser] = useState<UserModel>();

    const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
        event.preventDefault();
        setVisibleTabIndex(newValue)
    };

    const tabProps = (index: number) => {
        return {id: `simple-tab-${index}`, 'aria-controls': `simple-tabpanel-${index}`}
    };

    useTitle(`${user?.username || "LOADING..."}`, [user?.username]);
    useEffect(() => {
        setLoading(true);

        fetchUser().then((response: AxiosResponse<UserModel>) => {
            setUser(response.data);

            setPageRequestPostReel({
                requestFilters: {
                    logicalOperator: FilterLogicalOperator.And,
                    filters: [
                        {
                            path: "UserId",
                            value: response.data.id.toString()
                        }
                    ]
                },
                pageSize: DefaultPageSize,
                getNewer: false
            });

            setPageRequestCommentReel({
                requestFilters: {
                    logicalOperator: FilterLogicalOperator.And,
                    filters: [
                        {
                            path: "UserId",
                            value: response.data.id.toString()
                        }
                    ]
                },
                pageSize: DefaultPageSize,
                getNewer: false
            });

            setLoading(false);
        }).catch((error: AxiosError<ErrorResponse>) => {
            setHasError(true);

            if (error.response) {
                setErrorText(error.response?.data.Message);
            }

            setLoading(false);
        });
    }, [isAuthorized]);

    return (
        <>
            {
                (isLoading)
                    ?
                    <CenteredLoader/>
                    :
                    <>
                        {user && !hasError ?
                            <>
                                <ProfileHeader setUser={setUser} user={user}/>

                                <Box style={{width: "70vw", margin: "0 auto"}}>
                                    <Box>
                                        <Tabs value={visibleTabIndex} onChange={handleTabChange}>
                                            <Tab label="Posts" {...tabProps(0)}/>
                                            <Tab label="Comments" {...tabProps(1)}/>
                                        </Tabs>
                                    </Box>

                                    <CustomTabPanel index={0} value={visibleTabIndex}>
                                        {pageRequestPostReel && <BlogReel reelWidth="100%" pageSize={DefaultPageSize}
                                                                          pagingRequestDefault={pageRequestPostReel}
                                        />}
                                    </CustomTabPanel>

                                    <CustomTabPanel index={1} value={visibleTabIndex}>
                                        {<CommentReel enableInfiniteScroll reelWidth="100%"
                                                      pagingRequestDefault={pageRequestCommentReel}></CommentReel>}
                                    </CustomTabPanel>
                                </Box>
                            </>
                            :
                            <Box style={{margin: "15% auto"}}>
                                <Typography variant={"h2"} style={{textAlign: "center"}}>
                                    {errorText}
                                </Typography>

                                <CancelIcon
                                    style={{margin: "0 auto", display: "block", width: "100px", height: "100px"}}/>
                            </Box>
                        }
                    </>
            }
        </>
    );
};

export {ProfilePage};