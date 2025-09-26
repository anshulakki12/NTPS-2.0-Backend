import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicantLogin } from './applicant-login';

describe('ApplicantLogin', () => {
  let component: ApplicantLogin;
  let fixture: ComponentFixture<ApplicantLogin>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicantLogin]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplicantLogin);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
