import { Injectable, Inject } from '@angular/core';
import { Player } from '../entities/Player';
import { Observable, BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  players : BehaviorSubject<Player[]> = new BehaviorSubject([
    {pseudo : "saad", score : 45, goals : 35, champions : 3},
    {pseudo : "saad1", score : 50, goals : 47, champions : 4},
    {pseudo : "thomas", score : 25, goals : 35, champions : 2},
    {pseudo : "thomas1", score : 10, goals : 10, champions : 1},
    {pseudo : "stbe", score : 30, goals : 20, champions : 2},
    {pseudo : "stbe1", score : 37, goals : 27, champions : 2},
    {pseudo : "str", score : 38, goals : 27, champions : 2},
    {pseudo : "str1", score : 27, goals : 27, champions : 2},
    {pseudo : "pistl", score : 29, goals : 27, champions : 2},
    {pseudo : "pislt", score : 39, goals : 27, champions : 2},
  ]) ;

    getPlayers(): Observable<Player[]> {
        //return this.http.get<Player[]>(this.baseUrl + "api/players");
        return this.players
    }
}
