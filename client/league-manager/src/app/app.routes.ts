import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { MainLayoutComponent } from './mainLayout/mainLayout.component';
import { LandingComponent } from './mainLayout/landingPage/landingPage.component';
import { RegisterComponent } from './register/register.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: LandingComponent }
    ],
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  }
];
