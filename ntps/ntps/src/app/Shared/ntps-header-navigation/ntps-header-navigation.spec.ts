import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NtpsHeaderNavigation } from './ntps-header-navigation';

describe('NtpsHeaderNavigation', () => {
  let component: NtpsHeaderNavigation;
  let fixture: ComponentFixture<NtpsHeaderNavigation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NtpsHeaderNavigation]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NtpsHeaderNavigation);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
