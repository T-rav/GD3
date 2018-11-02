import { Component } from '@angular/core';
import { DeveloperStats } from './domain/developer/developer-stats'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'gd3';
  displayedColumns = ['test'];
  results: DeveloperStats[] = [
    { field: "yay" },
    { field: "yay1" },
    { field: "yay2" }
  ]
}
