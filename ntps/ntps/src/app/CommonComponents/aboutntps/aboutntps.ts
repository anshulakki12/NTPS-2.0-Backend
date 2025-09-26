import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-aboutntps',
  imports: [],
  templateUrl: './aboutntps.html',
  styleUrl: './aboutntps.css'
})
export class Aboutntps {
constructor(private router: Router) {}

  goBack(): void {
    this.router.navigate(['/login']); // Change as needed
  }
}
