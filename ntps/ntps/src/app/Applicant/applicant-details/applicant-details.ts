import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedModule } from '../../Shared/shared.module';
import { Router } from '@angular/router';
import { Circle, Division, Range, State } from '../../models/common-models';
import { MasterRegistration } from '../../Services/master-registration';
import { IdentityProof, SourceType } from '../../models/common-enums';

@Component({
  selector: 'app-applicant-details',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './applicant-details.html',
  styleUrl: './applicant-details.css'
})
export class ApplicantDetails implements OnInit {
  detailsForm: FormGroup;
  fileError: string = '';
  selectedFile: File | null = null;

  states: State[] = [];
  circles: Circle[] = [];
  divisions: Division[] = [];
  ranges: Range[] = [];
  maxCommentLength = 200;
  IdentityProof = IdentityProof; // ✅ bind enum to template
  SourceType = SourceType;
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private registrationService: MasterRegistration
  ) {
    this.detailsForm = this.fb.group({
      title: ['Mr.', Validators.required],
      name: ['', Validators.required],
      mobile: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      email: ['', [Validators.required, Validators.email]],
      identityProof: ['', Validators.required],
      identityNumber: ['', Validators.required],
      state: ['', Validators.required],
      circle: ['', Validators.required],
      division: ['', Validators.required],
      range: ['', Validators.required],
      address: ['', Validators.required],
      pincode: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
    });
  }

  ngOnInit(): void {
   const userData = localStorage.getItem('loggedInUser');
  if (userData) {
    const user = JSON.parse(userData);

    this.detailsForm.patchValue({
      title: user.nameTitle || 'Mr.',
      name: user.name,
      mobile: user.mobileNo,
      email: user.emailId
    });

    this.detailsForm.get('title')?.disable({ emitEvent: false });
    this.detailsForm.get('name')?.disable({ emitEvent: false });
    this.detailsForm.get('mobile')?.disable({ emitEvent: false });
    this.detailsForm.get('email')?.disable({ emitEvent: false });
  }

  this.loadStates();

  // reload circles when state changes
  this.detailsForm.get('state')?.valueChanges.subscribe((stateId: number) => {
    this.loadCircles(stateId);
  });

  // 🔹 Dynamic validation for Identity Proof
  this.detailsForm.get('identityProof')?.valueChanges.subscribe((proof: string) => {
    const identityCtrl = this.detailsForm.get('identityNumber');
    identityCtrl?.clearValidators();

    if (proof === 'Aadhar') {
      identityCtrl?.setValidators([
        Validators.required,
        Validators.pattern('^[0-9]{12}$') // Aadhaar must be exactly 12 digits
      ]);
    } else {
      identityCtrl?.setValidators([
        Validators.required,
        Validators.pattern('^[A-Za-z0-9]{5,20}$') // generic: 5–20 alphanumeric
      ]);
    }

    identityCtrl?.updateValueAndValidity();
  });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      if (file.type !== 'application/pdf') {
        this.fileError = 'Only PDF files are allowed.';
        this.selectedFile = null;
      } else if (file.size > 2 * 1024 * 1024) {
        this.fileError = 'File size must not exceed 2MB.';
        this.selectedFile = null;
      } else {
        this.fileError = '';
        this.selectedFile = file;
      }
    }
  }

  onClear() {
    this.detailsForm.reset();
    this.selectedFile = null;
    this.fileError = '';
  }

  // onSubmit() {
  //   const proof = this.detailsForm.get('identityProof')?.value;

  //   if (this.detailsForm.invalid) {
  //     this.detailsForm.markAllAsTouched();
  //     return;
  //   }

  //   if (proof !== 'Aadhar' && !this.selectedFile) {
  //     this.fileError = 'Please upload valid PDF file.';
  //     return;
  //   }

  //   const formData = new FormData();
  //   formData.append('details', JSON.stringify(this.detailsForm.getRawValue()));

  //   if (proof !== 'Aadhar' && this.selectedFile) {
  //     formData.append('file', this.selectedFile);
  //   }

  //   console.log('Form submitted:', formData);
  //   alert('Applicant details submitted successfully!');
  //   this.router.navigate(['/login']);
  // }

  loadStates() {
    this.registrationService.getAllStates().subscribe({
      next: (data) => (this.states = data),
      error: (err) => console.error('Error fetching states', err)
    });
  }

onSubmit() {
  const proof = this.detailsForm.get('identityProof')?.value;

  // 🔹 Validate form
  if (this.detailsForm.invalid) {
    this.detailsForm.markAllAsTouched();
    return;
  }

  // 🔹 File check if not Aadhaar
  if (proof !== 'Aadhar' && !this.selectedFile) {
    this.fileError = 'Please upload valid PDF file.';
    return;
  }

  // 🔹 Get logged-in user data from localStorage
  const loggedInUser = JSON.parse(localStorage.getItem('loggedInUser') || '{}');

  // 🔹 Merge form values with extra data from logged-in user
  const payload = {
    ...this.detailsForm.getRawValue(),
    loginId: loggedInUser.loginId,
    name: loggedInUser.name,
    email: loggedInUser.emailId,
    sourceType: loggedInUser.sourceType || 1 // fallback to Web=1
  };

  // 🔹 Prepare FormData
  const formData = new FormData();
  formData.append('dto.LoginId', payload.loginId);
  formData.append('dto.IDProof', payload.identityProof);
  formData.append('dto.IDNumber', payload.identityNumber);

  // ✅ Always send selected values
  formData.append('dto.StateId', payload.state);
  formData.append('dto.CircleId', payload.circle);
  formData.append('dto.DivisionId', payload.division);
  formData.append('dto.RangeId', payload.range);
  formData.append('dto.PinCode', payload.pincode);

  // ✅ SubDivisionId can be null → if not selected, send empty string (backend can treat as null)
  formData.append('dto.SubDivisionId', payload.subDivisionId ?? '');

  formData.append('dto.Address', payload.address || '');
  formData.append('dto.Name', payload.name || '');
  formData.append('dto.Email', payload.email || '');
  formData.append('dto.SourceType', payload.sourceType.toString());
  formData.append('dto.CreatedDate', new Date().toISOString());


  

  if (proof !== 'Aadhar' && this.selectedFile) {
    formData.append('file', this.selectedFile, this.selectedFile.name);
  }

  // 🔹 Call API
  this.registrationService.saveApplicantDetails(formData).subscribe({
    next: (res: any) => {
      console.log('API Response:', res);
      alert('Applicant details submitted successfully!');
      this.router.navigate(['/login']);
    },
    error: (err) => {
      console.error('Error saving applicant details:', err);
      alert('Failed to submit applicant details. Please try again.');
    }
  });
}

  loadCircles(stateId: number) {
    if (!stateId) {
      this.circles = [];
      return;
    }
    this.registrationService.getCirclesByState(stateId).subscribe({
      next: (data) => (this.circles = data),
      error: (err) => console.error('Error fetching circles', err)
    });
  }

  onCircleChange(circleId: number) {
    if (circleId) {
      this.registrationService.getDivisionsByCircle(circleId).subscribe({
        next: (data) => (this.divisions = data),
        error: (err) => console.error('Error fetching divisions', err)
      });
    } else {
      this.divisions = [];
    }
  }

  onDivisionChange(divisionId: number) {
    if (divisionId) {
      this.registrationService.getRangesByDivision(divisionId).subscribe({
        next: (data) => (this.ranges = data),
        error: (err) => console.error('Error fetching ranges', err)
      });
    } else {
      this.ranges = [];
    }
  }

 fetchAadhaarDetails() {
  const proofType = this.detailsForm.get('identityProof')?.value;
  const aadhaarNumber = this.detailsForm.get('identityNumber')?.value;

  // 🔹 Only call Aadhaar service if Aadhaar is selected
  if (proofType !== 'Aadhar') {
    return;
  }

  // Basic check for 12-digit Aadhaar
  if (!aadhaarNumber || aadhaarNumber.length !== 12) {
    console.warn('Invalid Aadhaar number');
    return;
  }

  this.registrationService.getAadhaarDetails(aadhaarNumber).subscribe((data: any) => {
    const loggedInUser = JSON.parse(localStorage.getItem('loggedInUser') || '{}');
    const registeredName = loggedInUser?.name;
    const aadhaarName = data?.name;

    if (aadhaarName && registeredName && aadhaarName.toLowerCase() === registeredName.toLowerCase()) {
      // ✅ Names match → autofill
      this.detailsForm.patchValue({ name: aadhaarName });
      this.detailsForm.get('name')?.disable({ emitEvent: false });
      alert('Aadhaar verified and name matched!');
    } else {
      // ❌ Name mismatch → make editable
      alert('Your name on adhaar does not match. Enter your correct name please!');
      this.detailsForm.get('name')?.enable({ emitEvent: false });
    }
  });
}

}
