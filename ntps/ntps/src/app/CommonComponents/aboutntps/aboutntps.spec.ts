import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Aboutntps } from './aboutntps';

describe('Aboutntps', () => {
  let component: Aboutntps;
  let fixture: ComponentFixture<Aboutntps>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Aboutntps]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Aboutntps);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
