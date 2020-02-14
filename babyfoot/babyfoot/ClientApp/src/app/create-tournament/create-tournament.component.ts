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
        //this.tournamentservice.saveTournament(tournament).subscribe();
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

    generateOrderedMatches(teams: TournamentTeam[]): Match[]
    {
        let matches : Match[] = [];
        let ordre : number[] = [];
        let a = teams.length-1;
        for (let i = teams.length - 2; i > 0; i--)
        {
            a = a + i;
        }
        for (let i = 0; i < a; i++)
        {
            ordre.push(i);
        }
        ordre = this.shuffleArray(ordre);
        a = 0;
        for (let i = 0; i < teams.length - 1; i++)
        {
            for (let j = i + 1; j < teams.length; j++)
            {
                let match: Match =
                {
                    token: Date.now() + "" + ordre[a],
                    start: null,
                    order: ordre[a],
                    state: "NotStarted",
                    stage: "Pool",
                    elapsed: 0,
                    //teams: [toMatchTeam(teams[i]), toMatchTeam(teams[j])]
                    teams: [[
                        { pseudo: teams[i].pseudos[0], goals: 0 },
                        { pseudo: teams[i].pseudos[1], goals: 0 }],
                        [
                            { pseudo: teams[j].pseudos[0], goals: 0 },
                            { pseudo: teams[j].pseudos[1], goals: 0 }]
                    ]
                };
                matches.push(match);
                a++;
            }
        }
        return matches;
    }

    shuffleArray(array : any[]): any[]
    {
        for (var i = array.length - 1; i > 0; i--) {
            var j = Math.floor(Math.random() * (i + 1));
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }


    generateBetterOrderedMatches(teams: TournamentTeam[]): Match[] {
        let k = 2;
        let n = teams.length;
        let m = n - 1;
        var unrelated_tuples = this.getUnrelatedTuples(k, n, m);

        let matches: Match[]=[];
        let order = 1;
        for (var tuple of unrelated_tuples) {
            let match: Match =
            {
                token: Date.now() + "" + order,
                start: null,
                order: order,
                state: "NotStarted",
                stage: "Pool",
                elapsed: 0,
                teams: [
                    [
                        { pseudo: teams[tuple[0]].pseudos[0], goals: 0 },
                        { pseudo: teams[tuple[0]].pseudos[1], goals: 0 }
                    ],
                    [
                        { pseudo: teams[tuple[1]].pseudos[0], goals: 0 },
                        { pseudo: teams[tuple[1]].pseudos[1], goals: 0 }
                    ]
                ]
                //teams: [toMatchTeam(teams[tuple[0]]), toMatchTeam(teams[tuple[1]])]
            };
            matches.push(match);

            ++order;
        }
        return matches;
    }


    range(start, end) :number[] {
        let tab: number[] = [];
        for (var i = start; i <= end; i++) {
            tab.push(i);
        }
        return tab;
    }

    getIncrementalOrderedTuples(k: number, n: number): number[][] {
        let tuples: number[][]=[];
        let tuple = this.range(0, k - 1);

        let reset_identity = (j: number) => {
            for (var i of this.range(0, j - 1)) {
                tuple[i] = i;
            }
        }

        tuples.push(tuple);

        if (n > k) {
            let last_i = k - 1;
            let i = k - 1;

            while (tuple[0] != n - k) {
                if (i == last_i) {
                    reset_identity(i);
                    ++tuple[i];
                    --i;
                }
                else if (tuple[i] + 1 == tuple[i + 1]) {
                    if (i == 0) {
                        i = last_i;
                        reset_identity(i);
                        ++tuple[i];
                        --i;
                    }
                    else {
                        reset_identity(i);
                        --i;
                        ++tuple[i];
                    }
                }
                else
                    ++tuple[i];


                tuples.push(tuple);
            }
        }
        return tuples;
    }

    linearizedIndex(v: number[], n: number): number {
        let res = 0;
        let p = 1;
        for (var k of v) {
            res += +k * p;
            p *= n;
        }
        return res;
    }

    getUnrelatedTuples(k: number, n: number, m: number): number[][] {

        let result: number[][]=[];

        let nb_tuples = (n * m) / k;
        let nb_visited = new Array<number>(n);
        let last = new Array<number>(n);
        let tuple_visited = new Array<boolean>(Math.pow(n, k));


        for (var i = nb_visited.length - 1; i > 0; i--) {
            nb_visited.push(0);
        }
        for (var i = last.length - 1; i > 0; i--) {
            last.push(0);
        }
        for (var i = tuple_visited.length - 1; i > 0; i--) {
            tuple_visited.push(false);
        }


        let compare_last = (a: number, b: number): number => {
            if (last[a] < last[b])
                return -1;
            else if (last[a] == last[b]) {
                if (nb_visited[a] < nb_visited[b])
                    return -1;
                else if (nb_visited[a] == nb_visited[b])
                    return 0;
                else
                    return 1;
            }
            else
                return 1;
        };

        let compare_visited = (a: number, b: number): number => {
            if (nb_visited[a] < nb_visited[b])
                return -1;
            else if (nb_visited[a] == nb_visited[b]) {
                if (last[a] < last[b])
                    return -1;
                else if (last[a] == last[b])
                    return 0;
                else
                    return 1;
            }
            else
                return 1;
        };

        for (var i of this.range(1, nb_tuples)) {
            let tuple = new Array<number>(k);
            let ordered_tuple = new Array<number>(k);

            // prioritize visited compare for the last 2 matches, to ensure that anyone is left alone at (m - 2) matches
            let compare = (nb_tuples - +i) < 3 ? compare_visited : compare_last;

            let best = this.range(0, n - 1);
            best.sort(compare);

            let tuples: number[][] = this.getIncrementalOrderedTuples(k, n);

            for (var indexes of tuples) {
                let go_next = false;
                let i_t = 0;
                for (var b of indexes) {
                    if (nb_visited[best[b]] == m) {
                        go_next = true;
                        break;
                    }
                    tuple[i_t++] = best[b];
                }
                if (go_next)
                    continue;
                ordered_tuple = tuple.slice(0, k);
                ordered_tuple.sort();
                if (!tuple_visited[this.linearizedIndex(ordered_tuple, n)])
                    break;
            }
            // if no non-visited tuple is found, we take a visited one
            if (tuple_visited[this.linearizedIndex(ordered_tuple, n)]) {
                let found = false;

                let tuples2: number[][] = this.getIncrementalOrderedTuples(k, n);
                for (indexes of tuples2) {
                    let go_next = false;
                    let i_t = 0;
                    for (var b of indexes) {
                        if (nb_visited[best[b]] == m) {
                            go_next = true;
                            break;
                        }
                        tuple[i_t++] = best[b];
                    }
                    if (go_next)
                        continue;
                    ordered_tuple = tuple.slice(0, k);
                    ordered_tuple.sort();
                    found = true;
                    break;
                }
            }

            for (var v of tuple) {
                last[v] = i;
                ++nb_visited[v];
            }

            tuple_visited[this.linearizedIndex(ordered_tuple, n)] = true;

            result.push(ordered_tuple);
        }
        return result;
    }

}
