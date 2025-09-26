import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { InstructionsPopup } from '../../CommonComponents/instructions-popup/instructions-popup';
import { SharedModule } from '../../Shared/shared.module';

@Component({
  selector: 'app-regenerate-code',
  imports: [SharedModule],
  templateUrl: './regenerate-code.html',
  styleUrl: './regenerate-code.css'
})
export class RegenerateCode {
regenForm!: FormGroup;
  captcha: string = '';

  constructor(private fb: FormBuilder, private router: Router, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.regenForm = this.fb.group({
      loginId: ['', Validators.required],
      captchaInput: ['', Validators.required]
    });
    this.refreshCaptcha();
  }

  refreshCaptcha() {
    const chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
    this.captcha = Array.from({ length: 6 }, () => chars[Math.floor(Math.random() * chars.length)]).join('');
  }

  onSubmit() {
    if (this.regenForm.valid) {
      console.log(this.regenForm.value);
      // handle submission logic
    } else {
      this.regenForm.markAllAsTouched();
    }
  }

  goBack() {
    this.router.navigate(['/login']);
  }
}
