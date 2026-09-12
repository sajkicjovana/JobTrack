import { TestBed } from '@angular/core/testing';

import { Interviews } from './interviews';

describe('Interviews', () => {
  let service: Interviews;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Interviews);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
