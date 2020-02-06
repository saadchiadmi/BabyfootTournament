import { MatchTeam, getScore } from './MatchTeam';
import { MatchPlayer } from './MatchPlayer';

export interface Match
{
    token: string;
    order: number;
    stage: string;
    state: string;
    start : Date;
    elapsed: number;
    teams: MatchPlayer[][];
}

export function getMatch(matches: Match[], stage: string, order: number): Match {
    return matches.filter(m => m.stage === stage && m.order == order)[0];
}

export function getFinalMatch(matches: Match[]): Match {
    return matches.filter(m => m.stage === "Final")[0];
}

export function getSemifinalMatch(matches: Match[], order: number): Match {
    return getMatch(matches, "Semifinal", order);
}

export function getPoolMatch(matches: Match[], order: number): Match {
    return getMatch(matches, "Pool", order);
}

export function getWinnerTeam(match: Match): MatchPlayer[]
{
    let score1 = getScore(match.teams[0]);
    let score2 = getScore(match.teams[1]);
    if (score1 > score2)
        return match.teams[0];
    else if (score1 < score2)
        return match.teams[1];
    else
        return null;
}
