import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyDeveloperStatComponent } from './daily-developer-stat.component';

describe('DailyDeveloperStatComponent', () => {
  let component: DailyDeveloperStatComponent;
  let fixture: ComponentFixture<DailyDeveloperStatComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DailyDeveloperStatComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DailyDeveloperStatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
