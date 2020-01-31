import { Injectable, Inject } from '@angular/core';
import { Tournament } from '../entities/Tournament';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Player } from '../entities/Player';
import { TeamService } from './team.service';
import { Team } from '../entities/Team';
import { Match } from '../entities/Match';
import { Poule } from '../entities/Poule';
import { Arbre } from '../entities/Arbre';
@Injectable({
  providedIn: 'root'
})
export class TournamentService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    
    getTournament(): Observable<Tournament[]> {
        //return this.http.get<Tournament[]>(this.baseUrl + "api/tournaments");
        return this.tounament;
  }

  getTournamentById(id : string): Observable< Tournament>{
      //return this.http.get<Tournament>(this.baseUrl + "api/tournaments/" + id);
      return this.tounament[0];
    }


    players: Player[] = [
        { pseudo: "saad", score: 45, goals: 35, champions: 3 },
        { pseudo: "saad1", score: 50, goals: 47, champions: 4 },
        { pseudo: "thomas", score: 25, goals: 35, champions: 2 },
        { pseudo: "thomas1", score: 10, goals: 10, champions: 1 },
        { pseudo: "stbe", score: 30, goals: 20, champions: 2 },
        { pseudo: "stbe1", score: 37, goals: 27, champions: 2 },
        { pseudo: "str", score: 30, goals: 20, champions: 2 },
        { pseudo: "str1", score: 37, goals: 27, champions: 2 },
        { pseudo: "wesh", score: 30, goals: 20, champions: 2 },
        { pseudo: "wesh1", score: 37, goals: 27, champions: 2 },
    ];


    team1: Team = { player1: this.players[0], player2: this.players[1], point: 7 };
    team2 : Team = {player1 : this.players[2], player2 : this.players[3], point : 5};
    team3 : Team = {player1 : this.players[4], player2 : this.players[5], point : 3};
    team4 : Team = {player1 : this.players[6], player2 : this.players[7], point : 2};
    team5: Team = { player1: this.players[8], player2: this.players[9], point: 1 };
    match1: Match = { token: 'match1', ordre: 1, team1: this.team1, team2: this.team2, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2: 0, finish: true };
    match2 : Match = {token: 'match2', ordre : 2, team1 : this.team5, team2: this.team3, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2:0, finish: true};
    match3 : Match = {token: 'match3', ordre : 3, team1 : this.team2, team2: this.team4, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2:0, finish: true};
    match4 : Match = {token: 'match4', ordre : 4, team1 : this.team1, team2: this.team3, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2:0, finish: true};
    match5 : Match = {token: 'match5', ordre : 5, team1 : this.team2, team2: this.team5, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2:0, finish: true};
    poule : Poule = {teams : [this.team1, this.team2, this.team3, this.team4, this.team5],
                    matchs : [this.match1, this.match2, this.match3, this.match4, this.match5]
    };

    match6 : Match = {token: 'match6', ordre : 0, team1 : this.team1, team2: this.team3, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2:0, finish: true};
    match7 : Match = {token: 'match7', ordre : 0, team1 : this.team2, team2: this.team4, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2:0, finish: true};
    match8 : Match = {token: 'match8', ordre : 0, team1 : this.team1, team2: this.team2, scoreTeam1Player1: 2, scoreTeam1Player2: 3, scoreTeam2Player1: 2, scoreTeam2Player2:0, finish: true};
    arbre : Arbre = {match1 : this.match6, match2 : this.match7, match3 : this.match8};

    tounament : BehaviorSubject<Tournament[]> = new BehaviorSubject([
    {token : "saad1234", /*date : new Date(2019, 10, 21),*/ finish : true, poule : this.poule, arbre : this.arbre}
    ]);
}
