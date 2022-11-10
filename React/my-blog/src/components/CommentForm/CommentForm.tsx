import React, {useState} from 'react';
import {CommentFormProps} from "./CommentFormProps";
import {Box, Button, CircularProgress, FormControl, FormHelperText, IconButton, Paper, TextField} from "@mui/material";
import {useFormik} from "formik";
import {CommentDto} from '../../shared/api/types/comment';
import {AxiosError} from "axios";
import * as Yup from "yup";
import {CommentValidationConstraints, palette} from "../../shared/assets";
import {FormHeader} from '../FormHeader';
import ModeEditIcon from '@mui/icons-material/ModeEdit';
import CloseIcon from "@mui/icons-material/Close";
import {useNotifier} from '../../hooks';


const textFieldStyle: React.CSSProperties = {
    padding: "0 0 20px 0"
}

const errorTextStyle: React.CSSProperties = {
    color: "red",
    fontStyle: "italic"
}


const CommentForm = ({
                         post,
                         initialComment = {postId: 0, content: ""},
                         formActionCallback,
                         formCloseHandler,
                         width = "100%",
                         caption = "Form"
                     }: CommentFormProps) => {

    const [loading, setLoading] = useState<boolean>();

    const notifyUser = useNotifier();


    const formik = useFormik<CommentDto>({
        initialValues: {
            postId: post?.id || initialComment?.postId,
            content: initialComment.content
        },
        onSubmit: async (values, formikHelpers) => {
            setLoading(true);

            formActionCallback(values).then((result) => {
                if (result.status !== 200 && result instanceof AxiosError) {
                    notifyUser(result.response?.data.Message, "error");
                } else {
                    notifyUser("Comment was successfully handled!", "success");
                    formikHelpers.resetForm();
                }
            }).then(() => {
                setLoading(false);
            });
        },
        validationSchema: Yup.object({
            postId: Yup.number()
                .required(),
            content: Yup.string()
                .required("Please introduce comment text")
                .max(CommentValidationConstraints.ContentMaxLength)
        })
    });

    return (
        <>
            {
                loading
                    ?
                    <Box style={{margin: "50px auto", width: "fit-content"}}>
                        <CircularProgress/>
                    </Box>
                    :
                    <Paper elevation={1} sx={{
                        width: width,
                        padding: "20px 0",
                        margin: "0",
                        display: "flex",
                        justifyContent: "space-between",
                        flexDirection: "row"
                    }}>
                        <FormHeader iconColor={palette.JET} caption={caption} icon={<ModeEditIcon/>}/>

                        <form style={{
                            width: "100%",
                            display: "flex",
                            flexDirection: "column",
                            justifyContent: "space-around"
                        }} onSubmit={formik.handleSubmit}>

                            <FormControl>
                                <FormHelperText>
                                    {formik.touched.content && formik.errors.content &&
                                        <span style={errorTextStyle}>{formik.errors.content}</span>}
                                </FormHelperText>

                                <TextField multiline
                                           rows={5} style={textFieldStyle} name="content" label="Comment"
                                           placeholder="Your comment here..."
                                           value={formik.values.content} onChange={formik.handleChange}/>
                            </FormControl>

                            <Button type={"submit"}>Submit</Button>
                        </form>

                        <Box>
                            <IconButton onClick={formCloseHandler}>
                                <CloseIcon/>
                            </IconButton>
                        </Box>
                    </Paper>
            }
        </>
    );
};

export {CommentForm};