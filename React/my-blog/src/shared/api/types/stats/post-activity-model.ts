import { TimeMeasure } from "./time-measure";

export interface PostActivityModel {
    id: number;
    generatedAt: Date;
    measure: TimeMeasure;
    steps: PostActivityStepModel[]
}

interface PostActivityStepModel {
    reactions: number;
    comments: number;
    startDate: Date
}