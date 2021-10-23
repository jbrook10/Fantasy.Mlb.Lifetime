import { PlayerService } from './../../../service/player.service';
import { Batter, Owner } from './../../../models/owner.model';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-batter-stats',
  templateUrl: './batter-stats.component.html',
  styleUrls: ['../../stats.component.scss']
})
export class BatterStatsComponent implements OnInit, OnChanges {

  @Input() Owner!: Owner;
  @Input() SmallScreen!: boolean;

  countingPlayers = 10;

  columns = ['Name', 'GP', 'AB', 'H', 'R', 'HR', 'RBI', 'SB', 'BB', 'Total'];
  columnsBig = ['Name', 'GP', 'AB', 'H', 'R', 'HR', 'RBI', 'SB', 'BB', 'Total'];
  columnsSmall = ['Name', 'H', 'R', 'HR', 'RBI', 'SB', 'BB', 'Total'];


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
    this.Owner.Batters.map(b => b.Total = this.playerService.GetBatterPoints(b));
    this.Owner.Batters.sort((a, b) => b.Total - a.Total);
  }

  scrollToTop(): void {
    const el = document.getElementById('topjump');
    el?.scrollIntoView();
  }

}
