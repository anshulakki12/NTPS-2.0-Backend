import { TestBed } from '@angular/core/testing';

import { MasterRegistration } from './master-registration';

describe('MasterRegistration', () => {
  let service: MasterRegistration;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MasterRegistration);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
