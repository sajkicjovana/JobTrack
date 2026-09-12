import { TestBed } from '@angular/core/testing';

import { JobApplications } from './job-applications';

describe('JobApplications', () => {
  let service: JobApplications;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JobApplications);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
