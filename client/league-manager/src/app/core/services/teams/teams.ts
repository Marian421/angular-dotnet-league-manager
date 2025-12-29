import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Team } from '../../../shared/models/team.model';

@Injectable({
  providedIn: 'root',
})
export class TeamsService {
  private apiUrl = "url";

  constructor(private http: HttpClient) { }

  getTeams() {
    return this.http.get<Team[]>(this.apiUrl);
  }

  getTeamById(id: string): Observable<Team> {
    return this.http.get<Team>(`${this.apiUrl}/${id}`);
  }
}
