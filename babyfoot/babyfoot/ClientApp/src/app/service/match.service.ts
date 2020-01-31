import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Match } from '../entities/Match';
import { Player } from '../entities/Player';
import { Team } from '../entities/Team';

@Injectable({
  providedIn: 'root'
})
export class MatchService {

    constructor(private http: HttpClient) { }

    player1: Player = { pseudo: "saad", score: 45, goals: 35, champions: 3 };
    player2: Player = { pseudo: "saad1", score: 50, goals: 47, champions: 4 };
    player3: Player = { pseudo: "thomas", score: 25, goals: 35, champions: 2 };
    player4: Player = { pseudo: "thomas1", score: 10, goals: 10, champions: 1 };
    team1: Team = { player1: this.player1, player2: this.player2, point: 7 };
    team2: Team = { player1: this.player3, player2: this.player4, point: 5 };
    match1: BehaviorSubject<Match> = new BehaviorSubject( { id: 2, ordre: 1, team1: this.team1, team2: this.team2, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2: 0, finish: true, token: null, niveau: null, winner: null });

    getMatchById(id: string): Observable<Match> {
        return this.match1.asObservable();
        //return this.http.get<Match>(this.baseUrl + "api/players");
    }
}
