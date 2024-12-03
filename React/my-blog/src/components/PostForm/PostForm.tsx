import React, { useState } from 'react';
import { useFormik } from "formik";
import { PostDto } from "../../shared/api/types/post";
import * as Yup from 'yup';
import { commonValidationConstraints, palette, PostValidationConstraints } from "../../shared/assets";
import { Box, Button, FormControl, FormHelperText, IconButton, Paper, TextField } from '@mui/material';
import { PostFormProps } from "./PostFormProps";
import { FormHeader } from '../FormHeader';
import AutoFixHighIcon from '@mui/icons-material/AutoFixHigh';
import { AxiosError } from "axios";
import CloseIcon from '@mui/icons-material/Close';
import { useNotifier } from "../../hooks";
import { CenteredLoader } from '../CenteredLoader';
import styles from "./post-form.module.scss";

const PostForm = ({
                      initialPost = {content: "", title: "", topic: ""},
                      formCloseHandler,
                      caption = "Form",
                      formActionCallback,
                      width = "100%"
                  }: PostFormProps) => {

    const [loading, setLoading] = useState<boolean>(false);
    const notifyUser = useNotifier();


    const formik = useFormik<PostDto>({
            initialValues: {
                title: initialPost.title,
                content: initialPost.content,
                topic: initialPost.topic
            },
            onSubmit: (values, formikHelpers) => {
                setLoading(true);
                formActionCallback(values).then((result) => {
                    if (result.status !== 200 && result instanceof AxiosError) {
                        notifyUser(result.response?.data.Message, "error");
                    } else {
                        notifyUser("Post was successfully handled", "success");
                        formikHelpers.resetForm();
                    }
                }).then(() => {
                    setLoading(false);
                });
            },
            validationSchema: Yup.object({
                title: Yup.string()
                    .required()
                    .max(PostValidationConstraints.TitleMaxLength)
                    .matches(commonValidationConstraints.isAlphaNumeric, {
                        message: "Title should be alphanumeric"
                    }),
                content: Yup.string()
                    .required()
                    .max(PostValidationConstraints.ContentMaxLength),
                topic: Yup.string()
                    .nullable()
                    .optional()
                    .max(PostValidationConstraints.TopicMaxLength)
                    .matches(commonValidationConstraints.isAlphaNumeric, {
                        message: "Topic should be alphanumeric"
                    })
            })
        }
    );

    return (<>
            {
                loading
                    ?
                    <CenteredLoader/>
                    :
                    <Paper elevation={12} style={{
                        width: width,
                        padding: "20px",
                        margin: "20px auto",
                        display: "flex",
                        justifyContent: "space-between",
                        flexDirection: "row"
                    }}>
                        <FormHeader iconColor={palette.JET} caption={caption} icon={<AutoFixHighIcon/>}></FormHeader>

                        <form className={styles["form"]} onSubmit={formik.handleSubmit}>
                            <FormControl>
                                <FormHelperText>
                                    {formik.touched.title && formik.errors.title &&
                                        <span className={styles["form__error"]}>
                                            {formik.errors.title}
                                        </span>}
                                </FormHelperText>
                                <TextField className={styles["form__text-field"]}
                                    name="title"
                                    label="Title"
                                    placeholder="Title"
                                    value={formik.values.title}
                                    onChange={formik.handleChange}/>
                            </FormControl>

                            <FormControl>
                                <FormHelperText>
                                    {formik.touched.topic && formik.errors.topic &&
                                        <span className={styles["form__error"]}>
                                            {formik.errors.topic}
                                            </span>}
                                </FormHelperText>
                                <TextField className={styles["form__text-field"]}
                                    name="topic"
                                    label="Topic (optional)"
                                    value={formik.values.topic}
                                    onChange={formik.handleChange}/>
                            </FormControl>

                            <FormControl>
                                <FormHelperText>
                                    {formik.touched.content && formik.errors.content &&
                                        <span className={styles["form__error"]}>
                                            {formik.errors.content}
                                        </span>}
                                </FormHelperText>

                                <TextField className={styles["form__text-field"]}
                                           name="content"
                                           label="Content"
                                           placeholder="Content"
                                           multiline
                                           rows={5}
                                           value={formik.values.content}
                                           onChange={formik.handleChange}
                                />
                            </FormControl>

                            <Button disabled={JSON.stringify(formik.values) === JSON.stringify(formik.initialValues)}
                                    type={"submit"}>Submit</Button>
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

export {PostForm};
