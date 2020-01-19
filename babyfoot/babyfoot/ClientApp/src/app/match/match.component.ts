import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatchService } from '../service/match.service';
import { Match } from '../entities/Match';
import { Player } from '../entities/Player';

@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.css']
})
export class MatchComponent implements OnInit {

    id: string;
    selectedMatch: Match;

    constructor(private activatedRoute: ActivatedRoute, private matchService: MatchService) { }

    ngOnInit() {
        this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.matchService.getMatchById(this.id).subscribe(res => {
                this.selectedMatch = res;
            });
        });
    }

    addGoal(s: string) {
          this.selectedMatch[s] = this.selectedMatch[s] + 1; 
    }

    dropGoal(s: string) {
        if (this.selectedMatch[s] != 0)
          this.selectedMatch[s] = this.selectedMatch[s] - 1; 
    }

}
