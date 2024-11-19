import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material"
import { PostCardStatisticsDialogProps } from "./post-card-statistics-dialog-props"
import PostStatisticsTable from "../../PostStatisticsTable/PostStatisticsTable"
import { TimeMeasure } from "../../../shared/api/types/stats"
import { useState } from "react"
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import dayjs, { Dayjs } from "dayjs"

export const PostCardStatisticsDialog = (
    { open, close, post }: PostCardStatisticsDialogProps) =>
{
    const now = dayjs();
    const minDate = dayjs(post.registrationDate);
    const [startDate, setStartDate] = useState<Dayjs>(now.subtract(7, 'day') < now ? now : now.subtract(7, 'day') );
    const [endDate, setEndDate] = useState<Dayjs>(now);
    const [measure, setMeasure] = useState<TimeMeasure>(TimeMeasure.Day);

    return <Dialog open={open}>
        <DialogTitle>Statistics of post {post.id + ' by ' + post.authorUsername}</DialogTitle>
        <DialogContent>
            <PostStatisticsTable postId={post.id} startDate={startDate.toDate()} endDate={endDate.toDate()} measure={measure}/>
        </DialogContent>
        <DialogActions>
            <DatePicker label="Start Date" value={startDate} onChange={(value) => value && setStartDate(value)}
                minDate={minDate} maxDate={endDate}/>
            <DatePicker label="End Date" value={endDate} onChange={(value) => value && setEndDate(value)}
                minDate={startDate} maxDate={now}/>
            <Button variant="outlined" onClick={close}>CLOSE</Button>
        </DialogActions>
    </Dialog>
}