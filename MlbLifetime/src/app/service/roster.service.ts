import { YearType, LeagueData } from './../models/owner.model';
import { Owner } from 'src/app/models/owner.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, ReplaySubject, Subject } from 'rxjs';
import { map, publishReplay, refCount, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class RosterService {

  public LeagueData$ = new ReplaySubject<LeagueData>(1);

  private year: number = new Date().getFullYear();
  public get Year(): number {
    return this.year;
  }
  public set Year(year: number) {
    this.year = year;
    console.log(year);
  }

  private yearType: YearType = 'Regular';
  public get YearType(): YearType {
    return this.yearType;
  }
  public set YearType(value: YearType) {
    this.yearType = value;
  }

  constructor(private http: HttpClient) { }


  public GetLeagueData(year: number, type: YearType): void {

    this.year = year;
    this.yearType = type;

    const typeText = type === 'Regular' ? 'Regular' : 'Post';

    this.http.get<LeagueData>(`assets/${year}.${typeText}.Data.json`).subscribe(d => this.LeagueData$.next(d));

  }


}
