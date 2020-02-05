import { MatchPlayer } from './MatchPlayer';

export interface MatchTeam
{
    points: number;
    players: MatchPlayer[];
}


export function getScore(team: MatchTeam)
{
    return team.players[0].goals + team.players[1].goals;
}

export function newMatchTeam(team: MatchTeam)
{
    return { points: 0, players: [{ pseudo: team.players[0].pseudo, goals: 0 }, { pseudo: team.players[1].pseudo, goals: 0 }] };
}