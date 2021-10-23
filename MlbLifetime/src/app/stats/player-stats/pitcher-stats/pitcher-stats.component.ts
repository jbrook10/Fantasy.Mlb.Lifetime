import { PlayerService } from './../../../service/player.service';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Owner } from 'src/app/models/owner.model';

@Component({
  selector: 'app-pitcher-stats',
  templateUrl: './pitcher-stats.component.html',
  styleUrls: ['../../stats.component.scss']
})
export class PitcherStatsComponent implements OnInit, OnChanges {

  @Input()  Owner!: Owner;
  @Input()  SmallScreen!: boolean;

  countingPlayers = 10;

  columns = ['Name', 'GP', 'W', 'IP', 'SO', 'SV', 'Total'];
  columnsBig = ['Name', 'GP', 'W', 'IP', 'SO', 'SV', 'Total'];
  columnsSmall = ['Name', 'W', 'IP', 'SO', 'SV', 'Total'];

  constructor(private playerService: PlayerService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (!!changes && !!changes.SmallScreen) {
      this.columns = changes.SmallScreen.currentValue ? this.columnsSmall : this.columnsBig;
    }

    if (!!changes && !!changes.Owner) {
      this.countingPlayers = changes.Owner.currentValue.CountingPlayers || 10;
    }
  }

  ngOnInit(): void {
    this.Owner.Pitchers.map(p => p.Total = this.playerService.GetPitcherPoints(p));
    this.Owner.Pitchers.sort((a, b) => b.Total - a.Total);
  }

  scrollToTop(): void {
    const el = document.getElementById('topjump');
    el?.scrollIntoView();
  }

}
