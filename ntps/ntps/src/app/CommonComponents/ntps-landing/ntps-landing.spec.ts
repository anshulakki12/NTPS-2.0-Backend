import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NtpsLanding } from './ntps-landing';

describe('NtpsLanding', () => {
  let component: NtpsLanding;
  let fixture: ComponentFixture<NtpsLanding>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NtpsLanding]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NtpsLanding);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
