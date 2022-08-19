import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayVisaComponent } from './display-visa.component';

describe('DisplayVisaComponent', () => {
  let component: DisplayVisaComponent;
  let fixture: ComponentFixture<DisplayVisaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayVisaComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayVisaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
