import { Injectable, Inject } from '@angular/core';
import { TournamentTeam } from '../entities/TournamentTeam';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TeamService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getTeamById(id: number): Observable<TournamentTeam> {
        return this.http.get<TournamentTeam>(this.baseUrl + "api/teams/" + id);
    }
}
