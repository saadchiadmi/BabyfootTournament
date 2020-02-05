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

    
    getTournament(): Observable<Tournament[]>
    {
        //return this.http.get<Tournament[]>(this.baseUrl + "api/tournaments");
        return this.subject;
    }

    getTournamentById(id : string): Observable<Tournament>{
        //return this.http.get<Tournament>(this.baseUrl + "api/tournaments/" + id);
        return this.subject[0];
    }


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
    mteam3: MatchTeam = { players: [{ pseudo: this.players[4].pseudo, goals: 2 }, { pseudo: this.players[5].pseudo, goals: 3 }], points: 3 };
    mteam4: MatchTeam = { players: [{ pseudo: this.players[6].pseudo, goals: 2 }, { pseudo: this.players[7].pseudo, goals: 3 }], points: 2 };
    mteam5: MatchTeam = { players: [{ pseudo: this.players[8].pseudo, goals: 2 }, { pseudo: this.players[9].pseudo, goals: 3 }], points: 1 };
    match1: Match = { token: 'semifinal1', start: new Date(Date.now() + "" + 1), order: 1, teams: [ this.mteam1, this.mteam2 ], state: "Ended", stage: "Pool", elapsed: 200 };
    match2: Match = { token: 'semifinal2', start: new Date(Date.now() + "" + 2), order: 2, teams: [this.mteam5, this.mteam3], state: "Ended", stage: "Pool", elapsed: 200 };
    match3: Match = { token: 'match3', start: new Date(Date.now() + "" + 3), order: 3, teams: [this.mteam2, this.mteam4], state: "Ended", stage: "Pool", elapsed: 200 };
    match4: Match = { token: 'match4', start: new Date(Date.now() + "" + 4), order: 4, teams: [this.mteam1, this.mteam3], state: "Ended", stage: "Pool", elapsed: 200 };
    match5: Match = { token: 'match5', start: new Date(Date.now() + "" + 5), order: 5, teams: [this.mteam2, this.mteam5], state: "Ended", stage: "Pool", elapsed: 200 };
    match6: Match = { token: 'match6', start: new Date(Date.now() + "" + 6), order: 1, teams: [this.mteam1, this.mteam3], state: "Ended", stage: "Semifinal", elapsed: 200 };
    match7: Match = { token: 'match7', start: new Date(Date.now() + "" + 7), order: 2, teams: [this.mteam2, this.mteam4], state: "Ended", stage: "Semifinal", elapsed: 250 };
    match8: Match = { token: 'match8', start: new Date(Date.now() + "" + 8), order: 0, teams: [this.mteam1, this.mteam2], state: "Ended", stage: "Final", elapsed: 200 };

    team1: TournamentTeam = { pseudos: [this.players[0].pseudo, this.players[1].pseudo], points: 7 };
    team2: TournamentTeam = { pseudos: [this.players[2].pseudo, this.players[3].pseudo], points: 5 };
    team3: TournamentTeam = { pseudos: [this.players[4].pseudo, this.players[5].pseudo], points: 3 };
    team4: TournamentTeam = { pseudos: [this.players[6].pseudo, this.players[7].pseudo], points: 2 };
    team5: TournamentTeam = { pseudos: [this.players[8].pseudo, this.players[9].pseudo], points: 1 };
    teams: TournamentTeam[] = [this.team1, this.team2, this.team3, this.team4, this.team5];

    matches: Match[] = [this.match1, this.match2, this.match3, this.match4, this.match5, this.match6, this.match7, this.match8];

    tournament: Tournament = { token : "saad1234", date : new Date(Date.now()), state: "Ended", teams: this.teams, matches: this.matches };
    subject: BehaviorSubject<Tournament[]> = new BehaviorSubject([this.tournament]);
}
