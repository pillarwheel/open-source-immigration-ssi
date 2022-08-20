import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayEyeInfoComponent } from './display-eye-info.component';

describe('DisplayEyeInfoComponent', () => {
  let component: DisplayEyeInfoComponent;
  let fixture: ComponentFixture<DisplayEyeInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayEyeInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayEyeInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
