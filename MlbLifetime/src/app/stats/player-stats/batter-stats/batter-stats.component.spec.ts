import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BatterStatsComponent } from './batter-stats.component';

describe('BatterStatsComponent', () => {
  let component: BatterStatsComponent;
  let fixture: ComponentFixture<BatterStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BatterStatsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BatterStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
