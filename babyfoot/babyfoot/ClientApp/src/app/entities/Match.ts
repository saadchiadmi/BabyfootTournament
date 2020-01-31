import { Team } from './Team';

export interface Match {

    token: string;
    //token: string;
    //start : Date;
    team1: Team;
    team2: Team;
    finish: boolean;
    //winner: Team;
    //niveau: string;
    ordre : number;
    scoreTeam1Player1: number;
    scoreTeam1Player2: number;
    scoreTeam2Player1: number;
    scoreTeam2Player2: number;
    
}
