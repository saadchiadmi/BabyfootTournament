import { Player } from './Player';
import { MatchTeam } from './MatchTeam';
import { MatchPlayer } from './MatchPlayer';

export interface TournamentTeam
{
    pseudos: string[];
    points: number;
}

export function getTeam(mteam: MatchPlayer[], teams: TournamentTeam[]): TournamentTeam
{
    let pseudos: string[] = mteam.map(p => p.pseudo);
    let team = teams.filter(t => t.pseudos.some(pseudo => pseudos.some(p => p === pseudo)))[0];
    return team;
}

export function toMatchTeam(team: TournamentTeam): MatchPlayer[] {
    return 
        [
            { pseudo: team.pseudos[0], goals: 0 },
            { pseudo: team.pseudos[1], goals: 0 }
        ];
}
