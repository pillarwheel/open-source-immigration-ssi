import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayDs2019Component } from './display-ds2019.component';

describe('DisplayDs2019Component', () => {
  let component: DisplayDs2019Component;
  let fixture: ComponentFixture<DisplayDs2019Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayDs2019Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayDs2019Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
