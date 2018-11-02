import { Component } from '@angular/core';
import { DeveloperStats } from './domain/developer/developer-stats'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
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
  public stats: DeveloperStats[] = [
    this.createDeveloperStat('Brendon Page', 'brendonpage@live.co.za'),
    this.createDeveloperStat('Travis Frysinger', 'tmfrysiner@gmail.com')
  ]

  private createDeveloperStat(
    name: string,
    email: string,
  ): DeveloperStats {
    return {
      author: { name: name, emails: [email] },
      periodActiveDays: 1,
      activeDaysPerWeek: 2,
      commitsPerDay: 3,
      impact: 4,
      linesOfChangePerHour: 5,
      churn: 6,
      linesAdded: 7,
      linesRemoved: 8,
      rtt100: 9,
      ptt100: 10,
      dtt100: 11,
      riskFactor: 12
    }
  }
}
