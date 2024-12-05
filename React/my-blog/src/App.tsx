import React from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import OnlyForUnauthorized from './guards/OnlyForUnauthorized';
import { RequireAuth } from './guards';
import { HomePage, Layout, LoginPage, NotFoundPage, PostPage, ProfilePage, RegisterPage, TopicPage } from './pages';
import "./App.scss";
import { UserInfoCache } from "./shared/types";
import { useSelector } from 'react-redux';
import { ApplicationState } from './redux';


function App() {
    const user = useSelector<ApplicationState, (UserInfoCache | null)>(state => state.user);
    const userId = user ? user.id : 0;

    return (
        <div className="App">
            <Routes>
                <Route path="/" element={<Layout/>}>
                    <Route path="myBlog" element={<Navigate to='/'/>}></Route>
                    <Route path="me" element={<RequireAuth children={<Navigate to={`/user/${userId}`}/>}/>}/>
                    <Route path="login" element={<OnlyForUnauthorized children={<LoginPage/>}/>}/>
                    <Route path="register" element={<OnlyForUnauthorized children={<RegisterPage/>}/>}/>
                    <Route path="/" element={<HomePage/>}/>
                    <Route path="post/:postId" element={<PostPage/>}/>
                    <Route path="/topic/:topicName" element={<TopicPage/>}/>
                    <Route path="user/:userId" element={<ProfilePage/>}/>
                    <Route path="*" element={<NotFoundPage/>}/>
                </Route>
            </Routes>
        </div>
    );
}

export default App;