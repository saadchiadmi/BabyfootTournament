import { Poule } from './Poule';
import { Arbre } from './Arbre';
import { Match } from './Match';
import { TournamentService } from '../service/tournament.service';
import { Team } from './Team';

export interface Tournament {

    token: string;
    //date: Date;
    finish: boolean;
    poule: Poule;
    arbre: Arbre;
    //matches: Match[];
    //teamsid: number[];
   // teams: Team[];
    //winner: Team;
}
