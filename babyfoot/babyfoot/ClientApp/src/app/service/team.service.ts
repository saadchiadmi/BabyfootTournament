import { Injectable, Inject } from '@angular/core';
import { Team } from '../entities/Team';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TeamService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getTeamById(id: number): Observable<Team> {
        return this.http.get<Team>(this.baseUrl + "api/teams/" + id);
    }
}
