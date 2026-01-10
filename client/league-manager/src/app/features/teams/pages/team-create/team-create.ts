import { Component } from '@angular/core';
import { TeamCreate } from '../../../../shared/models/team-create.model';
import { TeamsService } from '../../../../core/services/teams/teams';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-team-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './team-create.html',
})
export class TeamCreateComponent {
  teamForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private teamService: TeamsService,
  ) {
    this.teamForm = this.fb.group({
      name: ['', Validators.required],
      isCompetitive: [false],
      minAge: [6, [Validators.required, Validators.min(6)]],
      ownerId: [1],
      memberIds: [[]],
    });
  }

  onSubmit() {
    if (this.teamForm.invalid) return;

    const team: TeamCreate = this.teamForm.value;
    this.teamService.createTeam(team).subscribe({
      next: (createdTeam) => {
        console.log('Team created:', createdTeam);
        this.teamForm.reset({
          name: '',
          isCompetitive: false,
          minAge: 0,
          ownerId: 1,
          memberIds: [],
        });
      },
      error: (err) => console.error('Error creating team:', err),
    });
  }
}
