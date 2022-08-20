import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayI20Component } from './display-i20.component';

describe('DisplayI20Component', () => {
  let component: DisplayI20Component;
  let fixture: ComponentFixture<DisplayI20Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayI20Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayI20Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
