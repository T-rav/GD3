import { Component, Input, OnInit } from '@angular/core';
import { DailyDeveloperStat } from './domain/daily-developer-stat';
import { MatTableDataSource } from '@angular/material';

@Component({
  selector: 'daily-developer-stat',
  templateUrl: './daily-developer-stat.component.html',
  styleUrls: ['./daily-developer-stat.component.scss']
})
export class DailyDeveloperStatComponent implements OnInit {

  @Input() developerStats : DailyDeveloperStat[];

  displayedColumns = ['name', 'commits', 'impact', 'linesOfChangePerHour', 'churn', 'linesAdded','linesRemoved'];
  dataSource: MatTableDataSource<DailyDeveloperStat>;

  constructor() { }

  ngOnInit() {
    this.dataSource = new MatTableDataSource(this.developerStats);
  }

  isValid(stats : DailyDeveloperStat[]) : boolean {
    return typeof stats != "undefined" && stats != null && stats.length > 0;
  }
}
