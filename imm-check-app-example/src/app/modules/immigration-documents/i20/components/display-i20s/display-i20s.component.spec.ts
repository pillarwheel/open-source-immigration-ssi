import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayI20sComponent } from './display-i20s.component';

describe('DisplayI20sComponent', () => {
  let component: DisplayI20sComponent;
  let fixture: ComponentFixture<DisplayI20sComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayI20sComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayI20sComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
