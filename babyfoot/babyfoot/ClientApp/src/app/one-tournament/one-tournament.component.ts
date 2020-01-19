import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {TreeNode} from 'primeng/api';
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
  styleUrls: ['./one-tournament.component.css']
})
export class OneTournamentComponent implements OnInit {

    id: string;
    tournament: Tournament;
    arbre = {} as Arbre;
    poule = {} as Poule;
    displayDialog: boolean;
    selectedMatch: Match;
    sortedTeamPoule: Team[];
    sortedMatchPoule: Match[];

    constructor(private activatedRoute: ActivatedRoute, private tournamentService: TournamentService, private teamservice: TeamService) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(params => {
        this.id = params['id'];
        this.tournamentService.getTournament().subscribe(res => {
            this.tournament = res[0];
            //this.tournament.teams = [];
            //this.tournament.teamsid.forEach(id => this.teamservice.getTeamById(id).subscribe(team => this.tournament.teams.push(team)));
            //this.defineArbre();
            this.poule = this.tournament.poule;
            this.arbre = this.tournament.arbre;
            this.sortPoule();
        });
    });
    }

    selectMatch(event: Event, match: Match) {
        this.selectedMatch = match;
        this.displayDialog = true;
        event.preventDefault();
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
