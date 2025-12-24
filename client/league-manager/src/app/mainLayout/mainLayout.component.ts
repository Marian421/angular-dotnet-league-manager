import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-main-layout',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './mainLayout.component.html',
})
export class MainLayoutComponent { }
