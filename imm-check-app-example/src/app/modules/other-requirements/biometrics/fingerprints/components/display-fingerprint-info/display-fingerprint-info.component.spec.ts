import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayFingerprintInfoComponent } from './display-fingerprint-info.component';

describe('DisplayFingerprintInfoComponent', () => {
  let component: DisplayFingerprintInfoComponent;
  let fixture: ComponentFixture<DisplayFingerprintInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayFingerprintInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayFingerprintInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
