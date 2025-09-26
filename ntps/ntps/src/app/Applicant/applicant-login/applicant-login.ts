import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedModule } from '../../Shared/shared.module';
import { MatDialog } from '@angular/material/dialog';
import { InstructionsPopup } from '../../CommonComponents/instructions-popup/instructions-popup';
import { MasterRegistration } from '../../Services/master-registration';
import { Router } from '@angular/router';
@Component({
  selector: 'app-applicant-login',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './applicant-login.html',
  styleUrls: ['./applicant-login.css']
})
export class ApplicantLogin {
loginForm: FormGroup;
  hidePassword: boolean = true;
  captcha: string = this.generateCaptcha();

  constructor(private fb: FormBuilder, 
    private dialog: MatDialog,
    private registrationService: MasterRegistration,
    private router: Router) {
    this.loginForm = this.fb.group({
      loginId: ['', Validators.required],
      password: ['', Validators.required],
      captchaInput: ['', Validators.required]
    });
  }

  ngOnInit(): void {
}

   generateCaptcha(): string {
    const chars = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    return Array.from({ length: 6 }, () => chars[Math.floor(Math.random() * chars.length)]).join('');
  }

  refreshCaptcha(): void {
    this.captcha = this.generateCaptcha();
  }

  // onLogin(): void {
  //   if (this.loginForm.invalid) return;

  //   if (this.loginForm.value.captchaInput !== this.captcha) {
  //     alert('Captcha does not match!');
  //     this.refreshCaptcha();
  //     return;
  //   }

  //   console.log('Login Success', this.loginForm.value);
  //   alert('Login Successful!');
  // }

  // onLogin(): void {
  //   if (this.loginForm.invalid) return;

  //   if (this.loginForm.value.captchaInput !== this.captcha) {
  //     alert('Captcha does not match!');
  //     this.refreshCaptcha();
  //     return;
  //   }

  //   const { loginId, password } = this.loginForm.value;

  //   this.registrationService.login(loginId, password).subscribe({
  //     next: (res) => {
  //       alert(res.message || 'Login Successful!');
  //       this.router.navigate(['/ApplicantDetails']); // Navigate to login after successful registration
  //       console.log('Login Response:', res);
  //     },
  //     error: (err) => {
  //       alert(err.error?.message || 'Login failed');
  //     }
  //   });
  // }

  onLogin(): void {
  if (this.loginForm.invalid) return;

  if (this.loginForm.value.captchaInput !== this.captcha) {
    alert('Captcha does not match!');
    this.refreshCaptcha();
    return;
  }

  const { loginId, password } = this.loginForm.value;

  this.registrationService.login(loginId, password).subscribe({
    next: (res) => {
      alert(res.message || 'Login Successful!');

      // ✅ Save user info in localStorage
      localStorage.setItem('loggedInUser', JSON.stringify(res.user));

      this.router.navigate(['/ApplicantDetails']);
    },
    error: (err) => {
      alert(err.error?.message || 'Login failed');
    }
  });
}


  onBack(): void {
    // You can use router.navigate here if routing is implemented
    alert('Back to previous page');
  }
}
