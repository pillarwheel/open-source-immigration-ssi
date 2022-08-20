import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayI94sComponent } from './display-i94s.component';

describe('DisplayI94sComponent', () => {
  let component: DisplayI94sComponent;
  let fixture: ComponentFixture<DisplayI94sComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayI94sComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayI94sComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
