import { Match } from './Match';
import { TournamentService } from '../service/tournament.service';
import { TournamentTeam } from './TournamentTeam';

export interface Tournament
{

    token: string;
    date: Date;
    state: string;
    teams: TournamentTeam[];
    matches: Match[];
}
