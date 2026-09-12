import { TestBed } from '@angular/core/testing';

import { TestAttempts } from './test-attempts';

describe('TestAttempts', () => {
  let service: TestAttempts;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TestAttempts);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
