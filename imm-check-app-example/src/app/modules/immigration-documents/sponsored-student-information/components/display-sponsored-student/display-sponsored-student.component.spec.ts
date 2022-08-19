import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplaySponsoredStudentComponent } from './display-sponsored-student.component';

describe('DisplaySponsoredStudentComponent', () => {
  let component: DisplaySponsoredStudentComponent;
  let fixture: ComponentFixture<DisplaySponsoredStudentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplaySponsoredStudentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplaySponsoredStudentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
