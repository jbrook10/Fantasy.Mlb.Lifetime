import { Component, Input, OnInit } from '@angular/core';
import { Owner } from 'src/app/models/owner.model';

@Component({
  selector: 'app-player-stats',
  templateUrl: './player-stats.component.html',
  styleUrls: ['../stats.component.scss']
})
export class PlayerStatsComponent implements OnInit {

  @Input()  Owner!: Owner;
  @Input()  SmallScreen!: boolean;

  constructor() { }

  ngOnInit(): void {
  }

}
