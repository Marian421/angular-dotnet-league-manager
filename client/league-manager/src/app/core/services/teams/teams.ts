import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Team } from '../../../shared/models/team.model';
import { environment } from '../../../../environments/environment';
import { TeamCreate } from '../../../shared/models/team-create.model';

@Injectable({
  providedIn: 'root',
})
export class TeamsService {
  private readonly apiUrl = environment.apiBaseUrl;
  private readonly teamsUrl = `${this.apiUrl}/teams`;

  constructor(private http: HttpClient) {}

  getTeams() {
    return this.http.get<Team[]>(this.teamsUrl);
  }

  getTeamById(id: string): Observable<Team> {
    return this.http.get<Team>(`${this.teamsUrl}/${id}`);
  }

  createTeam(team: TeamCreate) {
    return this.http.post<Team>(this.teamsUrl, team);
  }
}
