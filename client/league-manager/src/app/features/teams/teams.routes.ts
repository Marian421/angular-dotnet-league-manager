import { Routes } from '@angular/router';
import { TeamCreateComponent } from './pages/team-create/team-create';
import { TeamDetailsComponent } from './pages/team-details/team-details';
import { TeamManageComponent } from './pages/team-manage/team-manage';
import { TeamListComponent } from './pages/team-list/team-list';

export const teamsRoutes: Routes = [
  {
    path: '',
    component: TeamListComponent,
  },
  {
    path: 'create',
    component: TeamCreateComponent,
  },
  {
    path: ':id/manage',
    component: TeamManageComponent,
  },
  {
    path: ':id',
    component: TeamDetailsComponent,
  },
];
