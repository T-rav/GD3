import { Author } from "../../domain/developer/author";

export interface DailyDeveloperStat {
    author: Author,
    commitsPerDay: number,
    impact: number,
    linesOfChangePerHour: number,
    churn: number,
    linesAdded: number,
    linesRemoved: number,
    rtt100: number,
    ptt100: number,
    dtt100: number,
    riskFactor: number
}