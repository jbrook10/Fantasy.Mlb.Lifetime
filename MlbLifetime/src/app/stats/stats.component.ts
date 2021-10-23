import { RosterService } from './../service/roster.service';
import { LeagueData, Owner } from 'src/app/models/owner.model';
import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';


@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.scss']
})
export class StatsComponent implements OnInit {

  LeagueData: LeagueData | undefined;
  Owners: Owner[] = [];
  loading = true;
  SmallScreen = false;

  constructor(private rosterService: RosterService, breakpointObserver: BreakpointObserver) {
    breakpointObserver.observe([
      Breakpoints.Small,
      Breakpoints.XSmall,
    ]).subscribe(result => {
      this.SmallScreen = result.matches;
    });

  }

  ngOnInit(): void {
    this.rosterService.LeagueData$.subscribe(r => {
      console.log(r);
      this.LeagueData = r;
      this.Owners = this.LeagueData.Owners.sort((a, b) => (b.BatterScore + b.PitcherScore) - (a.BatterScore + a.PitcherScore));
      this.loading = false;
    });
  }

  scrollTo(name: string): void {
    const el = document.getElementById(name);
    if (!!el) {
      el.scrollIntoView();
    }
  }

}
