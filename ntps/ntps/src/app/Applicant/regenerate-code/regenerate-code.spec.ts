import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegenerateCode } from './regenerate-code';

describe('RegenerateCode', () => {
  let component: RegenerateCode;
  let fixture: ComponentFixture<RegenerateCode>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegenerateCode]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegenerateCode);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
