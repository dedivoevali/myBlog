import { AxiosInstance } from "axios";
import { instance } from "./api";
import { GetPostStatsDto, PostActivityModel, TimeMeasure } from "../types/stats";

export class ContentStatisticsApi {
    static create(): ContentStatisticsApi {
        return new ContentStatisticsApi(instance);
    }
    constructor(private readonly instance: AxiosInstance) {};
    
    public getPostStats = (postId: number, measure: TimeMeasure, payload: GetPostStatsDto): Promise<PostActivityModel> =>
        this.instance.post(`/stats/post/${postId}?measure=${measure}`, payload).then((response) => response.data);

}