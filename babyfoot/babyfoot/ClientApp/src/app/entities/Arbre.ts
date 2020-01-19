import { Team } from './Team';
import { Match } from './Match';

export interface Arbre {

    match1 : Match; //demi-final 1
    match2 : Match; //demi-final 2
    match3 : Match; //final
    team : Team; //gagnant
    
}