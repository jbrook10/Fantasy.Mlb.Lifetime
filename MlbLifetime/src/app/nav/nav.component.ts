import { AfterViewInit, Component, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay, take } from 'rxjs/operators';
import { LeagueData, YearType } from '../models/owner.model';
import { RosterService } from '../service/roster.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit, AfterViewInit {

  Year = '';
  YearType: YearType = 'Regular';
  LeagueData: LeagueData | undefined;
  route: string | undefined;
  showNames = false;

  Years: number[] = [];

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(private breakpointObserver: BreakpointObserver, private rosterService: RosterService, private router: Router) {


    const today = new Date();
    if (today.getMonth() < 3) {
      this.Year = today.getFullYear() - 1 + '';
    } else {
      this.Year = new Date().getFullYear() + '';
    }

    if (today.getMonth() < 9) {
      this.YearType = 'Regular';
    } else {
      this.YearType = 'Post';
    }

    const currentYear = today.getFullYear();
    for (let index = 2021; index <= currentYear; index++) {
      this.Years.push(index);
    }

   }

  ngOnInit(): void {

    this.loadFile();

    this.rosterService.LeagueData$.subscribe(r => this.LeagueData = r);

    this.router.events.subscribe(() => {
      this.route = this.router.url;
      this.showNames = this.route.indexOf('stats') >= 0;
    }
    );

    this.YearType = this.rosterService.YearType;
  }

  ngAfterViewInit(): void {

  }

  scrollTo(name: string): void {
    const el = document.getElementById(name);
    if (!!el) {
      el.scrollIntoView();
    }
  }

  loadFile(): void {
    this.rosterService.Year = +this.Year;
    this.rosterService.YearType = this.YearType;
    this.rosterService.GetLeagueData(+this.rosterService.Year, this.rosterService.YearType);
  }

}
