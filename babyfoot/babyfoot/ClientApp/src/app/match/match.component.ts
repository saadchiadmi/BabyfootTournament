import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatchService } from '../service/match.service';
import { Match, getFinalMatch, getSemifinalMatch, getPoolMatch, getWinnerTeam } from '../entities/Match';
import {ConfirmationService} from 'primeng/api';
import { Tournament } from '../entities/Tournament';
import { TournamentTeam, getTeam, toMatchTeam } from '../entities/TournamentTeam';
import { MatchTeam, getScore, newMatchTeam } from '../entities/MatchTeam';
import { MatchPlayer } from '../entities/MatchPlayer';

@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.css'],
  providers: [ConfirmationService]
})
export class MatchComponent implements OnInit {

    id: string;
    tournament: Tournament;
    selectedMatch: Match;
    updateScore: boolean = false;
    max_goals: number = 5;

    constructor(private confirmationService: ConfirmationService, private activatedRoute: ActivatedRoute, private matchService: MatchService, private router: Router) { }

    ngOnInit() {
        this.tournament = JSON.parse(localStorage.getItem('tournament'));
        if(this.tournament) 
            this.activatedRoute.params.subscribe(params => {
                this.id = params['id'];
                this.selectedMatch = this.getMatch(this.id);
                if (!this.selectedMatch) {
                    this.matchService.getMatchById(this.id).subscribe(res => {
                        this.selectedMatch = res;
                    });
                }
            });
        else 
            this.matchService.getMatchById(this.id).subscribe(res => {
                this.selectedMatch = res;
            });
        console.log(this.selectedMatch);
    }

    addGoal(i: number, j: number)
    {
        this.selectedMatch.teams[i][j].goals = this.selectedMatch.teams[i][j].goals + 1;
        let t1 = this.selectedMatch.teams[0];
        let t2 = this.selectedMatch.teams[1];

        if (t1[0].goals + t1[1].goals == this.max_goals)
        {
            //put match
            this.selectedMatch.state = "Ended";
        }
        else if (t2[0].goals + t2[1].goals == this.max_goals)
        {
            //put match
            this.selectedMatch.state = "Ended";
        }
        this.putMatch(this.selectedMatch);
        if (this.selectedMatch.state === "Ended")
        {
            this.confirmationService.confirm({
                message: 'Are you sure that you want to finish the match',
                header: 'Confirmation',
                icon: 'pi pi-exclamation-triangle',
                accept: () =>
                {
                    this.compute();
                    console.log(this.tournament);
                    //put tournament
                    this.router.navigate(["/tournament", this.tournament.token], { state: {tournament: this.tournament}});
                },
                reject: () =>
                {
                    this.selectedMatch.state === "InProgress";
                    this.dropGoal(i, j);
                }
            });
        }
    }

    dropGoal(i: number, j: number) {
        if (this.selectedMatch.teams[i][j].goals != 0)
            this.selectedMatch.teams[i][j].goals = this.selectedMatch.teams[i][j].goals - 1; 
    }

    getMatch(token: string): Match
    {
        this.tournament.matches.filter(m => m.token === token)[0].start = new Date(Date.now());
        let match: Match = this.tournament.matches.filter(m => m.token === token)[0];
        return match;
    }

    checkIfFinishPoule(): boolean
    {
        return this.tournament.matches.filter(m => m.stage == "Pool").filter(m => m.state === "Ended").length == this.tournament.matches.filter(m => m.stage == "Pool").length ? true : false;
    }

    checkIfFinishSemiFinal(): boolean
    {
        return this.tournament.matches.filter(m => m.stage === "Semifinal" && m.order == 1 && m.state === "Ended") && this.tournament.matches.filter(m => m.stage === "Semifinal" && m.order == 2 && m.state === "Ended") ? true : false;
    }

    putMatch(match: Match)
    {
        let match_before : Match =  this.tournament.matches.filter(m => m.token === match.token)[0];
        let index = this.tournament.matches.indexOf(match_before);
        this.tournament.matches[index] = this.selectedMatch;
    }

    sortPoule() {
        this.tournament.teams = this.tournament.teams.sort((n1, n2) => {
            if (n1.points > n2.points) return -1;
            if (n1.points < n2.points) return 1;
            return 0;
        });
    }

    timesUp(event)
    { 
        if (event.action == "done")
        { 
            this.finish();
        }
    }

    finish()
    {
        this.confirmationService.confirm({
            message: 'Are you sure that you want to finish the match',
            header: 'Confirmation',
            icon: 'pi pi-exclamation-triangle',
            accept: () =>
            {
                this.selectedMatch.state = "Ended";
                this.compute();
                //put tournament
                this.router.navigate(["/tournament", this.tournament.token], { state: {tournament: this.tournament}});
            },
            reject: () =>
            {
                this.selectedMatch.state = "InProgress";
                this.updateScore=true;
            }
        });
     }

    compute()
    {
        if (this.selectedMatch.stage === "Pool")
        {
            let team1: TournamentTeam = getTeam(this.selectedMatch.teams[0], this.tournament.teams);
            let team2: TournamentTeam = getTeam(this.selectedMatch.teams[1], this.tournament.teams);
            let indexTeam1 = this.tournament.teams.indexOf(team1);
            let indexTeam2 = this.tournament.teams.indexOf(team2);
            let winner_team = getWinnerTeam(this.selectedMatch);
            if (winner_team == this.selectedMatch.teams[0])
            {
                this.tournament.teams[indexTeam1].points = this.tournament.teams[indexTeam1].points + 3;
                this.tournament.teams[indexTeam2].points = this.tournament.teams[indexTeam2].points + 1;
            }
            else if (winner_team == this.selectedMatch.teams[1])
            {
                this.tournament.teams[indexTeam1].points = this.tournament.teams[indexTeam1].points + 1;
                this.tournament.teams[indexTeam2].points = this.tournament.teams[indexTeam2].points + 3;
            }
            else
            {
                this.tournament.teams[indexTeam1].points = this.tournament.teams[indexTeam1].points + 2;
                this.tournament.teams[indexTeam2].points = this.tournament.teams[indexTeam2].points + 2;
            }

            if (this.tournament.matches.filter(m => m.state === "Ended").length == this.tournament.matches.length)
            {
                this.sortPoule();
                let semifinal1: Match =
                {
                    token: Date.now() + "" + 1,
                    start: null,
                    order: 1,
                    state: "NotStarted",
                    stage: "Semifinal",
                    elapsed: 0,
                    teams: [[{ pseudo: this.tournament.teams[0].pseudos[0], goals: 0 },
                        { pseudo: this.tournament.teams[0].pseudos[1], goals: 0 }],
                        [{ pseudo: this.tournament.teams[2].pseudos[0], goals: 0 },
                            { pseudo: this.tournament.teams[2].pseudos[1], goals: 0 }]]
                };
                let semifinal2: Match =
                {
                    token: Date.now() + "" + 2,
                    start: null,
                    order: 2,
                    state: "NotStarted",
                    stage: "Semifinal",
                    elapsed: 0,
                    teams: [[{ pseudo: this.tournament.teams[1].pseudos[0], goals: 0 },
                    { pseudo: this.tournament.teams[1].pseudos[1], goals: 0 }],
                    [{ pseudo: this.tournament.teams[3].pseudos[0], goals: 0 },
                    { pseudo: this.tournament.teams[3].pseudos[1], goals: 0 }]]
                };
                this.tournament.matches.push(semifinal1);
                this.tournament.matches.push(semifinal2);

                /*
                this.sortPoule();
                this.tournament.semifinal1.team1 = this.tournament.teams[0];
                this.tournament.semifinal1.team2 = this.tournament.teams[2];
                this.tournament.semifinal2.team1 = this.tournament.teams[1];
                this.tournament.semifinal2.team2 = this.tournament.teams[3];*/
            }
        }
        else if (this.selectedMatch.stage === "Semifinal")
        {
            if (this.tournament.matches.filter(m => m.stage == "Semifinal").filter(m => m.state == "Ended")[0]) {
                if (this.tournament.matches.filter(m => m.stage == "Semifinal").filter(m => m.state == "Ended")[1]) {
                    let winners: MatchPlayer[][] = [
                        getWinnerTeam(getSemifinalMatch(this.tournament.matches, 1)),
                        getWinnerTeam(getSemifinalMatch(this.tournament.matches, 2))]
                    let winner1 = getWinnerTeam(getSemifinalMatch(this.tournament.matches, 1));
                    let final: Match =
                    {
                        token: Date.now() + "" + 0,
                        start: null,
                        order: 0,
                        state: "NotStarted",
                        stage: "Final",
                        elapsed: 0,
                        teams: [newMatchTeam(winners[0]), newMatchTeam(winners[1])]
                    };
                    this.tournament.matches.push(final);
                }
            }
        }
     }

}
