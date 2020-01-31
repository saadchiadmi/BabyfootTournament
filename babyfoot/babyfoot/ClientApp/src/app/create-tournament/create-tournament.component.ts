import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../service/player.service';
import { Player } from '../entities/Player';
import { Team } from '../entities/Team';
import { Match } from '../entities/Match';
import { Poule } from '../entities/Poule';
import { Arbre } from '../entities/Arbre';
import { Tournament } from '../entities/Tournament';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.component.html',
  styleUrls: ['./create-tournament.component.css']
})
export class CreateTournamentComponent implements OnInit {

  players: Player[]=[];
  filteredPlayersMultiple: Player[];

  constructor(private playerservice: PlayerService, private router: Router) { }

  ngOnInit() {
  }

  filterplayerMultiple(event) {
      let query = event.query;
      this.playerservice.getPlayers().subscribe(players => {
          this.filteredPlayersMultiple = this.filterplayer(query, players);
      });
  }

  filterplayer(query, players: Player[]):Player[] {
      //in a real application, make a request to a remote url with the query and return filtered results, for demo we filter at client side
      let filtered : Player[] = [];
      for(let i = 0; i < players.length; i++) {
          let player = players[i];
          if (player.pseudo.toLowerCase().indexOf(query.toLowerCase()) == 0) {
              filtered.push(player);
          }
      }
      return filtered;
  }

  createTournament(){
    let teams : Team[] = this.generateTeams();
    let poule : Poule = {teams: teams, matchs: this.generateMatchs(teams)};
    let arbre : Arbre = this.generateArbre();
    let tournament : Tournament = {token : Date.now()+"", finish: false, poule: poule, arbre: arbre};
    this.router.navigate(["/tournament", tournament.token], { state: {tournament: tournament}});
    console.log(tournament);
  }

  generateTeams() : Team[]{
    let teams : Team[]=[];
    let result = this.players.sort((a, b) => (a.score<b.score) ? 1 : (a.score>b.score) ? -1 : 0);
    let indexToSplit = result.length / 2;
    let first : Player[] = this.shuffleArray(result.slice(0, indexToSplit));
    let second : Player[] = this.shuffleArray(result.slice(indexToSplit, result.length));
    for (let i = 0; i < indexToSplit; i++) {
        teams.push({player1 :first.pop(), player2 :second.pop(), point : 0 });
    }
    return teams;
  }

  generateMatchs(teams : Team[]) : Match[]{
    let matches : Match[] = [] ;
    let ordre : number[] = [];
    let a = teams.length-1;
    for (let i = teams.length-2; i > 0; i--) {
      a = a+i;
    }
    for (let i = 0; i < a; i++) {
      ordre.push(i);
    }
    ordre = this.shuffleArray(ordre);
    a=0;
    for (let i = 0 ; i < teams.length - 1 ; i ++)
      for (let j = i + 1 ; j < teams.length ; j ++){
        matches.push({token:Date.now()+""+ordre[a], ordre : ordre[a], team1 : teams[i], team2: teams[j], scoreTeam1Player1: 0, scoreTeam1Player2: 0, scoreTeam2Player1: 0, scoreTeam2Player2:0, finish: false}) ;
        a++;
      }
    console.log(matches);
    return matches;
  }

  generateArbre() : Arbre{
    let arbre = {} as Arbre;
    arbre.match1 = {token:Date.now()+"A1", ordre : 0, team1 : null, team2: null, scoreTeam1Player1: 0, scoreTeam1Player2: 0, scoreTeam2Player1: 0, scoreTeam2Player2:0, finish: false};
    arbre.match2 = {token:Date.now()+"A2", ordre : 0, team1 : null, team2: null, scoreTeam1Player1: 0, scoreTeam1Player2: 0, scoreTeam2Player1: 0, scoreTeam2Player2:0, finish: false};
    arbre.match3 = {token:Date.now()+"A3", ordre : 0, team1 : null, team2: null, scoreTeam1Player1: 0, scoreTeam1Player2: 0, scoreTeam2Player1: 0, scoreTeam2Player2:0, finish: false};
    return arbre;
  }

  shuffleArray(array : any[]) :any[] {
    for (var i = array.length - 1; i > 0; i--) {
        var j = Math.floor(Math.random() * (i + 1));
        var temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
    return array;
 }

}
