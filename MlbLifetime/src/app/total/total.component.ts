import { PlayerService } from './../service/player.service';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Batter, IPlayer, ITotalView, LeagueData, Owner, Pitcher, Position } from '../models/owner.model';
import { RosterService } from '../service/roster.service';
import { MatButtonToggleChange, MatButtonToggleGroup } from '@angular/material/button-toggle';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-total',
  templateUrl: './total.component.html',
  styleUrls: ['./total.component.scss']
})
export class TotalComponent implements OnInit {

  LeagueData: LeagueData | undefined;
  Owners: Owner[] = [];
  Batters: Batter[] = [];
  Pitchers: Pitcher[] = [];
  totalType = 0;
  viewType = 'All';
  ownerTotals: ITotalView[] = [];
  selection = new SelectionModel<ITotalView>(false, []);

  Players: IPlayer[] = [];
  columns = ['Rank', 'Owner', 'Name', 'Position', 'Points'];
  ownerTotalColumns = ['Owner', 'Count', 'Average'];
  // columns = ['Owner', 'Name', 'H', 'R', 'HR', 'RBI', 'SB', 'BB', 'Points'];

  view: MatButtonToggleGroup | undefined;

  constructor(private rosterService: RosterService, breakpointObserver: BreakpointObserver, private playerService: PlayerService) {
  }

  ngOnInit(): void {

    this.rosterService.LeagueData$.subscribe(r => {
      this.LeagueData = r;
      this.Owners = this.LeagueData.Owners.sort((a, b) => (b.BatterScore + b.PitcherScore) - (a.BatterScore + a.PitcherScore));

      this.Batters = this.Owners.flatMap(o => o.Batters);
      this.Batters.map(b => b.Total = this.playerService.GetBatterPoints(b));
      this.Batters = this.Batters.filter(b => b.Total > 0);

      this.Pitchers = this.Owners.flatMap(o => o.Pitchers);
      this.Pitchers.map(p => p.Total = this.playerService.GetPitcherPoints(p));
      this.Pitchers = this.Pitchers.filter(p => p.Total > 0);

      this.totalType = this.Batters.length + this.Pitchers.length;

      this.FilterList();
    });
  }

  ChangeView(event: MatButtonToggleChange): void {
    this.viewType = event.value;
    this.FilterList();
  }

  FilterList(): void {
    if (this.viewType === 'All') {
      this.Players = [...this.Batters, ...this.Pitchers];
    } else if (this.viewType === 'Batters') {
      this.Players = [...this.Batters];
    } else {
      this.Players = [...this.Pitchers];
    }
    this.Players.sort((a, b) => b.Total - a.Total);
    this.Players = this.Players.slice(0, +this.totalType);

    // get the
    this.ownerTotals = [];
    this.Players.map((p, index) => {

      let currentOwner = this.ownerTotals.find(o => o.FantasyOwner === p.FantasyOwner);
      if (!!currentOwner) {
        currentOwner.Count = currentOwner.Count + 1;
        currentOwner.IndexTotal = currentOwner.IndexTotal + (index + 1);
      } else {
        currentOwner = {
          FantasyOwner: p.FantasyOwner,
          Count: 1,
          IndexTotal: index + 1
        };
        this.ownerTotals.push(currentOwner);
      }
    });

    this.ownerTotals.sort((a, b) => b.Count - a.Count);
  }

  PositionName(pos: Position): string {
    return Position[pos];
  }

}
