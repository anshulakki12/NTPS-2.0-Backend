import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { SharedModule } from '../../Shared/shared.module';
import { MatDialog } from '@angular/material/dialog';
import { MasterRegistration } from '../../Services/master-registration';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-applicant-registration',
  standalone: true,
  imports: [SharedModule,MatProgressBarModule],
  templateUrl: './applicant-registration.html',
  styleUrls: ['./applicant-registration.css']
})
export class ApplicationRegistration {
  registrationForm: FormGroup;
  captcha: string = this.generateCaptcha();
  showPassword: boolean = false;
  loadingAadhaar = false;
  otpSent = false;
  canResendOtp = false;
  otpValue = '';
  countdown = 60;
  timer: any;
  mobileAllowed: boolean = false;
  mobileStatusMessage: string = '';
  // password toggle flags
  showConfirmPassword = false;
  constructor(private fb: FormBuilder
    , private dialog: MatDialog
    ,private registrationService: MasterRegistration
    ,private router: Router) {
  this.registrationForm = this.fb.group({
  registrationType: ['', Validators.required],
  title: ['Mr.', Validators.required],
  name: ['', Validators.required],
  email: ['', [Validators.required, Validators.email]],
  mobile: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
  loginId: ['', [Validators.required, Validators.minLength(6)]], // ✅ New field
  captchaInput: ['', Validators.required],
  password: [
      '',
      [
        Validators.required,
        Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)
      ]
    ],
    confirmPassword: ['', Validators.required]
  },
  { validators: this.passwordMatchValidator } // attach here at group level
); // ✅ custom validator

  }

 ngOnInit(): void {
  
  this.registrationForm.get('mobile')?.valueChanges.subscribe(mobile => {
    if (mobile && /^[0-9]{10}$/.test(mobile)) {
      this.checkMobileBeforeOtp(mobile);
    }
  });
  const storedOtp = sessionStorage.getItem('testOtp');
  if (storedOtp) {
    this.otpValue = storedOtp; // auto-fill the OTP
  }
  this.registrationForm.get('confirmPassword')?.valueChanges.subscribe(() => {
    this.registrationForm.updateValueAndValidity({ onlySelf: false, emitEvent: true });
  });
}

// checkMobileBeforeOtp(mobile: string) {
//   this.registrationService.checkMobile(mobile).subscribe({
//     next: (res: any) => {
//       console.log(res.message);
//       this.mobileStatusMessage = res.message;
//       this.mobileAllowed = true;   // ✅ Allow OTP later
//     },
//    error: (err) => {
//   if (err.status === 409) {
//     const code = err.error?.code;

//     switch (code) {
//       case "MOBILE_EXISTS_VERIFIED":
//         this.mobileStatusMessage = "Mobile number entered is already registered.";
//         break;

//       case "MOBILE_EXISTS_NOT_VERIFIED":
//         this.mobileStatusMessage = "Registration is Incomplete for this Mobile number.";
//         break;

//       default:
//         this.mobileStatusMessage = "An unexpected error occurred.";
//     }
//   }
// }

//   });
// }

checkMobileBeforeOtp(mobile: string) {
  this.registrationService.checkMobile(mobile).subscribe({
    next: (res: any) => {
      this.mobileStatusMessage = '';
      this.mobileAllowed = true;
      this.registrationForm.get('mobile')?.setErrors(null); // ✅ clear errors
    },
    error: (err) => {
      this.mobileAllowed = false; // ❌ disable Send OTP
      let msg = "Invalid or blocked mobile number.";

      if (err.status === 409) {
        const code = err.error?.code;
        switch (code) {
          case "MOBILE_EXISTS_VERIFIED":
            msg = "Mobile number entered is already registered.";
            break;
          case "MOBILE_EXISTS_NOT_VERIFIED":
            msg = "Registration is incomplete for this mobile number.";
            break;
        }
      }
      this.mobileStatusMessage = msg;
      // ✅ tell Angular form control it's invalid
      this.registrationForm.get('mobile')?.setErrors({ serverError: true });
    }
  });
}

  generateCaptcha(): string {
    const chars = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    return Array.from({ length: 6 }, () => chars[Math.floor(Math.random() * chars.length)]).join('');
  }

  refreshCaptcha(): void {
    this.captcha = this.generateCaptcha();
  }

// onSubmit(): void {
//   if (this.registrationForm.invalid) return;

//   if (this.registrationForm.value.captchaInput !== this.captcha) {
//     alert('Captcha does not match!');
//     this.refreshCaptcha();
//     return;
//   }

//   const formData = {
//     nameTitle: this.registrationForm.value.title,    // ✅ match C# NameTitle
//     name: this.registrationForm.value.name,          // ✅ same
//     emailId: this.registrationForm.value.email,      // ✅ match C# EmailId
//     loginId: this.registrationForm.value.email,      // or another unique username
//     password: this.registrationForm.value.password,  // ✅ same
//     mobileNo: this.registrationForm.value.mobile,    // ✅ match C# MobileNo
//     isVerified: 'N',                                 // default value
//     createdDate: new Date().toISOString()            // send valid datetime
//   };

//   this.registrationService.register(formData).subscribe({
//     next: (res) => {
//       console.log('Registration Successful!', res);
//       alert('Registration Successful!');
//       this.onClear();
//     },
//     error: (err) => {
//       console.error('Registration Failed', err);
//       alert('Registration Failed! Please try again.');
//     }
//   });
// }


  onClear(): void {
    this.registrationForm.reset({ title: 'Mr.' });
    this.refreshCaptcha();
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
//   onAadhaarUpload(event: any) {
//   const file = event.target.files[0];
//   if (!file) return;

//   const formData = new FormData();
//   formData.append('file', file);

//   this.loadingAadhaar = true;
//   this.registrationService.extractAadhaarData(formData).subscribe({
//     next: (res) => {
//       this.loadingAadhaar = false;
//       if (res?.name) {
//         this.registrationForm.patchValue({ name: res.name });
//       } else {
//         alert('Could not extract Aadhaar details. Please enter manually.');
//       }
//     },
//     error: (err) => {
//       this.loadingAadhaar = false;
//       console.error(err);
//       alert('Aadhaar OCR failed.');
//     }
//   });
// }

onAadhaarUpload(event: any) {
  const file = event.target.files[0];
  if (!file) return;

  const formData = new FormData();
  formData.append('file', file);

  this.loadingAadhaar = true;
  this.registrationService.extractAadhaarData(formData).subscribe({
    next: (res) => {
      this.loadingAadhaar = false;
      if (res?.name) {
        this.registrationForm.patchValue({ name: res.name });
      } else {
        alert('Could not extract Aadhaar details. Please enter manually.');
      }
    },
    error: (err) => {
      this.loadingAadhaar = false;
      console.error(err);
      alert('Aadhaar OCR failed.');
    }
  });
}


verifyCaptcha(): void {
  if (this.registrationForm.value.captchaInput === this.captcha) {
    this.registrationForm.get('password')?.enable();
    this.registrationForm.get('confirmPassword')?.enable();
    alert('Captcha verified! You can now set your password.');
  } else {
    alert('Captcha does not match! Try again.');
    this.refreshCaptcha();
  }
}

onSubmit(): void {
  if (this.registrationForm.invalid) {
    // mark all controls as touched so errors show up
    Object.keys(this.registrationForm.controls).forEach(field => {
      const control = this.registrationForm.get(field);
      control?.markAsTouched({ onlySelf: true });
    });
    return; // stop submission if invalid
  }

  const rawForm = this.registrationForm.getRawValue(); // ✅ includes disabled fields

const formData = {
  registrationType: rawForm.registrationType,
  nameTitle: rawForm.title,
  name: rawForm.name,
  emailId: rawForm.email,
  loginId: rawForm.loginId,   // ✅ User-defined Login ID
  password: rawForm.password,
  mobileNo: rawForm.mobile,
  isVerified: 'N',
  createdDate: new Date().toISOString()
};



  this.registrationService.register(formData).subscribe({
    next: (res) => {
      alert('Registration Successful!');
      this.onClear();
      this.router.navigate(['/login']); // Navigate to login after successful registration
    },
    error: () => alert('Registration Failed! Please try again.')
  });
}


// sendOtp() {
//   const mobile = this.registrationForm.value.mobile;
//   if (!/^[0-9]{10}$/.test(mobile)) {
//     alert('Please enter a valid 10-digit mobile number.');
//     return;
//   }

//   this.registrationService.generateOtp(mobile).subscribe({
//     next: () => {
//       this.otpSent = true;
//       this.startCountdown();
//       alert('OTP sent successfully!');
//     },
//     error: () => alert('Failed to send OTP. Try again.')
//   });
// }

sendOtp() {
  const mobile = this.registrationForm.value.mobile;
  if (!/^[0-9]{10}$/.test(mobile)) {
    alert('Please enter a valid 10-digit mobile number.');
    return;
  }

  // Generate a mock OTP for local testing
  const generatedOtp = Math.floor(100000 + Math.random() * 900000).toString();
  
  // Store it in sessionStorage so you can retrieve later
  sessionStorage.setItem('testOtp', generatedOtp);
  console.log('Mock OTP for local use:', generatedOtp);

  // Set in component so input can be auto-filled
  this.otpValue = generatedOtp;

  // Skip backend and just simulate success
  this.otpSent = true;
  this.startCountdown();
  alert(`OTP sent successfully! (For testing: ${generatedOtp})`);
  this.registrationService.generateOtp(mobile).subscribe({
    next: () => {
      this.otpSent = true;
      this.startCountdown();
      alert('OTP sent successfully!');
    },
    error: () => alert('Failed to send OTP. Try again.')
  });
}




// verifyOtp() {
//   const mobile = this.registrationForm.value.mobile;
//   this.registrationService.verifyOtp({ mobileNumber: mobile, otp: this.otpValue }).subscribe({
//     next: () => {
//       alert('OTP verified successfully!');
//     },
//     error: () => alert('Invalid or expired OTP.')
//   });
// }

// verifyOtp() {
//   const storedOtp = sessionStorage.getItem('testOtp');
//   if (this.otpValue === storedOtp) {
//     alert('OTP verified successfully!');
//   } else {
//     alert('Invalid OTP!');
//   }
// }

verifyOtp() {
  const mobile = this.registrationForm.value.mobile;
  const otp = this.otpValue;

  if (!mobile || !otp) {
    alert('Please enter both Mobile Number and OTP.');
    return;
  }

  this.registrationService.verifyOtp({ mobileNumber: mobile, otp: otp }).subscribe({
    next: (res: any) => {
      alert(res.message);
      // Optionally disable OTP input after successful verification
      this.registrationForm.get('mobile')?.disable();
    },
    error: (err) => {
      alert(err.error?.message || 'OTP verification failed.');
    }
  });
}


startCountdown() {
  this.countdown = 60;
  this.canResendOtp = false;
  clearInterval(this.timer);
  this.timer = setInterval(() => {
    this.countdown--;
    if (this.countdown <= 0) {
      clearInterval(this.timer);
      this.canResendOtp = true;
    }
  }, 1000);
}
// Toggle methods

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  // Custom validator: check if password === confirmPassword
passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');

  if (!password || !confirmPassword) {
    return null; // nothing to validate yet
  }

  return password.value === confirmPassword.value ? null : { mismatch: true };
}


}
