import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayI94Component } from './display-i94.component';

describe('DisplayI94Component', () => {
  let component: DisplayI94Component;
  let fixture: ComponentFixture<DisplayI94Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayI94Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayI94Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
