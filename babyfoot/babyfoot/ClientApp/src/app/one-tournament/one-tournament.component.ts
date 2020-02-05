import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {ConfirmationService} from 'primeng/api';
import { Tournament } from '../entities/Tournament';
import { TournamentService } from '../service/tournament.service';
import { TournamentTeam } from '../entities/TournamentTeam';
import { Match, getFinalMatch, getSemifinalMatch, getPoolMatch } from '../entities/Match';
import { TeamService } from '../service/team.service';

@Component({
  selector: 'app-one-tournament',
  templateUrl: './one-tournament.component.html',
  styleUrls: ['./one-tournament.component.css'],
  providers: [ConfirmationService]
})
export class OneTournamentComponent implements OnInit
{

    id: string;
    tournament: Tournament;
    tree: Match[];
    //poule = {} as Pool;
    checkFinishPoule : boolean = false;
    checkFinishSemiFinal : boolean = false;
    displayDialog: boolean;
    selectedMatch: Match;
    sortedPoolTeams: TournamentTeam[];
    sortedPoolMatches: Match[];

    constructor(private confirmationService: ConfirmationService, private activatedRoute: ActivatedRoute, private tournamentService: TournamentService, private teamservice: TeamService, private router: Router) { }

    ngOnInit()
    {
        this.tournament = window.history.state.tournament;
        if(this.tournament) 
            localStorage.setItem('tournament', JSON.stringify(this.tournament));
        else 
            this.tournament = JSON.parse(localStorage.getItem('tournament'));
            if (!this.tournament)
            {
            this.activatedRoute.params.subscribe(params =>
            {
                this.id = params['id'];
                this.tournamentService.getTournamentById(this.id).subscribe(res => this.tournament = res);
            });
        }
        this.sortPoule();
        this.tree = [
            getFinalMatch(this.tournament.matches),
            getSemifinalMatch(this.tournament.matches, 1),
            getSemifinalMatch(this.tournament.matches, 2)];
        this.checkFinishPoule = this.tournament.matches.filter(m => m.state === "Ended"==true).length == this.tournament.matches.length ? true : false;
        this.checkFinishSemiFinal = this.tournament.matches.filter(m => m.stage === "Semifinal" && m.order == 1 && m.state === "Ended") && this.tournament.matches.filter(m => m.stage === "Semifinal" && m.order == 2 && m.state === "Ended") ? true : false;
    }

    selectMatch(event: Event, match: Match) {
        this.selectedMatch = match;
        if (match.state === "Ended") {
            this.displayDialog = true;
            event.preventDefault();
        }
        else{
            this.confirmationService.confirm({
                message: 'Are you sure you want to start the match ?',
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
        this.arbre.semifinal1 = this.tournament.matches.filter(m => m.niveau == "demifinal")[0];
        this.arbre.semifinal2 = this.tournament.matches.filter(m => m.niveau == "demifinal")[1];
        this.final = this.tournament.matches.filter(m => m.niveau == "final")[0];
        this.matchs = this.tournament.matches.filter(m => m.niveau == "poule");
    }*/

    sortPoule() {

        this.sortedPoolTeams = this.tournament.teams.sort((n1, n2) => {
            if (n1.points > n2.points) return -1;
            if (n1.points < n2.points) return 1;
            return 0;
        });
        this.sortedPoolMatches = this.tournament.matches.sort((n1, n2) =>
        {
            if (n1.order > n2.order) return 1;
            if (n1.order < n2.order) return -1;
            return 0;
        });
    }
    
    

}
