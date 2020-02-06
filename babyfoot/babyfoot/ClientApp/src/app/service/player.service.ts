import { Injectable, Inject } from '@angular/core';
import { Player } from '../entities/Player';
import { Observable, BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    players: BehaviorSubject<Player[]> = new BehaviorSubject([
        { pseudo: "thomas", score: 45, goals: 35, champions: 3 },
        { pseudo: "pierre", score: 50, goals: 47, champions: 4 },
        { pseudo: "saad", score: 25, goals: 35, champions: 2 },
        { pseudo: "antoine", score: 10, goals: 10, champions: 1 },
        { pseudo: "philippe", score: 30, goals: 20, champions: 2 },
        { pseudo: "jean", score: 37, goals: 27, champions: 2 },
        { pseudo: "alexis", score: 30, goals: 20, champions: 2 },
        { pseudo: "jeff", score: 37, goals: 27, champions: 2 },
        { pseudo: "antoinette", score: 30, goals: 20, champions: 2 },
        { pseudo: "valentin", score: 37, goals: 27, champions: 2 },
    ]) ;

    URI: string = "https://localhost:44324/";
    getPlayers(): Observable<Player[]> {
        return this.http.get<Player[]>(this.baseUrl + "api/players");
    }

    savePlayer(player: Player): Observable<Player> {
        return this.http.post<Player>(this.URI + "api/players", player);
    }
}
