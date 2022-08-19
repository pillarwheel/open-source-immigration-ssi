import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayFingerprintsInfoComponent } from './display-fingerprints-info.component';

describe('DisplayFingerprintsInfoComponent', () => {
  let component: DisplayFingerprintsInfoComponent;
  let fixture: ComponentFixture<DisplayFingerprintsInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayFingerprintsInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayFingerprintsInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
