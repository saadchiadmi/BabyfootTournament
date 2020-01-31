import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatchService } from '../service/match.service';
import { Match } from '../entities/Match';
import {ConfirmationService} from 'primeng/api';
import { Tournament } from '../entities/Tournament';
import { Team } from '../entities/Team';

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
    }

    addGoal(s: string) {
        this.selectedMatch[s] = this.selectedMatch[s] + 1; 
        if (this.selectedMatch.scoreTeam1Player1+this.selectedMatch.scoreTeam1Player2==5) {
            //put match
            this.selectedMatch.finish = true;
        }else
        if (this.selectedMatch.scoreTeam2Player1+this.selectedMatch.scoreTeam2Player2==5) {
            //put match
            this.selectedMatch.finish = true;
        }
        this.putMatch(this.selectedMatch);
        if (this.selectedMatch.finish==true) {
            this.confirmationService.confirm({
                message: 'Are you sure that you want to finish the match',
                header: 'Confirmation',
                icon: 'pi pi-exclamation-triangle',
                accept: () => {
                    this.compute();
                    //put tournament
                    this.router.navigate(["/tournament", this.tournament.token], { state: {tournament: this.tournament}});
                },
                reject: () => {
                    this.selectedMatch.finish = false;
                    this.dropGoal(s);
                }
            });
        }
    }

    dropGoal(s: string) {
        if (this.selectedMatch[s] != 0)
          this.selectedMatch[s] = this.selectedMatch[s] - 1; 
    }

    getMatch(id : string): Match{
        let matchPoule : Match =  this.tournament.poule.matchs.filter(m => m.token === id).find(m=>true);
        let matchArbre : Match;
        if (!matchPoule) {
            if (this.tournament.arbre.match1.token === id) {
                matchArbre = this.tournament.arbre.match1;
            }else if (this.tournament.arbre.match2.token === id) {
                matchArbre = this.tournament.arbre.match2;
            }else if (this.tournament.arbre.match3.token === id) {
                matchArbre = this.tournament.arbre.match3;
            }
            if (matchArbre)
                return matchArbre;
            else return null
        }else{
            return matchPoule
        }
    }

    checkIfMatchInPoule(match : Match) : Match{
        let matchPoule : Match =  this.tournament.poule.matchs.filter(m => m.token === match.token).find(m=>true);
        if(matchPoule){
            return matchPoule
        }else 
            return null;
    }

    checkIfFinishPoule() : boolean{
        return this.tournament.poule.matchs.filter(m => m.finish==true).length == this.tournament.poule.matchs.length ? true : false;
    }

    checkIfFinishSemiFinal() : boolean{
        return this.tournament.arbre.match1.finish && this.tournament.arbre.match2.finish ? true : false;
    }

    putMatch(match : Match){
        let matchPoule : Match =  this.tournament.poule.matchs.filter(m => m.token === match.token).find(m=>true);
        let matchArbre : Match;
        if (!matchPoule) {
            if (this.tournament.arbre.match1.token === match.token) {
                matchArbre = this.tournament.arbre.match1;
                let index = this.tournament.poule.matchs.indexOf(matchArbre);
                this.tournament.arbre.match1 = matchArbre;
            }else if (this.tournament.arbre.match2.token === match.token) {
                matchArbre = this.tournament.arbre.match2;
                let index = this.tournament.poule.matchs.indexOf(matchArbre);
                this.tournament.arbre.match2 = matchArbre;
            }else if (this.tournament.arbre.match3.token === match.token) {
                matchArbre = this.tournament.arbre.match3;
                let index = this.tournament.poule.matchs.indexOf(matchArbre);
                this.tournament.arbre.match3 = matchArbre;
            }
        }else{
            let index = this.tournament.poule.matchs.indexOf(matchPoule);
            this.tournament.poule.matchs[index] = this.selectedMatch;
        }
    }

    getMatchWinner(match : Match) : Team{
        if(match.scoreTeam1Player1 + match.scoreTeam1Player2 > match.scoreTeam2Player1 + match.scoreTeam2Player2)
            return match.team1;
        else if (match.scoreTeam1Player1 + match.scoreTeam1Player2 < match.scoreTeam2Player1 + match.scoreTeam2Player2) {
            return match.team2;
        }
            return null;
    }

    sortPoule() {
        this.tournament.poule.teams = this.tournament.poule.teams.sort((n1, n2) => {
            if (n1.point > n2.point) return -1;
            if (n1.point < n2.point) return 1;
            return 0;
        });
    }

    timesUp(event) { 
        if (event.action == "done") { 
            this.finish();
        }
     }

     finish(){
        this.confirmationService.confirm({
            message: 'Are you sure that you want to finish the match',
            header: 'Confirmation',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.selectedMatch.finish=true;
                this.compute();
                //put tournament
                this.router.navigate(["/tournament", this.tournament.token], { state: {tournament: this.tournament}});
            },
            reject: () => {
                this.selectedMatch.finish = false;
                this.updateScore=true;
            }
        });
     }

     compute(){
        if (this.checkIfMatchInPoule(this.selectedMatch)) {
            let team1 : Team = this.tournament.poule.teams.filter(t => t.player1.pseudo === this.selectedMatch.team1.player1.pseudo).find(t=>true);
            let team2 : Team = this.tournament.poule.teams.filter(t => t.player1.pseudo === this.selectedMatch.team2.player1.pseudo).find(t=>true);
            let indexTeam1 = this.tournament.poule.teams.indexOf(team1);
            let indexTeam2 = this.tournament.poule.teams.indexOf(team2);
            if (this.getMatchWinner(this.selectedMatch) == this.selectedMatch.team1) {
                this.tournament.poule.teams[indexTeam1].point = this.tournament.poule.teams[indexTeam1].point + 3;
                this.tournament.poule.teams[indexTeam2].point = this.tournament.poule.teams[indexTeam2].point + 1;
            }else if (this.getMatchWinner(this.selectedMatch) == this.selectedMatch.team2) {
                this.tournament.poule.teams[indexTeam1].point = this.tournament.poule.teams[indexTeam1].point + 1;
                this.tournament.poule.teams[indexTeam2].point = this.tournament.poule.teams[indexTeam2].point + 3;
            }else{
                this.tournament.poule.teams[indexTeam1].point = this.tournament.poule.teams[indexTeam1].point + 2;
                this.tournament.poule.teams[indexTeam2].point = this.tournament.poule.teams[indexTeam2].point + 2;
            }

            if(this.checkIfFinishPoule){
                this.sortPoule();
                this.tournament.arbre.match1.team1 = this.tournament.poule.teams[0];
                this.tournament.arbre.match1.team2 = this.tournament.poule.teams[2];
                this.tournament.arbre.match2.team1 = this.tournament.poule.teams[1];
                this.tournament.arbre.match2.team2 = this.tournament.poule.teams[3];
            }
        }else{
            if(this.checkIfFinishSemiFinal){
                this.tournament.arbre.match3.team1 = this.getMatchWinner(this.tournament.arbre.match1);
                this.tournament.arbre.match3.team2 = this.getMatchWinner(this.tournament.arbre.match2);
            }
        }
     }

}
