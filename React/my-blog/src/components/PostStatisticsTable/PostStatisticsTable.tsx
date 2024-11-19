import { BarChart } from '@mui/x-charts';
import styles from './PostStatisticsTable.module.scss';
import { useEffect, useState } from 'react';
import { PostStatisticsTableProps } from './post-statistics-table-props';
import { PostActivityModel, TimeMeasure } from '../../shared/api/types/stats';
import { CenteredLoader } from '../CenteredLoader';
import { ContentStatisticsApi } from '../../shared/api/http/content-statistics-api';
import { Box } from '@mui/material';

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
    }, [startDate, endDate]);

    return <>
        {
            isLoading ?
                <CenteredLoader />
                :
                <Box className={styles['graphics-wrapper']}>
                    <BarChart
                    desc='Reactions'
                    height={300}
                    width={300}
                    xAxis={
                        [
                            {
                                id: "steps",
                                data: stats?.steps.map(s => s.startDate.toString()),
                                scaleType: "band",
                                label:'Reactions'
                            }
                        ]
                    }
                    series={[
                        {
                            data: stats?.steps.map(s => s.reactions)
                        }
                    ]} />
                
                <BarChart                    
                    height={300}
                    width={300}
                    xAxis={
                        [
                            {
                                id: "steps",
                                data: stats?.steps.map(s => s.startDate.toString()),
                                scaleType: "band",
                                label: 'Comments'
                            }
                        ]
                    }
                    series={[
                        {
                            data: stats?.steps.map(s => s.comments)
                        }
                    ]} />
                </Box>
                
        }
    </>
}

export default PostStatisticsTable;