import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayPassportComponent } from './display-passport.component';

describe('DisplayPassportComponent', () => {
  let component: DisplayPassportComponent;
  let fixture: ComponentFixture<DisplayPassportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayPassportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayPassportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
