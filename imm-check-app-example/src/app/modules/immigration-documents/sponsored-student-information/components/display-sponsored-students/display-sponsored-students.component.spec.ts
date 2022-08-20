import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplaySponsoredStudentsComponent } from './display-sponsored-students.component';

describe('DisplaySponsoredStudentsComponent', () => {
  let component: DisplaySponsoredStudentsComponent;
  let fixture: ComponentFixture<DisplaySponsoredStudentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplaySponsoredStudentsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplaySponsoredStudentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
