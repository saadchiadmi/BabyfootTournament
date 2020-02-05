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
    mteam1: MatchTeam = { players: [{ pseudo: this.players[0].pseudo, goals: 2 }, { pseudo: this.players[1].pseudo, goals: 3 }], points: 7 };
    mteam2: MatchTeam = { players: [{ pseudo: this.players[2].pseudo, goals: 2 }, { pseudo: this.players[3].pseudo, goals: 3 }], points: 5 };
    test_match: Match = { token: 'hellomatch', start: new Date(Date.now()), order: 1, teams: [this.mteam1, this.mteam2], state: "Ended", stage: "Pool", elapsed: 200 };
    subject: BehaviorSubject<Match> = new BehaviorSubject(this.test_match);

    getMatchById(id: string): Observable<Match> {
        return this.subject.asObservable();
        //return this.http.get<Match>(this.baseUrl + "api/players");
    }
}
