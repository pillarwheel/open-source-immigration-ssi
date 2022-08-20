import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayFaceScanInfoComponent } from './display-face-scan-info.component';

describe('DisplayFaceScanInfoComponent', () => {
  let component: DisplayFaceScanInfoComponent;
  let fixture: ComponentFixture<DisplayFaceScanInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayFaceScanInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayFaceScanInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
