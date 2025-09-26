// src/app/app.ts
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NtpsHeaderNavigation } from './Shared/ntps-header-navigation/ntps-header-navigation';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,NtpsHeaderNavigation],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  protected readonly title = signal('ntps');
}
