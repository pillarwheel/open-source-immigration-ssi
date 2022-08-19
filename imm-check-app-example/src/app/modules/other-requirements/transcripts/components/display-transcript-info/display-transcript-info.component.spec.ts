import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayTranscriptInfoComponent } from './display-transcript-info.component';

describe('DisplayTranscriptInfoComponent', () => {
  let component: DisplayTranscriptInfoComponent;
  let fixture: ComponentFixture<DisplayTranscriptInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayTranscriptInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayTranscriptInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
