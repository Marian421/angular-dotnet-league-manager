import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: 'login.component.html',
})
export class LoginComponent {
  route = inject(ActivatedRoute);

  loginID = signal('');
  limit = signal('');

  constructor() {
    this.loginID.set(this.route.snapshot.paramMap.get('loginID') ?? '');
    this.limit.set(this.route.snapshot.queryParamMap.get('limit') ?? '');
  }
}
