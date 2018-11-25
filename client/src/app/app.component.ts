import { Component, OnInit } from '@angular/core';
import { DeveloperStats } from './domain/developer/developer-stats'
import { DeveloperStatsGatewayService } from './domain/developer/developer-stats-gateway.service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public title: string = 'gd3';
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
  public stats: DeveloperStats[];

  constructor(private developerStatsGateway: DeveloperStatsGatewayService) { }

  public ngOnInit() {
    console.log(this.developerStatsGateway);
    this.developerStatsGateway.get().subscribe(d => {
      this.stats = d;
    })
  }
}
