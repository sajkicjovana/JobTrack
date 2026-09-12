import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Preparation } from './preparation';

describe('Preparation', () => {
  let component: Preparation;
  let fixture: ComponentFixture<Preparation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Preparation]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Preparation);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
