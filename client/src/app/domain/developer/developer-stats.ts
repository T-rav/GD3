import { Author } from "./author"

export interface DeveloperStats {
    author: Author,
    periodActiveDays: number,
    activeDaysPerWeek: number,
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