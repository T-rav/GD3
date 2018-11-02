import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { MatTableModule } from '@angular/material/table';
import { DeveloperStats } from './domain/developer/developer-stats';
import { DeveloperStatsTestDataBuilder } from './../test-data-builders/developer-stats-test-data-builder'

describe('AppComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatTableModule
      ],
      declarations: [
        AppComponent
      ],
    }).compileComponents();
  }));

  it('should create the app', () => {
    // Arrange
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    // Act
    // Assert
    expect(app).toBeTruthy();
  });

  it(`should have as title 'gd3'`, () => {
    // Arrange
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    // Act
    // Assert
    expect(app.title).toEqual('gd3');
  });

  it('should render title in a h1 tag', () => {
    // Arrange
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    // Act
    // Assert
    expect(compiled.querySelector('h1').textContent).toContain('Welcome to gd3!');
  });

  it('should render a header row', () => {
    // Arrange
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    // Act
    // Assert
    const rows = getRows(fixture);
    expectRowToBeDeveloperStatsHeader(rows[0]);
  });

  it('should render a row for each developer stat', () => {
    // Arrange
    const fixture = TestBed.createComponent(AppComponent);
    const stats = [
      DeveloperStatsTestDataBuilder.create().build(),
      DeveloperStatsTestDataBuilder.create().build()
    ];
    // Act
    fixture.componentInstance.stats = stats;
    fixture.detectChanges();
    // Assert
    const rows = getRows(fixture);
    expect(rows.length).toEqual(3)
    expectRowToBeForStat(rows[1], stats[0]);
    expectRowToBeForStat(rows[2], stats[1]);
  });

  function expectRowToBeDeveloperStatsHeader(row: any) {
    const columns = row.querySelectorAll('th');
    expect(columns.length).toEqual(13);
    expect(columns[0].textContent).toEqual('Name');
    expect(columns[1].textContent).toEqual('Period Active Days');
    expect(columns[2].textContent).toEqual('Active Days Per Week');
    expect(columns[3].textContent).toEqual('Commits Per Day');
    expect(columns[4].textContent).toEqual('Impact');
    expect(columns[5].textContent).toEqual('Lines of Change Per Hour');
    expect(columns[6].textContent).toEqual('Churn');
    expect(columns[7].textContent).toEqual('Lines Added');
    expect(columns[8].textContent).toEqual('Lines Removed');
    expect(columns[9].textContent).toEqual('Rtt100');
    expect(columns[10].textContent).toEqual('Ptt100');
    expect(columns[11].textContent).toEqual('Dtt100');
    expect(columns[12].textContent).toEqual('Risk Factor');
  }

  function expectRowToBeForStat(row: any, stat: DeveloperStats) {
    const columns = row.querySelectorAll('td');
    expect(columns[0].textContent).toEqual(stat.author.name);
    expect(columns[1].textContent).toEqual(stat.periodActiveDays.toString());
    expect(columns[2].textContent).toEqual(stat.activeDaysPerWeek.toString());
    expect(columns[3].textContent).toEqual(stat.commitsPerDay.toString());
    expect(columns[4].textContent).toEqual(stat.impact.toString());
    expect(columns[5].textContent).toEqual(stat.linesOfChangePerHour.toString());
    expect(columns[6].textContent).toEqual(stat.churn.toString());
    expect(columns[7].textContent).toEqual(stat.linesAdded.toString());
    expect(columns[8].textContent).toEqual(stat.linesRemoved.toString());
    expect(columns[9].textContent).toEqual(stat.rtt100.toString());
    expect(columns[10].textContent).toEqual(stat.ptt100.toString());
    expect(columns[11].textContent).toEqual(stat.dtt100.toString());
    expect(columns[12].textContent).toEqual(stat.riskFactor.toString());
  }
});

function getRows(fixture: any) {
  return fixture.debugElement.nativeElement.querySelectorAll('table tr');
}
