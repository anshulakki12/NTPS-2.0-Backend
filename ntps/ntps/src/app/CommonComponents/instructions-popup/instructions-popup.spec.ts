import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstructionsPopup } from './instructions-popup';

describe('InstructionsPopup', () => {
  let component: InstructionsPopup;
  let fixture: ComponentFixture<InstructionsPopup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstructionsPopup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstructionsPopup);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
