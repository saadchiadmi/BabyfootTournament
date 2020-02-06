import { MatchPlayer } from './MatchPlayer';

export interface MatchTeam
{
    points: number;
    players: MatchPlayer[];
}


export function getScore(team: MatchPlayer[])
{
    return team[0].goals + team[1].goals;
}

export function newMatchTeam(team: MatchPlayer): MatchPlayer[]
{
    return [{ pseudo: team[0].pseudo, goals: 0 }, { pseudo: team[1].pseudo, goals: 0 }];
}