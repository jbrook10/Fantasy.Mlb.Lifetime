import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PitcherStatsComponent } from './pitcher-stats.component';

describe('PitcherStatsComponent', () => {
  let component: PitcherStatsComponent;
  let fixture: ComponentFixture<PitcherStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PitcherStatsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PitcherStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
