import { TotalComponent } from './total/total.component';
import { StatsComponent } from './stats/stats.component';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { NgModule } from '@angular/core';
import { ExtraOptions, RouterModule, Routes } from '@angular/router';

const routerOptions: ExtraOptions = {
  scrollPositionRestoration: 'enabled',
  anchorScrolling: 'enabled',
  scrollOffset: [0, 64]
};

const routes: Routes = [
  {
    path: 'leaderboard', component: LeaderboardComponent
  },
  {
    path: 'stats', component: StatsComponent
  },
  {
    path: 'total', component: TotalComponent
  },
  {
    path: '',
    redirectTo: 'leaderboard',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, routerOptions)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
