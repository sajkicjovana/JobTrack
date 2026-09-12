import { TestBed } from '@angular/core/testing';

import { Technologies } from './technologies';

describe('Technologies', () => {
  let service: Technologies;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Technologies);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
