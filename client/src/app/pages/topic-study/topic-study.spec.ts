import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopicStudy } from './topic-study';

describe('TopicStudy', () => {
  let component: TopicStudy;
  let fixture: ComponentFixture<TopicStudy>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TopicStudy]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TopicStudy);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
