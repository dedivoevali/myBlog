import { TimeMeasure } from "../../shared/api/types/stats";

export interface PostStatisticsTableProps {
    postId: number;
    startDate: Date;
    endDate: Date;
    measure: TimeMeasure;
}