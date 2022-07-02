import { Pitcher } from 'src/app/models/owner.model';
import { LeagueData } from './../models/owner.model';
import { RosterService } from './../service/roster.service';
import { Component, OnInit } from '@angular/core';
import { Owner } from '../models/owner.model';
import { take } from 'rxjs/operators';
import { PlayerService } from '../service/player.service';

@Component({
  selector: 'app-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.scss']
})
export class LeaderboardComponent implements OnInit {

  LeagueData: LeagueData | undefined;
  Owners: Owner[] = [];
  SortedOwners: Owner[] = [];
  Probables: Pitcher[] = [];
  columns = ['Name', 'BatterScore', 'PitcherScore', 'TotalScore'];

  constructor(private rosterService: RosterService, private playerService: PlayerService) { }

  ngOnInit(): void {

    this.rosterService.LeagueData$.subscribe(r => {
      this.LeagueData = r;
      this.Owners = this.LeagueData.Owners;
      this.SortedOwners = this.Owners.sort((a, b) => (b.BatterScore + b.PitcherScore) - (a.BatterScore + a.PitcherScore));
      this.Probables = this.Owners.flatMap(o => o.Pitchers.filter(p => p.Probable)) as Pitcher[];
    });
}

}


