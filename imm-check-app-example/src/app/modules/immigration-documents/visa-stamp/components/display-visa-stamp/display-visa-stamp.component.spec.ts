import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayVisaStampComponent } from './display-visa-stamp.component';

describe('DisplayVisaStampComponent', () => {
  let component: DisplayVisaStampComponent;
  let fixture: ComponentFixture<DisplayVisaStampComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayVisaStampComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayVisaStampComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
