import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationRegistration } from './applicant-registration';

describe('ApplicationRegistration', () => {
  let component: ApplicationRegistration;
  let fixture: ComponentFixture<ApplicationRegistration>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationRegistration]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplicationRegistration);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
