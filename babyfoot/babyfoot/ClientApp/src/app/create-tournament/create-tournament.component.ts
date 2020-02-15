import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../service/player.service';
import { Player } from '../entities/Player';
import { TournamentTeam, toMatchTeam } from '../entities/TournamentTeam';
import { Match } from '../entities/Match';
import { Tournament } from '../entities/Tournament';
import { Router } from '@angular/router';
import { TournamentService } from '../service/tournament.service';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.component.html',
  styleUrls: ['./create-tournament.component.css']
})
export class CreateTournamentComponent implements OnInit {

    players: Player[]=[];
    filteredPlayersMultiple: Player[];

    constructor(private playerservice: PlayerService, private tournamentservice: TournamentService, private router: Router) { }

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

    createTournament()
    {
        let teams: TournamentTeam[] = this.generateRandomBalancedTeams();
        let matches: Match[] = this.generateOrderedMatches(teams);
        let tournament: Tournament =
        {
                token: Date.now() + "",
                date: new Date(Date.now()),
                state: "Pool",
                teams: teams,
                matches: matches,
        };
        
        this.router.navigate(["/tournament", tournament.token], { state: {tournament: tournament}});
        this.tournamentservice.saveTournament(tournament).subscribe();
        console.log(tournament);
    }

    generateRandomBalancedTeams(): TournamentTeam[]
    {
        let teams: TournamentTeam[] = [];
        let result = this.players.sort((a, b) => (a.score < b.score) ? 1 : (a.score > b.score) ? -1 : 0);
        let indexToSplit = result.length / 2;
        let first : Player[] = this.shuffleArray(result.slice(0, indexToSplit));
        let second : Player[] = this.shuffleArray(result.slice(indexToSplit, result.length));
        for (let i = 0; i < indexToSplit; i++)
        {
            let player1 = first.pop();
            let player2 = second.pop();
            let team: TournamentTeam = { pseudos: [player1.pseudo, player2.pseudo], points: 0 };
            teams.push(team);
        }
        return teams;
    }

    generateBalancedTeams(): TournamentTeam[]
    {
        return [];
    }


    generateOrderedMatches(teams: TournamentTeam[]): Match[] {
        let matches: Match[] = [];
        let ordre: number[] = [];
        let a = teams.length - 1;
        for (let i = teams.length - 2; i > 0; i--) {
            a = a + i;
        }
        for (let i = 0; i < a; i++) {
            ordre.push(i);
        }
        ordre = this.shuffleArray(ordre);
        a = 0;
        for (let i = 0; i < teams.length - 1; i++) {
            for (let j = i + 1; j < teams.length; j++) {
                let match: Match =
                {
                    token: Date.now() + "" + ordre[a],
                    start: null,
                    order: ordre[a],
                    state: "NotStarted",
                    stage: "Pool",
                    elapsed: 0,
                    teams: [toMatchTeam(teams[i]), toMatchTeam(teams[j])]
                };
                matches.push(match);
                a++;
            }
        }
        return matches;
    }

    shuffleArray(array: any[]): any[] {
        for (var i = array.length - 1; i > 0; i--) {
            var j = Math.floor(Math.random() * (i + 1));
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }




}
