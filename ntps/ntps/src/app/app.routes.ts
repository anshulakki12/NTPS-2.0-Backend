// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { ApplicationRegistration } from './Applicant/applicant-registration/applicant-registration';
import { ApplicantLogin } from './Applicant/applicant-login/applicant-login';
import { RegenerateCode } from './Applicant/regenerate-code/regenerate-code';
import { NewUserCreation } from './Officer/new-user-creation/new-user-creation';
import { ForgotPassword } from './CommonComponents/forgot-password/forgot-password';
import { Aboutntps } from './CommonComponents/aboutntps/aboutntps';
import { ContactUs } from './CommonComponents/contact-us/contact-us';
import { ApplicantDetails } from './Applicant/applicant-details/applicant-details';
import { NtpsLanding } from './CommonComponents/ntps-landing/ntps-landing';

export const routes: Routes = [
  { path: 'register', component: ApplicationRegistration },
  { path: 'login', component: ApplicantLogin },
  { path: 'ApplicantDetails', component: ApplicantDetails },
  { path: 'regeneratecode', component: RegenerateCode },
  { path: 'newusercreation', component: NewUserCreation },
  { path: 'ForgotPassword', component: ForgotPassword },
  { path: 'AboutNTPS', component: Aboutntps },
  { path: 'ContactUs', component: ContactUs },
  { path: 'NTPSLanding', component: NtpsLanding },
  { path: '', redirectTo: 'NTPSLanding', pathMatch: 'full' }
];
