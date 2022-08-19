import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayFaceScansInfoComponent } from './display-face-scans-info.component';

describe('DisplayFaceScansInfoComponent', () => {
  let component: DisplayFaceScansInfoComponent;
  let fixture: ComponentFixture<DisplayFaceScansInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayFaceScansInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayFaceScansInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
