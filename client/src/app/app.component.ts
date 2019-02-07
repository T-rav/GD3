import { Component, OnInit } from '@angular/core';
import { DeveloperStatsGatewayService } from './domain/developer/developer-stats-gateway.service'
import { AnalysisStats } from './domain/analysisStats';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public title: string = 'GD3 (Git Data Driven Decisions)';
  public displayedColumns: string[] = [
    'name',
    'periodActiveDays',
    'activeDaysPerWeek',
    'commitsPerDay',
    'impact',
    'linesOfChangePerHour',
    'churn',
    'linesAdded',
    'linesRemoved',
    'rtt100',
    'ptt100',
    'dtt100',
    'riskFactor',
  ];
  public teamDisplayedColumns: string[] = [
    'dateOf',
    'totalCommits',
    'activeDevelopers',
    'velocity'
  ];
  public stats: AnalysisStats[];

  constructor(private developerStatsGateway: DeveloperStatsGatewayService) { }

  public ngOnInit() {
    console.log(this.developerStatsGateway);
    this.developerStatsGateway.get().subscribe(d => {
      this.stats = d;
    })
  }
}
