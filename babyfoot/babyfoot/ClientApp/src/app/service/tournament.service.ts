import { Injectable, Inject } from '@angular/core';
import { Tournament } from '../entities/Tournament';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Player } from '../entities/Player';
import { TeamService } from './team.service';
import { TournamentTeam } from '../entities/TournamentTeam';
import { MatchPlayer } from '../entities/MatchPlayer';
import { MatchTeam } from '../entities/MatchTeam';
import { Match } from '../entities/Match';
@Injectable({
  providedIn: 'root'
})
export class TournamentService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    URI: string = "https://localhost:44324/";
    getTournament(): Observable<Tournament[]>
    {
        return this.http.get<Tournament[]>(this.URI + "api/tournaments");
    }

    getTournamentById(id : string): Observable<Tournament>{
        return this.http.get<Tournament>(this.URI + "api/tournaments/" + id);
    }

    saveTournament(tournament: Tournament): Observable<Tournament> {
        return this.http.put<Tournament>(this.URI + "api/tournaments", tournament);
    }


 
}
