import { Pitcher } from 'src/app/models/owner.model';
import { Batter } from './../models/owner.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  constructor() { }

  public GetBatterPoints(batter: Batter): number {
    return batter.H + batter.R + batter.HR + batter.RBI + batter.SB + batter.BB;
  }

  public GetPitcherPoints(pitcher: Pitcher): number {
    return (pitcher.W * 4) + (pitcher.SV * 5) + pitcher.IP + pitcher.SO;
  }
}
