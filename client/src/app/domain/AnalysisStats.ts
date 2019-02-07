import { DeveloperStats } from "./developer/developer-stats";
import { TeamStats } from "./team/TeamStats";
import { ReportRange } from "./time/ReportRange";

export interface AnalysisStats {
    reportingRange: ReportRange;
    developerStats: DeveloperStats[];
    teamStats: TeamStats[];
}