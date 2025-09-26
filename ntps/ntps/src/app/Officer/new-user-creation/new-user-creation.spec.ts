import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewUserCreation } from './new-user-creation';

describe('NewUserCreation', () => {
  let component: NewUserCreation;
  let fixture: ComponentFixture<NewUserCreation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewUserCreation]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewUserCreation);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
