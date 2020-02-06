import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Match } from '../entities/Match';
import { MatchTeam } from '../entities/MatchTeam';
import { Player } from '../entities/Player';
import { TournamentTeam } from '../entities/TournamentTeam';

@Injectable({
  providedIn: 'root'
})
export class MatchService {

    constructor(private http: HttpClient) { }
    players: Player[] = [
        { pseudo: "thomas", score: 45, goals: 35, champions: 3 },
        { pseudo: "pierre", score: 50, goals: 47, champions: 4 },
        { pseudo: "saad", score: 25, goals: 35, champions: 2 },
        { pseudo: "antoine", score: 10, goals: 10, champions: 1 },
        { pseudo: "philippe", score: 30, goals: 20, champions: 2 },
        { pseudo: "jean", score: 37, goals: 27, champions: 2 },
        { pseudo: "alexis", score: 30, goals: 20, champions: 2 },
        { pseudo: "jeff", score: 37, goals: 27, champions: 2 },
        { pseudo: "antoinette", score: 30, goals: 20, champions: 2 },
        { pseudo: "valentin", score: 37, goals: 27, champions: 2 },
    ];
    URI: string = "https://localhost:44324/";
    getMatchById(id: string): Observable<Match> {
        return this.http.get<Match>(this.URI + "api/matches"+id);
    }
}
