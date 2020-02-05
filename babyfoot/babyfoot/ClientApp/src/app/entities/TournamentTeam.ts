import { Player } from './Player';
import { MatchTeam } from './MatchTeam';

export interface TournamentTeam
{
    pseudos: string[];
    points: number;
}

export function getTeam(mteam: MatchTeam, teams: TournamentTeam[]): TournamentTeam
{
    let pseudos: string[] = mteam.players.map(p => p.pseudo);
    let team = teams.filter(t => t.pseudos.some(pseudo => pseudos.some(p => p === pseudo)))[0];
    return team;
}

export function toMatchTeam(team: TournamentTeam) {
    return {
        players: [
            { pseudo: team.pseudos[0], goals: 0 },
            { pseudo: team.pseudos[1], goals: 0 }],
        points: 0
    };
}
