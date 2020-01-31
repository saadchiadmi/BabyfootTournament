import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {ConfirmationService} from 'primeng/api';
import { Tournament } from '../entities/Tournament';
import { TournamentService } from '../service/tournament.service';
import { Team } from '../entities/Team';
import { Match } from '../entities/Match';
import { Arbre } from '../entities/Arbre';
import { Poule } from '../entities/Poule';
import { TeamService } from '../service/team.service';

@Component({
  selector: 'app-one-tournament',
  templateUrl: './one-tournament.component.html',
  styleUrls: ['./one-tournament.component.css'],
  providers: [ConfirmationService]
})
export class OneTournamentComponent implements OnInit {

    id: string;
    tournament: Tournament;
    arbre = {} as Arbre;
    poule = {} as Poule;
    checkFinishPoule : boolean = false;
    checkFinishSemiFinal : boolean = false;
    displayDialog: boolean;
    selectedMatch: Match;
    sortedTeamPoule: Team[];
    sortedMatchPoule: Match[];

    constructor(private confirmationService: ConfirmationService, private activatedRoute: ActivatedRoute, private tournamentService: TournamentService, private teamservice: TeamService, private router: Router) { }

  ngOnInit() {
    this.tournament = window.history.state.tournament;
    if(this.tournament) 
        localStorage.setItem('tournament', JSON.stringify(this.tournament));
    else 
        this.tournament = JSON.parse(localStorage.getItem('tournament'));
    if(!this.tournament){
        this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.tournamentService.getTournamentById(this.id).subscribe(res => this.tournament = res);
        });
    }
    this.poule = this.tournament.poule;
    this.arbre = this.tournament.arbre;
    this.sortPoule();
    this.checkFinishPoule = this.tournament.poule.matchs.filter(m => m.finish==true).length == this.tournament.poule.matchs.length ? true : false;
    this.checkFinishSemiFinal = this.tournament.arbre.match1.finish && this.tournament.arbre.match2.finish ? true : false;
    }

    selectMatch(event: Event, match: Match) {
        this.selectedMatch = match;
        if (match.finish) {
            this.displayDialog = true;
            event.preventDefault();
        }
        else{
            this.confirmationService.confirm({
                message: 'Are you sure that you want to start the match?',
                header: 'Confirmation',
                icon: 'pi pi-exclamation-triangle',
                accept: () => {
                    this.router.navigate(["/match", match.token], { state: {match: match}});
                }
            });
            
        }
    }

    onDialogHide() {
        this.displayDialog = false;
        this.selectedMatch = null;
    }

    /*defineArbre() {
        this.arbre.match1 = this.tournament.matches.filter(m => m.niveau == "demifinal")[0];
        this.arbre.match2 = this.tournament.matches.filter(m => m.niveau == "demifinal")[1];
        this.arbre.match3 = this.tournament.matches.filter(m => m.niveau == "final")[0];
        this.poule.matchs = this.tournament.matches.filter(m => m.niveau == "poule");
    }*/

    sortPoule() {

        this.sortedTeamPoule = this.tournament.poule.teams.sort((n1, n2) => {
            if (n1.point > n2.point) return -1;
            if (n1.point < n2.point) return 1;
            return 0;
        });
        this.sortedMatchPoule = this.poule.matchs.sort((n1,n2) => {
            if (n1.ordre > n2.ordre) return 1;
            if (n1.ordre < n2.ordre) return -1;
            return 0;
        });
    }
    
    

}
