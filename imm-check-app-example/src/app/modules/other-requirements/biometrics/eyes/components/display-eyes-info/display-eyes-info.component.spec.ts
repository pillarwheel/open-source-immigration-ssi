import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayEyesInfoComponent } from './display-eyes-info.component';

describe('DisplayEyesInfoComponent', () => {
  let component: DisplayEyesInfoComponent;
  let fixture: ComponentFixture<DisplayEyesInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayEyesInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayEyesInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
