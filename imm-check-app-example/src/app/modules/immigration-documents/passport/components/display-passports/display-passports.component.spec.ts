import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayPassportsComponent } from './display-passports.component';

describe('DisplayPassportsComponent', () => {
  let component: DisplayPassportsComponent;
  let fixture: ComponentFixture<DisplayPassportsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayPassportsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayPassportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
