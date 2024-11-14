import { BarChart } from '@mui/x-charts';
import styles from './PostStatisticsTable.module.scss';
import { useEffect, useState } from 'react';
import { PostStatisticsTableProps } from './post-statistics-table-props';
import { PostActivityModel, TimeMeasure } from '../../shared/api/types/stats';
import { CenteredLoader } from '../CenteredLoader';
import { ContentStatisticsApi } from '../../shared/api/http/content-statistics-api';

const PostStatisticsTable = ({ startDate, endDate, postId, measure = TimeMeasure.Day }: PostStatisticsTableProps) => {

    const api = ContentStatisticsApi.create();
    const [isLoading, setLoading] = useState(true);
    const [stats, setStats] = useState<null | PostActivityModel>(null);

    useEffect(() => {
        api.getPostStats(postId,
            measure,
            { startDate, endDate }
        ).then((data) => {
            setStats(data);
            setLoading(false);
        });
    }, []);

    return <>
        {
            isLoading ?
            <CenteredLoader/>
            :
        <BarChart
        height={300}
        width={300}
        xAxis={
            [
                {
                    id: "steps",
                    data: stats?.steps.map(s => s.startDate.toString()),
                    scaleType: "band"
                }
            ]
        }
        series={[
            {
                data: stats?.steps.map(s => s.reactions)
            }
        ]}/>}
        </>
}

export default PostStatisticsTable;