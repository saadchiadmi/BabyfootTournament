import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TournamentComponent } from './tournament/tournament.component';
import { PlayersComponent } from './players/players.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CreateTournamentComponent } from './create-tournament/create-tournament.component';
import { OneTournamentComponent } from './one-tournament/one-tournament.component';
import { MatchComponent } from './match/match.component';


const routes: Routes = [
  { path: 'tournaments', component: TournamentComponent },
  { path: 'tournament/:id', component: OneTournamentComponent },
  { path: 'match/:id', component: MatchComponent },
  { path: 'create', component: CreateTournamentComponent },
  { path: 'players',        component: PlayersComponent },
  { path: 'dashboard',        component: DashboardComponent },
  { path: '',   redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', component: DashboardComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
